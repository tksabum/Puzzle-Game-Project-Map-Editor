using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Itembase : MonoBehaviour
{
    [Header("- Core -")]
    public BlockManager.Obj obj;
    public bool getable;
    public bool breakable;
    public bool pushable;

    [Header("- Pushable")]
    public int pushCost;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 플레이어가 올라오면 실행
    public virtual void OnPrePlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {
        if (pushable)
        {
            blockManager.PreItemMoveEvent(this, itemidx, 2 * itemidx - enteridx);
        }
    }

    public virtual void OnPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {
        if (pushable)
        {
            blockManager.ItemMoveEvent(this, itemidx, 2 * itemidx - enteridx);
        }
    }

    public virtual void OnPostPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {
        if (pushable)
        {
            blockManager.PostItemMoveEvent(this, itemidx, 2 * itemidx - enteridx);
        }
    }

    // 플레이어가 나가면 실행
    public abstract void OnPlayerExit();
}
