using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Itembase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPrePlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {
        base.OnPrePlayerEnter(gameManager, blockManager, enteridx, itemidx);
    }

    public override void OnPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {
        base.OnPlayerEnter((GameManager)gameManager, blockManager, enteridx, itemidx);
    }

    public override void OnPostPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {
        base.OnPostPlayerEnter((GameManager)gameManager, blockManager, enteridx, itemidx);
    }

    public override void OnPlayerExit()
    {

    }
}
