using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class EditorManager : MonoBehaviour
{
    [Header("- Core -")]
    public DrawLayer drawLayer;
    public SizeController sizeController;

    [Header("- Camera -")]
    public float zoomSpeed;
    public float cameraMoveSpeed;
    public float minCameraSize;
    public float maxCameraSize;

    bool isLeftMouseDown;
    bool isRightMouseDown;

    Vector3 lastMousePos;

    string defaultPath;

    string mapDesigner;
    Vector2Int startIdx;
    int startLife;
    int maxLife;

    private void Awake()
    {
        isLeftMouseDown = false;
        isRightMouseDown = false;

        defaultPath = Application.dataPath + "/MapData";

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

    public Vector2Int GetStartIdx()
    {
        return startIdx;
    }

    public void ButtonSave()
    {

    }

    public void ButtonSaveAs()
    {
        FileBrowser.ShowSaveDialog((paths) => { SaveAs(paths[0]); },
                                   () => { },
                                   FileBrowser.PickMode.Files, false, defaultPath, null, "Save As", "Save");
    }

    void SaveAs(string path)
    {

    }
}
