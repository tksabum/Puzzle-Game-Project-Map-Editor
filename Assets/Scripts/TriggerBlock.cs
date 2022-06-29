using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerBlock : MonoBehaviour
{
    DrawLayer drawLayer;

    Vector2Int idx;

    private void Awake()
    {
        drawLayer = GameObject.Find("DrawLayer").GetComponent<DrawLayer>();
    }

    private void OnMouseEnter()
    {
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
