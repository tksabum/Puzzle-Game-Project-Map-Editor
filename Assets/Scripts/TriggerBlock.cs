using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerBlock : MonoBehaviour
{
    DrawLayer drawLayer;

    Vector2Int idx;

    bool isMouseEntered;
    bool isMouseRightClicked;
    Vector3 lastMouseRightClickedPos;

    private void Awake()
    {
        drawLayer = GameObject.Find("DrawLayer").GetComponent<DrawLayer>();

        isMouseEntered = false;
        isMouseRightClicked = false;
        lastMouseRightClickedPos = Vector3.zero;
    }

    public void Update()
    {
        if (isMouseEntered && Input.GetMouseButtonDown(1))
        {
            isMouseRightClicked = true;
            lastMouseRightClickedPos = Input.mousePosition;
        }

        if (isMouseRightClicked)
        {
            if (lastMouseRightClickedPos != Input.mousePosition)
            {
                isMouseRightClicked = false;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Debug.Log("타일정보창 열기");
            }
        }
    }

    private void OnMouseEnter()
    {
        isMouseEntered = true;

        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                drawLayer.TriggerMouseDown(idx);
            }
        }
        else
        {
            drawLayer.TriggerMouseEnter(idx);
        }
    }

    private void OnMouseExit()
    {
        isMouseEntered = false;
        isMouseRightClicked = false;
        drawLayer.TriggerMouseExit(idx);
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            drawLayer.TriggerMouseDown(idx);
        }
    }

    public void SetIdx(Vector2Int _idx)
    {
        idx = _idx;
    }
}
