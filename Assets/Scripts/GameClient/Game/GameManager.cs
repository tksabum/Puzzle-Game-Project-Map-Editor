using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum MoveState
{
    INPUT,
    PREANIME,
    ANIME,
    POSTANIME
}

enum MoveType
{
    WALK
}

public class GameManager : MonoBehaviour
{
    [Header("- UI -")]
    public UIManager uiManager;
    public Button pauseButton;

    [Header("- Block -")]
    public BlockManager blockManager;

    [Header("- Player - ")]
    public GameObject player;
    public Animator playerAnimator;
    public float playerSpeed;

    bool playerWalk;
    BlockManager.Direction playerDirection;
    Vector2Int walkStartPos;
    Vector2Int walkEndPos;
    
    Vector2Int startidx;
    int startlife;
    int maxlife;

    Vector2Int playeridx;
    int life;

    BlockManager.Obj preparedItem;

    [Header(" - Camera - ")]
    public Camera mainCamera;
    public float zoomSpeed;
    public float minCameraSize;
    public float maxCameraSize;
    [Tooltip("Zoom In 상태에서 플레이어가 구석에 있을 때 보이는 맵 밖의 여백")]
    public float padding;

    [Header(" - File - ")]
    public string testMapName;

    string mapName;
    string dataPath;

    MapData mapData;

    MoveState moveState;
    MoveType moveType;

    float lastTickTimer;
    float tickTimer;

    // Start is called before the first frame update
    void Start()
    {
        mapName = DataBus.Instance.ReadMapName();

        if (mapName == null)
        {
            mapName = testMapName;
        }

        dataPath = Application.dataPath + "/MapData/" + mapName + ".dat";

        mapData = Load();

        startidx = new Vector2Int(mapData.startIdx.x, mapData.startIdx.y);
        startlife = mapData.startLife;
        maxlife = mapData.maxLife;

        Init();

        blockManager.SetBlock(mapData);

        lastTickTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        // 아이템 사용
        UseItem();

        // 카메라
        UpdateCamera();

        tickTimer = Time.time - lastTickTimer;
        
        // Tick
        if (0.125f < tickTimer && tickTimer < 0.25f)
        {
            blockManager.Tick();
            lastTickTimer = Time.time;
        }
        else if (tickTimer >= 0.25f)
        {
            for (int i = 0; i < tickTimer / 0.125f; i++)
            {
                blockManager.Tick();
                lastTickTimer = Time.time;
            }
        }

        // 이동 처리
        Move();
    }

    private void Init()
    {
        // 변수 초기화
        playeridx = startidx;
        life = startlife;
        preparedItem = BlockManager.Obj.EMPTY;
        moveState = MoveState.INPUT;
        moveType = MoveType.WALK;

        // 플레이어 초기화
        player.transform.position = (Vector2)playeridx;
        playerDirection = BlockManager.Direction.DOWN;
        playerWalk = false;

        //애니메이터 초기화
        playerAnimator.SetBool("walking", playerWalk);
        playerAnimator.SetFloat("DirX", 0f);
        playerAnimator.SetFloat("DirY", -1f);

        // UI 초기화
        pauseButton.interactable = true;
        uiManager.Reset(life, mapName, preparedItem);
    }

    // 아이템 사용
    void UseItem()
    {
        if (!playerWalk && preparedItem != BlockManager.Obj.EMPTY && Input.GetKey(KeyCode.Space))
        {
            if (preparedItem == BlockManager.Obj.HAMMER)
            {
                if (blockManager.ItemUsable(preparedItem, playeridx, playerDirection))
                {
                    blockManager.ItemUseEvent(preparedItem, playeridx, playerDirection);
                    preparedItem = BlockManager.Obj.EMPTY;
                    uiManager.SetItem(preparedItem);
                }
            }
        }
    }

    // 매 프레임 마다 실행되는 카메라 관련 처리
    void UpdateCamera()
    {
        // 카메라 줌
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = mainCamera.aspect * mainCamera.orthographicSize;

        float horizMin = -0.5f + cameraWidth - padding;
        float horizMax = (float)mapData.mapWidth - 0.5f - cameraWidth + padding;
        float verMin = -0.5f + cameraHeight - padding;
        float verMax = (float)mapData.mapHeight - 0.5f - cameraHeight + padding;

        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        float maxCameraSizeHeight = ((float)mapData.mapHeight + 2 * padding) / 2f;
        float maxCameraSizeWidth = ((float)mapData.mapWidth + 2 * padding) / (2f * mainCamera.aspect);
        float maxCameraSizeTotal = Mathf.Clamp(Mathf.Max(maxCameraSizeHeight, maxCameraSizeWidth), minCameraSize, maxCameraSize);

        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - scroll, minCameraSize, maxCameraSizeTotal);

        /* 카메라 크기 조정에 필요
        if(scroll != 0f)
        {
            Debug.Log(mainCamera.orthographicSize);
        }
        /* */

