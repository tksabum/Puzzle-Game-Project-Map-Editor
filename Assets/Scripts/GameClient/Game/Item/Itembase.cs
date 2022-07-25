using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Itembase : MonoBehaviour
{
    public BlockManager.Obj obj;
    public bool getable;
    public bool breakable;
    public bool pushable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 플레이어가 올라오면 실행
    public abstract void OnPlayerEnter(GameManager gameManager);

    // 플레이어가 나가면 실행
    public abstract void OnPlayerExit();
}
