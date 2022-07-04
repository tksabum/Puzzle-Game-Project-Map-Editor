using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleFileBrowser;
using System.Runtime.Serialization.Formatters.Binary;

public class EditorManager : MonoBehaviour
{
    [Header("- Core -")]
    public DrawLayer drawLayer;
    public SizeController sizeController;
    public Setting setting;

    [Header("- Camera -")]
    public float zoomSpeed;
    public float cameraMoveSpeed;
    public float minCameraSize;
    public float maxCameraSize;

    bool isLeftMouseDown;
    bool isRightMouseDown;

    Vector3 lastMousePos;

    string defaultFolderPath;
    string filePath;

    const string DefaultFileName = "EmptyMap";

    string mapDesigner;
    Vector2Int startIdx;
    int startLife;
    int maxLife;

    private void Awake()
    {
        isLeftMouseDown = false;
        isRightMouseDown = false;

        defaultFolderPath = Application.dataPath + "/MapData";

        filePath = defaultFolderPath + "/" + DefaultFileName + ".dat";
        mapDesigner = "UnKnownDesigner";
        startIdx = new Vector2Int(0, 0);
        startLife = 1;
        maxLife = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        sizeController.Init();
        drawLayer.Init();
    }

    // Update is called once per frame
    void Update()
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

    public string GetMapDesigner()
    {
        return mapDesigner;
    }

    public void SetMapDesigner(string str)
    {
        mapDesigner = str;
    }

    public Vector2Int GetStartIdx()
    {
        return startIdx;
    }

    public void SetStartIdx(Vector2Int idx)
    {
        startIdx = idx;
    }

    public int GetStartLife()
    {
        return startLife;
    }

    public void SetStartLife(int _startLife)
    {
        startLife = _startLife;
    }

    public int GetMaxLife()
    {
        return maxLife;
    }

    public void SetMaxLife(int _maxLife)
    {
        maxLife = _maxLife;
    }





    public void ButtonSave()
    {
        if (filePath == defaultFolderPath + "/" + DefaultFileName + ".dat")
        {
            ButtonSaveAs();
        }
        else
        {
            SaveAs(filePath);
        }
    }

    public void ButtonSaveAs()
    {
        FileBrowser.ShowSaveDialog((paths) => { SaveAs(paths[0] + ".dat"); },
                                   () => { },
                                   FileBrowser.PickMode.Files, false, defaultFolderPath, null, "Save As", "Save");
    }

    void SaveAs(string path)
    {
        if (File.Exists(path))
        {
            throw new System.Exception("이미 존재함");
        }

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

                    mapData.powerData.Add(new PairInt(i, j), targetList);
                }
                else if (floorBlock.GetSprite().name.Contains("Portal_"))
                {
                    HashSet<Vector2Int> targets = floorBlock.GetTargets();
                    PairInt targetPair = new PairInt(0, 0);
                    foreach(Vector2Int target in targets)
                    {
                        targetPair = new PairInt(target.x, target.y);
                    }

                    mapData.portalData.Add(new PairInt(i, j), targetPair);
                }
            }
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(path);

        binaryFormatter.Serialize(file, mapData);
        file.Close();

        Debug.Log("저장완료");
    }
}