        // 카메라 이동
        Vector3 targetPos = player.transform.position;
        Vector3 centerPos = new Vector3(((float)mapData.mapWidth - 1f) / 2f, ((float)mapData.mapHeight - 1f) / 2f, 0);
        if (horizMin < horizMax)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, horizMin, horizMax);
        }
        else
        {
            targetPos.x = centerPos.x;
        }
        if (verMin < verMax)
        {
            targetPos.y = Mathf.Clamp(targetPos.y, verMin, verMax);
        }
        else
        {
            targetPos.y = centerPos.y;
        }
        targetPos.z = mainCamera.transform.position.z;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, 0.1f);
    }

    // 매 프레임 마다 실행되는 이동 관련 처리
    void Move()
    {
        // 입력
        if (moveState == MoveState.INPUT)
        {
            MoveStateInput();
        }
        
        // 이동 전 애니메이션
        if (moveState == MoveState.PREANIME)
        {
            MoveStatePreAnime();
        }

        // 이동 중 애니메이션
        if (moveState == MoveState.ANIME)
        {
            MoveStateAnime();
        }

        // 이동 후 애니메이션
        if (moveState == MoveState.POSTANIME)
        {
            MoveStatePostAnime();
        }
    }

    // 입력
    void MoveStateInput()
    {
        Vector2Int moveDir = Vector2Int.zero;

        if (Input.GetKey(KeyCode.D))
        {
            moveDir = Vector2Int.right;
            playerDirection = BlockManager.Direction.RIGHT;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveDir = Vector2Int.left;
            playerDirection = BlockManager.Direction.LEFT;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            moveDir = Vector2Int.up;
            playerDirection = BlockManager.Direction.UP;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir = Vector2Int.down;
            playerDirection = BlockManager.Direction.DOWN;
        }
        else
        {
            // 애니메이션 정지
            playerWalk = false;
            playerAnimator.SetBool("walking", playerWalk);

            return;
        }

        playerAnimator.SetFloat("DirX", (float)moveDir.x);
        playerAnimator.SetFloat("DirY", (float)moveDir.y);

        Vector2Int nowidx = playeridx;
        Vector2Int nextidx = playeridx + moveDir;

        // 갈 수 있는 위치인지 체크
        if (blockManager.Movable(BlockManager.Obj.PLAYER, nowidx, nextidx))
        {
            // 갈 수 있다면 이동
            walkStartPos = nowidx;
            walkEndPos = nextidx;

            moveType = MoveType.WALK;
            moveState = MoveState.PREANIME;
        }
    }

    // 이동 전 애니메이션
    void MoveStatePreAnime()
    {
        if (moveType == MoveType.WALK)
        {
            // walk는 이동 전 동작이 없음
            moveState = MoveState.ANIME;

            // 이동 전 이벤트 처리
            blockManager.PreMoveEvent(BlockManager.Obj.PLAYER, walkStartPos, walkEndPos);
        }
    }

    // 이동 중 애니메이션
    void MoveStateAnime()
    {
        if (moveType == MoveType.WALK)
        {
            // 애니메이션 재생
            playerWalk = true;
            playerAnimator.SetBool("walking", playerWalk);

            // 위치 이동
            Vector3 nextPos = player.transform.position + (playerSpeed * (Vector3)(Vector2)(walkEndPos - walkStartPos) * Time.deltaTime);

            float nextPosXY = nextPos.y;
            float playPosXY = player.transform.position.y;
            float startPosXY = walkStartPos.y;
            float endPosXY = walkEndPos.y;

            int playeridxXY = playeridx.y;

            if (playerDirection == BlockManager.Direction.LEFT || playerDirection == BlockManager.Direction.RIGHT)
            {
                nextPosXY = nextPos.x;
                playPosXY = player.transform.position.x;
                startPosXY = walkStartPos.x;
                endPosXY = walkEndPos.x;
                playeridxXY = playeridx.x;
            }

            if (((endPosXY - playPosXY) > 0 && nextPosXY < endPosXY) || ((endPosXY - playPosXY) < 0 && nextPosXY > endPosXY))
            {
                // 남은 이동거리가 이동한 거리보다 짧을 경우 위치 판정 이동
                if (playeridxXY != endPosXY && Mathf.Abs(endPosXY - nextPosXY) < Mathf.Abs(nextPosXY - startPosXY))
                {
                    playeridx = walkEndPos;

                    // 이동 중 이벤트 처리
                    blockManager.MoveEvent(BlockManager.Obj.PLAYER, walkStartPos, walkEndPos);
                }
                player.transform.position = nextPos;
            }
            else
            {
                player.transform.position = (Vector2)walkEndPos;
                moveState = MoveState.POSTANIME;
            }
        }
    }

    // 이동 후 애니메이션
    void MoveStatePostAnime()
    {
        if (moveType == MoveType.WALK)
        {
            // walk는 이동 후 동작이 없음
            moveState = MoveState.INPUT;

            // 이동 후 이벤트 처리
            blockManager.PostMoveEvent(BlockManager.Obj.PLAYER, walkStartPos, walkEndPos);
        }
    }

    // 아이템 획득
    public void GetItem(BlockManager.Obj obj)
    {
        if (obj == BlockManager.Obj.HAMMER)
        {
            preparedItem = BlockManager.Obj.HAMMER;
        }
        else if (obj == BlockManager.Obj.LIFE)
        {
            addLife(1);
        }
        else
        {
            throw new System.Exception("Error:get wrong item");
        }

        uiManager.SetItem(preparedItem);
    }

    // Goal 아이템 획득
    public void GetGoal()
    {
        GameClear();
    }

    // 플레이어 공격 받음
    public void AttackedPlayer()
    {
        addLife(-1);
    }

    // 특정 위치 공격 받음
    public void AttackedPos(Vector2Int attackedidx)
    {
        if (playeridx == attackedidx)
        {
            AttackedPlayer();
        }
    }

    // 포탈의 출구로 이동
    public void MovePortal(Vector2Int portalidx)
    {
        Vector2Int nextPos = blockManager.GetPortalExit(playeridx);
        playeridx = nextPos;
        player.transform.position = (Vector2)nextPos;
    }

    // 라이프 조정 (value값 음수 가능)
    void addLife(int value)
    {
        life = Mathf.Clamp(life + value, 0, maxlife);
        uiManager.SetLife(life);
        if (life == 0)
        {
            GameOver();
        }
    }

    // 맵 데이터 로드
    public MapData Load()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            MapData mapData = (MapData)binaryFormatter.Deserialize(file);
            file.Close();

            return mapData;
        }
        else
        {
            throw new System.Exception("데이터 불러오기 실패 File Name: " + dataPath);
        }
    }

    // 일시정지
    public void ButtonPause()
    {
        if (Time.timeScale == 0f)
        {
            uiManager.SetPause(false);
            Time.timeScale = 1f;
        }
        else
        {
            uiManager.SetPause(true);
            Time.timeScale = 0f;
        }
    }

    // 메뉴로 돌아가기
    public void ButtonBack()
    {
        DataBus.Instance.WriteMapName(mapName);
        SceneManager.LoadScene("MapEditor");
    }

    // 게임 오버
    void GameOver()
    {
        Time.timeScale = 0f;
        pauseButton.interactable = false;
        uiManager.SetGameOver(true);
    }

    // 게임 클리어
    void GameClear()
    {
        Time.timeScale = 0f;
        pauseButton.interactable = false;
        uiManager.SetGameClear(true);

        // 클리어 기록 갱신
        if (mapName.Length > 6 && mapName.Substring(0, 6) == "Story ")
        {
            string str1 = "";
            string str2 = "";
            bool flag = true;
            foreach(char c in mapName.Substring(6, mapName.Length - 6))
            {
                if (flag)
                {
                    if (c != '-')
                    {
                        str1 += c;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    str2 += c;
                }
            }

            int nowStoryNum = int.Parse(str1);
            int nowMapNum = int.Parse(str2);
            int saveStoryNum = PlayerPrefs.GetInt("clearInfo_storyNum", 0);
            int saveMapNum = PlayerPrefs.GetInt("clearInfo_mapNum", 10);

            if ((saveStoryNum < nowStoryNum) || (saveStoryNum == nowStoryNum && saveMapNum < nowMapNum))
            {
                PlayerPrefs.SetInt("clearInfo_storyNum", nowStoryNum);
                PlayerPrefs.SetInt("clearInfo_mapNum", nowMapNum);
            }
        }
    }

    // timeScale 초기화
    public void ButtonResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    // 게임 초기화
    public void ButtonResetGame()
    {
        Init();

        blockManager.ResetBlock();
    }

    // 다음맵으로 게임 초기화
    // NextButton은 10번 맵이 아닌 스토리맵에서만 Active상태가 됨
    public void ButtonNextGame()
    {
        string str1 = "";
        string str2 = "";
        bool flag = true;
        foreach (char c in mapName.Substring(6, mapName.Length - 6))
        {
            if (flag)
            {
                if (c != '-')
                {
                    str1 += c;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                str2 += c;
            }
        }

        int nowStoryNum = int.Parse(str1);
        int nowMapNum = int.Parse(str2);
        int nextStoryNum = nowStoryNum + (nowMapNum / 10);
        int nextMapNum = (nowMapNum % 10) + 1;

        string nextMapName = "Story " + nextStoryNum + "-" + nextMapNum;

        mapName = nextMapName;


        dataPath = Application.dataPath + "/MapData/" + mapName + ".dat";

        mapData = Load();

        startidx = new Vector2Int(mapData.startIdx.x, mapData.startIdx.y);
        startlife = mapData.startLife;
        maxlife = mapData.maxLife;

        Init();

        blockManager.RemoveBlock();
        blockManager.SetBlock(mapData);
    }

    public Vector2Int GetPlayerIdx()
    {
        return playeridx;
    }
}
