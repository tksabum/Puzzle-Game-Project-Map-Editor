using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBus : MonoBehaviour
{
    //---------------------------------------------------
    // MapName

    bool writeLock;
    string str;

    public void WriteMapName(string mapName)
    {
        if (writeLock)
        {
            throw new System.Exception("Write Lock");
        }
        str = mapName;
        writeLock = true;
    }

    public string ReadMapName()
    {
        writeLock = false;
        return str;
    }

    //---------------------------------------------------
    // StartState

    State startState;

    public void WriteStartState(State state)
    {
        startState = state;
    }

    public State ReadStartState()
    {
        return startState;
    }


    //---------------------------------------------------
    // Singleton

    private static DataBus instance;
    public static DataBus Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<DataBus>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newDataBus = new GameObject("Data Bus").AddComponent<DataBus>();
                    instance = newDataBus;
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    void Awake()
    {
        var objs = FindObjectsOfType<DataBus>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        writeLock = false;
        str = null;
        startState = State.TITLE;
    }
}
