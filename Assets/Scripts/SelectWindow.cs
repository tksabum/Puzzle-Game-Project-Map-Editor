using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SelectWindow : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text textButton1;
    public TMP_Text textButton2;

    public GameObject button1;
    public GameObject button2;

    Action action1;
    Action action2;

    public void OpenSelectWindow(Action _action1, Action _action2, bool isSelect, string _text, string btn1, string btn2)
    {
        action1 = _action1;
        action2 = _action2;
        if (isSelect)
        {
            button1.SetActive(true);
            button2.SetActive(true);
        }
        else
        {
            button1.SetActive(true);
            button2.SetActive(false);
        }
        text.text = _text;
        textButton1.text = btn1;
        textButton2.text = btn2;
        gameObject.SetActive(true);
    }

    public void ButtonEvent1()
    {
        gameObject.SetActive(false);
        if (action1 != null) action1();
    }

    public void ButtonEvent2()
    {
        gameObject.SetActive(false);
        if (action2 != null) action2();
    }
}
