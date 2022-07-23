using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockSettingWindow : MonoBehaviour
{
    public DrawLayer drawLayer;

    public TMP_Text textBlockType;
    public TMP_Text textBlockIdx;
    public TMP_Text textNumberOfTarget;

    public GameObject trapInfo;
    public Toggle trapInfoToggleAutomatic;
    public Toggle trapInfoToggleManual;
    public TMP_InputField trapInfoFirstDelay;
    public TMP_InputField trapInfoOddDelay;
    public TMP_InputField trapInfoEvenDelay;

    public GameObject textEmpty;
    public List<TMP_Text> textTargets;

    bool isOpened;
    Vector2Int currentIdx;

    bool isButtonOff;

    private void Awake()
    {
        isOpened = false;
        isButtonOff = false;
        currentIdx = Vector2Int.zero;

        trapInfoFirstDelay.onEndEdit.AddListener(delegate { ValidateDelay(); });
        trapInfoOddDelay.onEndEdit.AddListener(delegate { ValidateDelay(); });
        trapInfoEvenDelay.onEndEdit.AddListener(delegate { ValidateDelay(); });
    }

    // 이미 같은 index의 창이 열려있다면 false리턴
    public bool Open(Vector2Int selectedIdx, Block selectedBlock)
    {
        return Open(selectedIdx, selectedBlock, false);
    }

    public bool Open(Vector2Int selectedIdx, Block selectedBlock, bool isAutoTrap)
    {
        if (isOpened && currentIdx == selectedIdx)
        {
            Close();
            return false;
        }

        isOpened = true;
        currentIdx = selectedIdx;

        Refresh(selectedIdx, selectedBlock, isAutoTrap);

        gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

        return true;
    }

    public void Refresh(Vector2Int selectedIdx, Block selectedBlock, bool isAutoTrap)
    {
        // targets
        HashSet<Vector2Int> targets = selectedBlock.GetTargets();
        int cnt = 0;
        foreach (var target in targets)
        {
            textTargets[cnt].text = "(" + target.x + ", " + target.y + ")";
            textTargets[cnt].gameObject.SetActive(true);
            cnt++;
        }
        for (; cnt < textTargets.Count; cnt++)
        {
            textTargets[cnt].gameObject.SetActive(false);
        }

        // text empty
        if (targets.Count == 0)
        {
            textEmpty.SetActive(true);
        }
        else
        {
            textEmpty.SetActive(false);
        }

        // text block type
        string spriteName = selectedBlock.GetSprite().name;
        int substringLength = spriteName.IndexOf('_');
        if (substringLength < 0)
        {
            substringLength = spriteName.Length;
        }
        textBlockType.text = spriteName.Substring(0, substringLength);

        // text block idx
        textBlockIdx.text = "(" + selectedIdx.x + ", " + selectedIdx.y + ")";

        // number of target
        if (spriteName.Contains("Button_"))
        {
            textNumberOfTarget.text = "(" + selectedBlock.GetTargets().Count + " / 20)";
        }
        else if (spriteName.Contains("Portal_"))
        {
            textNumberOfTarget.text = "(" + selectedBlock.GetTargets().Count + " / 1)";
        }
        else
        {
            textNumberOfTarget.text = "(0 / 0)";
        }

        // trap info
        if (spriteName.Contains("Trap_"))
        {
            if (isAutoTrap)
            {
                OnToggleAutomatic();

                trapInfoFirstDelay.text = "" + selectedBlock.GetTrapDelay().x;
                trapInfoOddDelay.text = "" + selectedBlock.GetTrapDelay().y;
                trapInfoEvenDelay.text = "" + selectedBlock.GetTrapDelay().z;
                trapInfoFirstDelay.interactable = true;
                trapInfoOddDelay.interactable = true;
                trapInfoEvenDelay.interactable = true;
            }
            else
            {
                OnToggleManual();

                trapInfoFirstDelay.text = "-";
                trapInfoOddDelay.text = "-";
                trapInfoEvenDelay.text = "-";
                trapInfoFirstDelay.interactable = false;
                trapInfoOddDelay.interactable = false;
                trapInfoEvenDelay.interactable = false;
            }

            trapInfo.SetActive(true);
        }
        else
        {
            trapInfo.SetActive(false);
        }


        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    public void Refresh(Vector2Int selectedIdx, Block selectedBlock)
    {
        Refresh(selectedIdx, selectedBlock, false);
    }

    // Trap 추가 선택 시 호출
    public void RefreshTrap(Vector2Int selectedIdx, Block selectedBlock, bool isAutoTrap)
    {
        if (trapInfoToggleAutomatic.isOn && isAutoTrap)
        {
            OnToggleAutomatic();

            trapInfoFirstDelay.text = "-";
            trapInfoOddDelay.text = "-";
            trapInfoEvenDelay.text = "-";
            trapInfoFirstDelay.interactable = true;
            trapInfoOddDelay.interactable = true;
            trapInfoEvenDelay.interactable = true;
        }
        else if (trapInfoToggleManual.isOn && (!isAutoTrap))
        {
            OnToggleManual();

            trapInfoFirstDelay.text = "-";
            trapInfoOddDelay.text = "-";
            trapInfoEvenDelay.text = "-";
            trapInfoFirstDelay.interactable = false;
            trapInfoOddDelay.interactable = false;
            trapInfoEvenDelay.interactable = false;
        }
        else
        {
            OffToggle();

            trapInfoFirstDelay.text = "-";
            trapInfoOddDelay.text = "-";
            trapInfoEvenDelay.text = "-";
            trapInfoFirstDelay.interactable = false;
            trapInfoOddDelay.interactable = false;
            trapInfoEvenDelay.interactable = false;
        }
    }
    
    public void Close()
    {
        isOpened = false;
        gameObject.SetActive(false);
    }

    public void ButtonAutomatic()
    {
        if (trapInfoToggleAutomatic.isOn == true)
        {
            if (isButtonOff)
            {
                isButtonOff = false;
                return;
            }

            trapInfoFirstDelay.interactable = true;
            trapInfoOddDelay.interactable = true;
            trapInfoEvenDelay.interactable = true;

            drawLayer.SetAutoTrap(true);
        }
    }

    public void ButtonManual()
    {
        if (trapInfoToggleManual.isOn == true)
        {
            if (isButtonOff)
            {
                isButtonOff = false;
                return;
            }

            trapInfoFirstDelay.interactable = false;
            trapInfoOddDelay.interactable = false;
            trapInfoEvenDelay.interactable = false;

            drawLayer.SetAutoTrap(false);
        }
    }

    public void OnToggleAutomatic()
    {
        if (trapInfoToggleAutomatic.isOn && !trapInfoToggleManual.isOn)
        {
            return;
        }

        isButtonOff = true;
        trapInfoToggleAutomatic.isOn = true;
        trapInfoToggleManual.isOn = false;
    }
    
    public void OnToggleManual()
    {
        if (!trapInfoToggleAutomatic.isOn && trapInfoToggleManual.isOn)
        {
            return;
        }

        isButtonOff = true;
        trapInfoToggleAutomatic.isOn = false;
        trapInfoToggleManual.isOn = true;
    }

    public void OffToggle()
    {
        trapInfoToggleAutomatic.isOn = false;
        trapInfoToggleManual.isOn = false;
    }

    void ValidateDelay()
    {
        if (trapInfoFirstDelay.text == "-" || trapInfoOddDelay.text == "-" || trapInfoEvenDelay.text == "-")
        {
            return;
        }

        int firstdelay = int.Parse(trapInfoFirstDelay.text);
        int odddelay = int.Parse(trapInfoOddDelay.text);
        int evendelay = int.Parse(trapInfoEvenDelay.text);

        drawLayer.SetDelay(firstdelay, odddelay, evendelay);
        drawLayer.SetAutoTrap(true);
    }
}
