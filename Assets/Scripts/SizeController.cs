using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SizeController : MonoBehaviour
{
    [Header("- Core -")]
    [Range(0, 99)]
    public int defaultWidth;
    [Range(0, 99)]
    public int defaultHeight;

    public DrawLayer drawLayer;
    public GameObject controller;

    public TMP_Text textMapSize;

    public TMP_InputField newWidth;
    public TMP_InputField newHeight;

    bool isOpenedController;
    Vector2Int currentMapSize;

    private void Awake()
    {
        currentMapSize.x = defaultWidth;
        currentMapSize.y = defaultHeight;

        isOpenedController = false;
    }

    public void Init()
    {
        Resize(defaultWidth, defaultHeight);
    }

    public void Init(int x, int y)
    {
        Resize(x, y);
    }

    public void OpenCloseController()
    {
        newWidth.text = "";
        newHeight.text = "";
        isOpenedController = !isOpenedController;
        controller.SetActive(isOpenedController);
    }

    public void CloseController()
    {
        newWidth.text = "";
        newHeight.text = "";
        controller.SetActive(false);
    }

    void Resize()
    {
        textMapSize.text = currentMapSize.x + " X " + currentMapSize.y;
        drawLayer.Resize(currentMapSize.x, currentMapSize.y);
    }

    void Resize(int x, int y)
    {
        currentMapSize.x = x;
        currentMapSize.y = y;
        Resize();
    }

    public void ResizeWithController()
    {
        currentMapSize.x = int.Parse(newWidth.text);
        currentMapSize.y = int.Parse(newHeight.text);
        Resize();
        CloseController();
    }
}
