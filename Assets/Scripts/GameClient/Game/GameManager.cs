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
    [Tooltip("Zoom In ���¿��� �÷��̾ ������ ���� �� ���̴� �� ���� ����")]
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

        // ������ ���
        UseItem();

        // ī�޶�
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

        // �̵� ó��
        Move();
    }

    private void Init()
    {
        // ���� �ʱ�ȭ
        playeridx = startidx;
        life = startlife;
        preparedItem = BlockManager.Obj.EMPTY;
        moveState = MoveState.INPUT;
        moveType = MoveType.WALK;

        // �÷��̾� �ʱ�ȭ
        player.transform.position = (Vector2)playeridx;
        playerDirection = BlockManager.Direction.DOWN;
        playerWalk = false;

        //�ִϸ����� �ʱ�ȭ
        playerAnimator.SetBool("walking", playerWalk);
        playerAnimator.SetFloat("DirX", 0f);
        playerAnimator.SetFloat("DirY", -1f);

        // UI �ʱ�ȭ
        pauseButton.interactable = true;
        uiManager.Reset(life, mapName, preparedItem);
    }

    // ������ ���
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

    // �� ������ ���� ����Ǵ� ī�޶� ���� ó��
    void UpdateCamera()
    {
        // ī�޶� ��
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

        /* ī�޶� ũ�� ������ �ʿ�
        if(scroll != 0f)
        {
            Debug.Log(mainCamera.orthographicSize);
        }
        /* */

        // ī�޶� �̵�
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

    // �� ������ ���� ����Ǵ� �̵� ���� ó��
    void Move()
    {
        // �Է�
        if (moveState == MoveState.INPUT)
        {
            MoveStateInput();
        }
        
        // �̵� �� �ִϸ��̼�
        if (moveState == MoveState.PREANIME)
        {
            MoveStatePreAnime();
        }

        // �̵� �� �ִϸ��̼�
        if (moveState == MoveState.ANIME)
        {
            MoveStateAnime();
        }

        // �̵� �� �ִϸ��̼�
        if (moveState == MoveState.POSTANIME)
        {
            MoveStatePostAnime();
        }
    }

    // �Է�
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
            // �ִϸ��̼� ����
            playerWalk = false;
            playerAnimator.SetBool("walking", playerWalk);

            return;
        }

        playerAnimator.SetFloat("DirX", (float)moveDir.x);
        playerAnimator.SetFloat("DirY", (float)moveDir.y);

        Vector2Int nowidx = playeridx;
        Vector2Int nextidx = playeridx + moveDir;

        // �� �� �ִ� ��ġ���� üũ
        if (blockManager.Movable(BlockManager.Obj.PLAYER, nowidx, nextidx))
        {
            // �� �� �ִٸ� �̵�
            walkStartPos = nowidx;
            walkEndPos = nextidx;

            moveType = MoveType.WALK;
            moveState = MoveState.PREANIME;
        }
    }

    // �̵� �� �ִϸ��̼�
    void MoveStatePreAnime()
    {
        if (moveType == MoveType.WALK)
        {
            // walk�� �̵� �� ������ ����
            moveState = MoveState.ANIME;

            // �̵� �� �̺�Ʈ ó��
            blockManager.PreMoveEvent(BlockManager.Obj.PLAYER, walkStartPos, walkEndPos);
        }
    }

    // �̵� �� �ִϸ��̼�
    void MoveStateAnime()
    {
        if (moveType == MoveType.WALK)
        {
            // �ִϸ��̼� ���
            playerWalk = true;
            playerAnimator.SetBool("walking", playerWalk);

            // ��ġ �̵�
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
                // ���� �̵��Ÿ��� �̵��� �Ÿ����� ª�� ��� ��ġ ���� �̵�
                if (playeridxXY != endPosXY && Mathf.Abs(endPosXY - nextPosXY) < Mathf.Abs(nextPosXY - startPosXY))
                {
                    playeridx = walkEndPos;

                    // �̵� �� �̺�Ʈ ó��
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

    // �̵� �� �ִϸ��̼�
    void MoveStatePostAnime()
    {
        if (moveType == MoveType.WALK)
        {
            // walk�� �̵� �� ������ ����
            moveState = MoveState.INPUT;

            // �̵� �� �̺�Ʈ ó��
            blockManager.PostMoveEvent(BlockManager.Obj.PLAYER, walkStartPos, walkEndPos);
        }
    }

    // ������ ȹ��
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

    // Goal ������ ȹ��
    public void GetGoal()
    {
        GameClear();
    }

    // �÷��̾� ���� ����
    public void AttackedPlayer()
    {
        addLife(-1);
    }

    // Ư�� ��ġ ���� ����
    public void AttackedPos(Vector2Int attackedidx)
    {
        if (playeridx == attackedidx)
        {
            AttackedPlayer();
        }
    }

    // ��Ż�� �ⱸ�� �̵�
    public void MovePortal(Vector2Int portalidx)
    {
        Vector2Int nextPos = blockManager.GetPortalExit(playeridx);
        playeridx = nextPos;
        player.transform.position = (Vector2)nextPos;
    }

    // ������ ���� (value�� ���� ����)
    void addLife(int value)
    {
        life = Mathf.Clamp(life + value, 0, maxlife);
        uiManager.SetLife(life);
        if (life == 0)
        {
            GameOver();
        }
    }

    // �� ������ �ε�
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
            throw new System.Exception("������ �ҷ����� ���� File Name: " + dataPath);
        }
    }

    // �Ͻ�����
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

    // �޴��� ���ư���
    public void ButtonBack()
    {
        DataBus.Instance.WriteMapName(mapName);
        SceneManager.LoadScene("MapEditor");
    }

    // ���� ����
    void GameOver()
    {
        Time.timeScale = 0f;
        pauseButton.interactable = false;
        uiManager.SetGameOver(true);
    }

    // ���� Ŭ����
    void GameClear()
    {
        Time.timeScale = 0f;
        pauseButton.interactable = false;
        uiManager.SetGameClear(true);

        // Ŭ���� ��� ����
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

    // timeScale �ʱ�ȭ
    public void ButtonResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    // ���� �ʱ�ȭ
    public void ButtonResetGame()
    {
        Init();

        blockManager.ResetBlock();
    }

    // ���������� ���� �ʱ�ȭ
    // NextButton�� 10�� ���� �ƴ� ���丮�ʿ����� Active���°� ��
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
