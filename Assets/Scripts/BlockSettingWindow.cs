using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockSettingWindow : MonoBehaviour
{
    public TMP_Text textBlockType;
    public TMP_Text textBlockIdx;
    public TMP_Text textNumberOfTarget;

    public GameObject textEmpty;
    public List<TMP_Text> textTargets;

    bool isOpened;
    Vector2Int currentIdx;

    private void Awake()
    {
        isOpened = false;
        currentIdx = Vector2Int.zero;
    }

    public bool Open(Vector2Int selectedIdx, Block selectedBlock)
    {
        if (isOpened && currentIdx == selectedIdx)
        {
            Close();
            return false;
        }

        isOpened = true;
        currentIdx = selectedIdx;

        Refresh(selectedIdx, selectedBlock);

        gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

        return true;
    }

    public void Refresh(Vector2Int selectedIdx, Block selectedBlock)
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

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
    
    public void Close()
    {
        isOpened = false;
        gameObject.SetActive(false);
    }
}
