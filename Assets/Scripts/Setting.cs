using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Setting : MonoBehaviour
{
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

    }

    void ValidateStartIndexY()
    {

    }

    void ValidateStartLife()
    {
        int value = int.Parse(inputFieldStartLife.text);

        if (value > 5)
        {
            inputFieldStartLife.text = 5.ToString();
        }
        else if (value <= 1)
        {
            inputFieldStartLife.text = 1.ToString();
        }
    }

    void ValidateMaxLife()
    {
        int value = int.Parse(inputFieldMaxLife.text);

        if (value > 5)
        {
            inputFieldMaxLife.text = 5.ToString();
        }
        else if (value <= 1)
        {
            inputFieldMaxLife.text = 1.ToString();
        }
    }
}
