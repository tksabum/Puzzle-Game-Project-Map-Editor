using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockSettingWindow : MonoBehaviour
{
    public TMP_Text textBlockType;
    public TMP_Text textBlockIdx;

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

        HashSet<Vector2Int>targets = selectedBlock.GetTargets();
        int cnt = 0;
        foreach(var target in targets)
        {
            textTargets[cnt].text = "(" + target.x + ", " + target.y + ")";
            textTargets[cnt].gameObject.SetActive(true);
            cnt++;
        }
        for (; cnt < textTargets.Count; cnt++)
        {
            textTargets[cnt].gameObject.SetActive(false);
        }

        if (targets.Count == 0)
        {
            textEmpty.SetActive(true);
        }
        else
        {
            textEmpty.SetActive(false);
        }


        isOpened = true;
        currentIdx = selectedIdx;

        textBlockIdx.text = "(" + selectedIdx.x + ", " + selectedIdx.y + ")";
        gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

        return true;
    }
    
    public void Close()
    {
        isOpened = false;
        gameObject.SetActive(false);
    }
}
