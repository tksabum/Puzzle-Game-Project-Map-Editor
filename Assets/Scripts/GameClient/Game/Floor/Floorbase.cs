using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorType
{
    PLANE,
    WATER,
    BUTTON,
    TRAP,
    PORTAL,
    GENERATOR
}

public abstract class Floorbase : MonoBehaviour
{
    [Header("- EnterObject -")]
    public bool playerEnterable;
    public bool woodenboxEnterable;
    public bool sturdyboxEnterable;
    public bool steelboxEnterable;

    [Header("- ExitObject -")]
    public bool playerExitable;
    public bool woodenboxExitable;
    public bool sturdyboxExitable;
    public bool steelboxExitable;

    [Header("- EnterDirection -")]
    public bool leftEnterable;
    public bool rightEnterable;
    public bool upEnterable;
    public bool downEnterable;

    [Header("- ExitDirection -")]
    public bool leftExitable;
    public bool rightExitable;
    public bool upExitable;
    public bool downExitable;

    protected Vector2Int idx;
    protected bool power;

    protected void Awake()
    {
        //power = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetIdx(Vector2Int _idx)
    {
        idx = _idx;
    }

    // �� �� �ִ��� �Ǵ�
    public bool Enterable(BlockManager.Obj obj, BlockManager.Direction dir)
    {

        if (obj == BlockManager.Obj.PLAYER)
        {
            if (!playerEnterable) return false;
        }
        else if (obj == BlockManager.Obj.WOODENBOX)
        {
            if (!woodenboxEnterable) return false;
        }
        else if (obj == BlockManager.Obj.STURDYBOX)
        {
            if (!sturdyboxEnterable) return false;
        }
        else if (obj == BlockManager.Obj.STEELBOX)
        {
            if (!steelboxEnterable) return false;
        }
        else
        {
            throw new System.Exception("������ ������Ʈ ã�� ����");
        }

        if (dir == BlockManager.Direction.LEFT) return leftEnterable;
        else if (dir == BlockManager.Direction.RIGHT) return rightEnterable;
        else if (dir == BlockManager.Direction.UP) return upEnterable;
        else if (dir == BlockManager.Direction.DOWN) return downEnterable;
        else
        {
            throw new System.Exception("������ ���� ã�� ����");
        }
    }

    // ���� �� �ִ��� �Ǵ�
    public bool Exitable(BlockManager.Obj obj, BlockManager.Direction dir)
    {
        if (obj == BlockManager.Obj.PLAYER)
        {
            if (!playerExitable) return false;
        }
        else if (obj == BlockManager.Obj.WOODENBOX)
        {
            if (!woodenboxExitable) return false;
        }
        else if (obj == BlockManager.Obj.STURDYBOX)
        {
            if (!sturdyboxExitable) return false;
        }
        else if (obj == BlockManager.Obj.STEELBOX)
        {
            if (!steelboxExitable) return false;
        }
        else
        {
            throw new System.Exception("������ ������Ʈ ã�� ����");
        }

        if (dir == BlockManager.Direction.LEFT) return leftExitable;
        else if (dir == BlockManager.Direction.RIGHT) return rightExitable;
        else if (dir == BlockManager.Direction.UP) return upExitable;
        else if (dir == BlockManager.Direction.DOWN) return downExitable;
        else
        {
            throw new System.Exception("������ ���� ã�� ����");
        }
    }

    // ������Ʈ�� �ö���� ����
    public abstract void OnPreObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);
    public abstract void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);
    public abstract void OnPostObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);

    // ������Ʈ�� ������ ����
    public abstract void OnPreObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);
    public abstract void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);
    public abstract void OnPostObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);

    public abstract void PowerToggle(GameManager gameManager, BlockManager blockManager);
}
