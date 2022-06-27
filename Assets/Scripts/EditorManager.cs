using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    [Header(" - Default Setting -")]
    [Range(0, 99)]
    public int defaultWidth;
    [Range(0, 99)]
    public int defaultHeight;

    [Header("- Layer -")]
    public DrawLayer floorLayer;
    public DrawLayer itemLayer;

    [Header("- UI -")]
    public Toggle floorLayerSetting;
    public Toggle itemLayerSetting;

    [Header("- Camera -")]
    public float zoomSpeed;
    public float cameraMoveSpeed;
    public float minCameraSize;
    public float maxCameraSize;

    [SerializeField]
    public List<Sprite> floorList;
    [SerializeField]
    public List<Sprite> itemList;

    int width;
    int height;

    bool isLeftMouseDown;
    bool isRightMouseDown;

    Vector3 lastMousePos;

    private void Awake()
    {
        width = defaultWidth;
        height = defaultHeight;

        isLeftMouseDown = false;
        isRightMouseDown = false;

        InitSpriteList();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResizeMap();
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

    void ResizeMap()
    {
        floorLayer.Resize(width, height);
        itemLayer.Resize(width, height);
    }

    public void ActiveFloorLayer()
    {
        floorLayer.gameObject.SetActive(floorLayerSetting.isOn);
    }
    
    public void ActiveItemLayer()
    {
        itemLayer.gameObject.SetActive(itemLayerSetting.isOn);
    }

    void InitSpriteList()
    {
        
    }
}
