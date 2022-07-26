using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleFileBrowser;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class EditorManager : MonoBehaviour
{
    [Header("- Core -")]
    public SelectWindow selectWindow;
    public DrawLayer drawLayer;
    public Pallet pallet;
    public SizeController sizeController;
    public Setting setting;
    public List<ShortCutKeySet> shortCutKeySets;

    [Header("- Camera -")]
    public float zoomSpeed;
    public float cameraMoveSpeed;
    public float minCameraSize;
    public float maxCameraSize;

    bool isLeftMouseDown;
    bool isRightMouseDown;

    Vector3 lastMousePos;

    string defaultFolderPath;
    const string DefaultFileName = "EmptyMap";
    string filePath;
    string fileName;

    string mapDesigner;
    Vector2Int startIdx;
    int startLife;
    int maxLife;

    bool isSaved;
    bool isPauseCamera;
    bool isPauseShortCut;

    private void Awake()
    {
        isLeftMouseDown = false;
        isRightMouseDown = false;

        defaultFolderPath = Application.dataPath + "/MapData";

        fileName = DefaultFileName;
        filePath = defaultFolderPath + "/" + DefaultFileName + ".dat";
        mapDesigner = "UnKnownDesigner";
        startIdx = new Vector2Int(0, 0);
        startLife = 1;
        maxLife = 1;

        isSaved = true;
        isPauseCamera = false;
        isPauseShortCut = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        sizeController.Init();
        drawLayer.Init();

        string mappath = DataBus.Instance.ReadMapPath();
        if (mappath != null)
        {
            Load(mappath);
        }
    }

    void Init()
    {
        fileName = DefaultFileName;
        filePath = defaultFolderPath + "/" + DefaultFileName + ".dat";
        mapDesigner = "UnKnownDesigner";
        startIdx = new Vector2Int(0, 0);
        startLife = 1;
        maxLife = 1;

        sizeController.Init();
        drawLayer.InitBlocks();
        drawLayer.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPauseCamera)
        {
            UpdateCamera();
        }

        if (!isPauseShortCut)
        {
            UpdateShortCut();
        }
    }

    void UpdateCamera()
    {
        // 카메라 이동(키)
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");

        Vector3 currentCameraPos = Camera.main.transform.position;
        float moveHorizontal = inputHorizontal * cameraMoveSpeed * Camera.main.orthographicSize;
        float moveVertical = inputVertical * cameraMoveSpeed * Camera.main.orthographicSize;
        Camera.main.transform.position = currentCameraPos + new Vector3(moveHorizontal, moveVertical, 0);

        // 카메라 줌
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll, minCameraSize, maxCameraSize);

        // 카메라이동(마우스) & 마우스 클릭 확인
        if (Input.GetMouseButtonDown(0))
        {
            isLeftMouseDown = true;
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isLeftMouseDown = false;
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (isRightMouseDown && Input.GetMouseButton(1))
        {
            Camera.main.transform.position -= Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePos;
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void UpdateShortCut()
    {
        // 단축키
        // 등록
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetShortCut(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetShortCut(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetShortCut(3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SetShortCut(4);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SetShortCut(5);
            if (Input.GetKeyDown(KeyCode.Alpha6)) SetShortCut(6);
            if (Input.GetKeyDown(KeyCode.Alpha7)) SetShortCut(7);
            if (Input.GetKeyDown(KeyCode.Alpha8)) SetShortCut(8);
            if (Input.GetKeyDown(KeyCode.Alpha9)) SetShortCut(9);
            if (Input.GetKeyDown(KeyCode.Alpha0)) SetShortCut(0);
        }
        //사용
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) UseShortCut(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) UseShortCut(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) UseShortCut(3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) UseShortCut(4);
            if (Input.GetKeyDown(KeyCode.Alpha5)) UseShortCut(5);
            if (Input.GetKeyDown(KeyCode.Alpha6)) UseShortCut(6);
            if (Input.GetKeyDown(KeyCode.Alpha7)) UseShortCut(7);
            if (Input.GetKeyDown(KeyCode.Alpha8)) UseShortCut(8);
            if (Input.GetKeyDown(KeyCode.Alpha9)) UseShortCut(9);
            if (Input.GetKeyDown(KeyCode.Alpha0)) UseShortCut(0);
        }
    }

    void SetShortCut(int keyNum)
    {
        BlockType blockType = pallet.GetSelectedBlockType();
        Sprite sprite = pallet.GetSelectedSprite();

        if (sprite != null)
        {
            shortCutKeySets[keyNum].SetShortCut(blockType, sprite);
        }
    }

    void UseShortCut(int keyNum)
    {
        BlockType blockType = shortCutKeySets[keyNum].GetBlockType();
        Sprite sprite = shortCutKeySets[keyNum].GetSprite();

        if (sprite != null)
        {
            pallet.SelectBlock(sprite, blockType);
            drawLayer.Paint();
        }
    }

    public void PauseCamera()
    {
        isPauseCamera = true;
    }

    public void ResumeCamera()
    {
        isPauseCamera = false;
    }

    public void PauseShortCut()
    {
        isPauseShortCut = true;
    }

    public void ResumeShortCut()
    {
        isPauseShortCut = false;
    }

    public string GetMapDesigner()
    {
        return mapDesigner;
    }

    public void SetMapDesigner(string str)
    {
        mapDesigner = str;
        ChangedAnyData();
    }

    public Vector2Int GetStartIdx()
    {
        return startIdx;
    }

    public void SetStartIdx(Vector2Int idx)
    {
        startIdx = idx;
        ChangedAnyData();
    }

    public int GetStartLife()
    {
        return startLife;
    }

    public void SetStartLife(int _startLife)
    {
        startLife = _startLife;
        ChangedAnyData();
    }
    
    public int GetMaxLife()
    {
        return maxLife;
    }

    public void SetMaxLife(int _maxLife)
    {
        maxLife = _maxLife;
        ChangedAnyData();
    }

    public void ButtonNew()
    {
        if (isSaved)
        {
            Init();
        }
        else
        {
            selectWindow.OpenSelectWindow(Init, null, true, "Not saved. Would you like to continue?", "New", "Cancel");
        }
    }

    public void ButtonOpen()
    {
        FileBrowser.ShowLoadDialog((paths) => { Load(paths[0]); },
                                   () => { },
                                   FileBrowser.PickMode.Files, false, defaultFolderPath, null, "Load", "Select");
    }

    public void ButtonTestCurrentMap()
    {
        if (!isSaved)
        {
            selectWindow.OpenSelectWindow(null, null, false, "Not saved. Please save and try again.", "OK", "");
            return;
        }

        DataBus.Instance.WriteMapPath(filePath);
        SceneManager.LoadScene("GameScene");
    }

    public void ButtonSave()
    {
        if (fileName == DefaultFileName)
        {
            ButtonSaveAs();
        }
        else
        {
            SaveAs(filePath, true);
        }
    }

    public void ButtonSaveAs()
    {
        FileBrowser.ShowSaveDialog((paths) => { SaveAs(paths[0] + ".dat", false); },
                                   () => { },
                                   FileBrowser.PickMode.Files, false, defaultFolderPath, null, "Save As", "Save");
    }

    void QuitEditor()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void ButtonQuit()
    {
        if (isSaved)
        {
            QuitEditor();
        }
        else
        {
            selectWindow.OpenSelectWindow(QuitEditor, null, true, "Not saved. Would you like to continue?", "Quit", "Cancel");
        }
    }

    void Load(string path)
    {
        if (File.Exists(path))
        {
            int lastindexofslash = path.LastIndexOf('\\');
            fileName = path.Substring(lastindexofslash + 1, path.Length - (lastindexofslash + 1));
            fileName = fileName.Substring(0, fileName.LastIndexOf(".dat"));
            filePath = path;


            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            MapData mapData = (MapData)binaryFormatter.Deserialize(file);
            file.Close();

            mapDesigner = mapData.mapDesigner;
            startIdx = new Vector2Int(mapData.startIdx.x, mapData.startIdx.y);
            startLife = mapData.startLife;
            maxLife = mapData.maxLife;
            sizeController.Init(mapData.mapWidth, mapData.mapHeight);
            drawLayer.InitBlocks(mapData);

            isSaved = true;
        }
        else
        {
            throw new System.Exception("데이터 불러오기 실패" + path);
        }
    }

    void SaveAs(string path, bool isResave)
    {
        if (!isResave && File.Exists(path))
        {
            selectWindow.OpenSelectWindow(null, null, false, "The file name already exists", "OK", "");
            return;
        }

        int lastindexofslash = path.LastIndexOf('\\');
        fileName = path.Substring(lastindexofslash + 1, path.Length - (lastindexofslash + 1));
        fileName = fileName.Substring(0, fileName.LastIndexOf(".dat"));
        filePath = path;

        // MapData 객체생성
        MapData mapData = new MapData();

        int mapwidth = drawLayer.GetMapSize().x;
        int mapheight = drawLayer.GetMapSize().y;

        mapData.appVersion = Application.version;
        mapData.mapDesigner = mapDesigner;
        mapData.startIdx = new PairInt(startIdx.x, startIdx.y);
        mapData.startLife = startLife;
        mapData.maxLife = maxLife;
        mapData.mapWidth = mapwidth;
        mapData.mapHeight = mapheight;

        mapData.floorData = new List<List<string>>();
        for (int i = 0; i < mapwidth; i++)
        {
            mapData.floorData.Add(new List<string>());
            for (int j = 0; j < mapheight; j++)
            {
                mapData.floorData[i].Add(drawLayer.GetFloorBlock(i, j).GetSprite().name);
            }
        }

        mapData.itemData = new List<List<string>>();
        for (int i = 0; i < mapwidth; i++)
        {
            mapData.itemData.Add(new List<string>());
            for (int j = 0; j < mapheight; j++)
            {
                Sprite itemSprite = drawLayer.GetItemBlock(i, j).GetSprite();
                string tileName = "";
                if (itemSprite != null)
                {
                    tileName = itemSprite.name;
                }
                mapData.itemData[i].Add(tileName);
            }
        }

        mapData.powerData = new Dictionary<PairInt, List<PairInt>>();
        mapData.portalData = new Dictionary<PairInt, PairInt>();
        for (int i = 0; i < mapwidth; i++)
        {
            for (int j = 0; j < mapheight; j++)
            {
                Block floorBlock = drawLayer.GetFloorBlock(i, j);
                if (floorBlock.GetSprite().name.Contains("Button_"))
                {
                    HashSet<Vector2Int> targets = floorBlock.GetTargets();
                    List<PairInt> targetList = new List<PairInt>();
                    foreach(Vector2Int target in targets)
                    {
                        targetList.Add(new PairInt(target.x, target.y));
                    }

                    if (targetList.Count > 0)
                    {
                        mapData.powerData.Add(new PairInt(i, j), targetList);
                    }
                }
                else if (floorBlock.GetSprite().name.Contains("Portal_"))
                {
                    HashSet<Vector2Int> targets = floorBlock.GetTargets();
                    PairInt targetPair = null;
                    foreach(Vector2Int target in targets)
                    {
                        targetPair = new PairInt(target.x, target.y);
                    }

                    if (targetPair != null)
                    {
                        mapData.portalData.Add(new PairInt(i, j), targetPair);
                    }
                }
            }
        }

        mapData.trapData = new Dictionary<PairInt, ThreeInt>();

        HashSet<Vector2Int> trapset = drawLayer.GetAutoTrapSet();
        foreach(Vector2Int trapidx in trapset)
        {
            mapData.trapData.Add(new PairInt(trapidx.x, trapidx.y), drawLayer.GetFloorBlock(trapidx.x, trapidx.y).GetTrapDelay());
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(path);

        binaryFormatter.Serialize(file, mapData);
        file.Close();

        Debug.Log("저장완료");

        isSaved = true;
    }

    public void ChangedAnyData()
    {
        isSaved = false;
    }
}
