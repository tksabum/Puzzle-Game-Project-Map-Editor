using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerBlock : MonoBehaviour
{
    DrawLayer drawLayer;

    Vector2Int idx;

    bool isMouseEntered;
    bool isMouseLeftClicked;
    bool isMouseRightClicked;
    Vector3 lastMouseLeftClickedPos;
    Vector3 lastMouseRightClickedPos;

    float mouseTolerance = Vector3.SqrMagnitude(new Vector3(3, 3, 0));

    private void Awake()
    {
        drawLayer = GameObject.Find("DrawLayer").GetComponent<DrawLayer>();

        isMouseEntered = false;
        isMouseLeftClicked = false;
        isMouseRightClicked = false;
        lastMouseLeftClickedPos = Vector3.zero;
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
            if (Vector3.SqrMagnitude(lastMouseRightClickedPos - Input.mousePosition) > mouseTolerance)
            {
                isMouseRightClicked = false;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    drawLayer.TriggerMouseRightClick(idx);
                }
            }
        }


        if (isMouseEntered && Input.GetMouseButtonDown(0))
        {
            isMouseLeftClicked = true;
            lastMouseLeftClickedPos = Input.mousePosition;
        }

        if (isMouseLeftClicked)
        {
            if (Vector3.SqrMagnitude(lastMouseLeftClickedPos - Input.mousePosition) > mouseTolerance)
            {
                isMouseLeftClicked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    drawLayer.TriggerMouseLeftClick(idx);
                }
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
