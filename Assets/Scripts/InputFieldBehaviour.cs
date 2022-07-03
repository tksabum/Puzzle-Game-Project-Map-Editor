using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldBehaviour : MonoBehaviour
{
    public int minValue;
    public int maxValue;

    TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        //inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.onEndEdit.AddListener(delegate
        {
            System.Int32.TryParse(inputField.text, out int value);
            ValidateInput(value);
        });
    }

    void ValidateInput(int value)
    {
        if (value > maxValue)
        {
            inputField.text = maxValue.ToString();
        }
        else if (value <= minValue)
        {
            inputField.text = minValue.ToString();
        }
    }
}
