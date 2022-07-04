using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Setting : MonoBehaviour
{
    public EditorManager editorManager;
    public DrawLayer drawLayer;

    public TMP_InputField inputFieldMapDesigner;
    public TMP_InputField inputFieldStartIndexX;
    public TMP_InputField inputFieldStartIndexY;
    public TMP_InputField inputFieldStartLife;
    public TMP_InputField inputFieldMaxLife;

    private void Awake()
    {
        inputFieldMapDesigner.onEndEdit.AddListener(delegate { ValidateMapDesigner(); });
        inputFieldStartIndexX.onEndEdit.AddListener(delegate { ValidateStartIndexX(); });
        inputFieldStartIndexY.onEndEdit.AddListener(delegate { ValidateStartIndexY(); });
        inputFieldStartLife.onEndEdit.AddListener(delegate { ValidateStartLife(); });
        inputFieldMaxLife.onEndEdit.AddListener(delegate { ValidateMaxLife(); });
    }

    void ValidateMapDesigner()
    {

    }

    void ValidateStartIndexX()
    {
        int value = int.Parse(inputFieldStartIndexX.text);

        Vector2Int mapSize = drawLayer.GetMapSize();

        if (value >= mapSize.x)
        {
            inputFieldStartIndexX.text = (mapSize.x - 1).ToString();
        }
        else if (value < 0)
        {
            inputFieldStartIndexX.text = "0";
        }
    }

    void ValidateStartIndexY()
    {
        int value = int.Parse(inputFieldStartIndexY.text);

        Vector2Int mapSize = drawLayer.GetMapSize();

        if (value >= mapSize.y)
        {
            inputFieldStartIndexY.text = (mapSize.y - 1).ToString();
        }
        else if (value < 0)
        {
            inputFieldStartIndexY.text = "0";
        }
    }

    void ValidateStartLife()
    {
        int value = int.Parse(inputFieldStartLife.text);

        int maxlife = int.Parse(inputFieldMaxLife.text);

        if (value > maxlife)
        {
            inputFieldStartLife.text = maxlife.ToString();
        }
        else if (value < 1)
        {
            inputFieldStartLife.text = "1";
        }
    }

    void ValidateMaxLife()
    {
        int value = int.Parse(inputFieldMaxLife.text);

        int startlife = int.Parse(inputFieldStartLife.text);

        if (value > 5)
        {
            inputFieldMaxLife.text = 5.ToString();
        }
        else if (value < 1)
        {
            inputFieldMaxLife.text = "1";
        }

        if (value < startlife)
        {
            inputFieldStartLife.text = value.ToString();
        }
    }

    void OpenSettingWindow()
    {
        inputFieldMapDesigner.text = editorManager.GetMapDesigner();
        inputFieldStartIndexX.text = editorManager.GetStartIdx().x.ToString();
        inputFieldStartIndexY.text = editorManager.GetStartIdx().y.ToString();
        inputFieldStartLife.text = editorManager.GetStartLife().ToString();
        inputFieldMaxLife.text = editorManager.GetMaxLife().ToString();
        gameObject.SetActive(true);
    }

    public void CloseSettingWindow()
    {
        gameObject.SetActive(false);
    }

    public void SaveChangedSetting()
    {
        editorManager.SetMapDesigner(inputFieldMapDesigner.text);
        editorManager.SetStartIdx(new Vector2Int(int.Parse(inputFieldStartIndexX.text), int.Parse(inputFieldStartIndexY.text)));
        editorManager.SetStartLife(int.Parse(inputFieldStartLife.text));
        editorManager.SetMaxLife(int.Parse(inputFieldMaxLife.text));
        CloseSettingWindow();
    }

    public void ButtonSetting()
    {
        if (!gameObject.activeInHierarchy)
        {
            OpenSettingWindow();
        }
        else
        {
            CloseSettingWindow();
        }
    }
}
