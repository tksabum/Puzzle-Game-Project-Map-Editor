using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockManager : MonoBehaviour
{
    List<List<Floorbase>> floorList;
    List<List<Itembase>> itemList;

    [Header("- Core -")]
    public ObjectPool objectPool;
    public GameManager gameManager;

    Dictionary<Vector2Int, List<Vector2Int>> powerDic;
    Dictionary<Vector2Int, Vector2Int> portalDic;
    List<Vector2Int> tickTrapList;
    List<Vector2Int> tickGeneratorList;

    public enum Obj { EMPTY, PLAYER, WOODENBOX, GOAL, HAMMER, LIFE };
    public enum Direction { LEFT, RIGHT, UP, DOWN };

    int mapwidth;
    int mapheight;

    MapData mapData;

    int tickCount;

    string GetFloorString(Vector3Int vector)
    {
        return mapData.floorData[vector.x][vector.y];
    }

    string GetItemString(Vector3Int vector)
    {
        return mapData.itemData[vector.x][vector.y];
    }

    // 0.125�� ���� �߻�
    public void Tick()
    {
        // Trap �۵�
        for (int i = 0; i < tickTrapList.Count; i++)
        {
            Vector2Int ticktrapidx = tickTrapList[i];
            Trap ticktrap = (Trap)floorList[ticktrapidx.x][ticktrapidx.y];

            if (ticktrap.ToggleOnCurrentTick(tickCount))
            {
                ticktrap.PowerToggle(gameManager);
            }
        }

        // Generator �۵�
        for (int i = 0; i < tickGeneratorList.Count; i++)
        {
            Vector2Int tickgeneratoridx = tickGeneratorList[i];
            ItemGenerator tickgenerator = (ItemGenerator)floorList[tickgeneratoridx.x][tickgeneratoridx.y];

            tickgenerator.Tick(this, tickCount);
        }

        tickCount++;
    }

    // �̵� ���� ���� �Ǵ�
    public bool Movable(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        if (nextidx.x < 0 || mapwidth <= nextidx.x || nextidx.y < 0 || mapheight <= nextidx.y)
        {
            return false;
        }

        Vector2Int move = nextidx - nowidx;
        Direction enterDir = Direction.LEFT;
        Direction exitDir = Direction.RIGHT;

        if (move == Vector2Int.left)
        {
            enterDir = Direction.RIGHT;
            exitDir = Direction.LEFT;
        }
        else if (move == Vector2Int.right)
        {
            enterDir = Direction.LEFT;
            exitDir = Direction.RIGHT;
        }
        else if (move == Vector2Int.up)
        {
            enterDir = Direction.DOWN;
            exitDir = Direction.UP;
        }
        else if (move == Vector2Int.down)
        {
            enterDir = Direction.UP;
            exitDir = Direction.DOWN;
        }

        bool enterableFloor = floorList[nextidx.x][nextidx.y].Enterable(obj, enterDir);
        bool exitableFloor = floorList[nowidx.x][nowidx.y].Exitable(obj, exitDir);

        Itembase nextitem = itemList[nextidx.x][nextidx.y];
        // �̵��� �ڸ��� �������� ���� ���
        if (nextitem == null)
        {
            if (enterableFloor && exitableFloor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // �̵��� �ڸ��� �������� �ִ� ���
        else
        {
            bool pushableItem = nextitem.pushable;
            bool getableItem = nextitem.getable;

            // ȹ�� ������ ������
            if (getableItem)
            {
                return true;
            }
            // �� �� �ִ� ������
            else if (pushableItem)
            {
                Vector2Int pushidx = nextidx + move;
                // �и��� ����� ���� ����� ���� ���ϴ� ���
                if (pushidx.x < 0 || mapwidth <= pushidx.x || pushidx.y < 0 || mapheight <= pushidx.y)
                {
                    return false;
                }
                // �������� �з��� �ڸ��� �ٸ� �������� �ִ� ���
                else if (itemList[pushidx.x][pushidx.y] != null)
                {
                    return false;
                }
                // �ƴ� ��� floor�� �������� �̵��� �� �ִ� floor���� Ȯ��
                else
                {
                    bool enterableItem = floorList[pushidx.x][pushidx.y].Enterable(nextitem.obj, enterDir);
                    bool exitableItem = floorList[nextidx.x][nextidx.y].Exitable(nextitem.obj, exitDir);

                    if (enterableItem && exitableItem)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            // �� ���� ������
            else
            {
                return false;
            }
        }
    }

    // ####### �̵��� �̺�Ʈ�� Exit -> Enter ������ ó���Ѵ�.
    // �̵� �� �߻��ϴ� �̺�Ʈ
    public void PreMoveEvent(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        // item
        Itembase nextitem = itemList[nextidx.x][nextidx.y];
        if (nextitem != null)
        {
            if (nextitem.pushable)
            {
                Vector2Int pushidx = 2 * nextidx - nowidx;
                itemList[pushidx.x][pushidx.y] = nextitem;
                itemList[nextidx.x][nextidx.y] = null;
                nextitem.transform.position = new Vector3Int(pushidx.x, pushidx.y, 0);
                floorList[nextidx.x][nextidx.y].OnObjectExit(gameManager, this, nextitem.obj);
                floorList[pushidx.x][pushidx.y].OnObjectEnter(gameManager, this, nextitem.obj);
            }
        }

        // floor
        Floorbase nowfloor = floorList[nowidx.x][nowidx.y];
        Floorbase nextfloor = floorList[nextidx.x][nextidx.y];

        if (nowfloor.GetOccurInPreEvent())
        {
            nowfloor.OnObjectExit(gameManager, this, obj);
        }

        if (nextfloor.GetOccurInPreEvent())
        {
            nextfloor.OnObjectEnter(gameManager, this, obj);
        }
    }

    // �̵��� ���� ��Ϻ��� ���� ��Ͽ� �� ������� ���� �߻��ϴ� �̺�Ʈ
    public void MoveEvent(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        // floor
        Floorbase nowfloor = floorList[nowidx.x][nowidx.y];
        Floorbase nextfloor = floorList[nextidx.x][nextidx.y];

        if (nowfloor.GetOccurInEvent())
        {
            nowfloor.OnObjectExit(gameManager, this, obj);
        }

        if (nextfloor.GetOccurInEvent())
        {
            nextfloor.OnObjectEnter(gameManager, this, obj);
        }
    }

    // ���� ������� ������ �̵� �� �߻��ϴ� �̺�Ʈ
    public void PostMoveEvent(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        // item
        Itembase nextitem = itemList[nextidx.x][nextidx.y];
        if (nextitem != null)
        {
            if (nextitem.getable)
            {
                nextitem.OnPlayerEnter(gameManager);
                itemList[nextidx.x][nextidx.y] = null;
                objectPool.ReturnObject(nextitem.gameObject);
            }
        }

        // floor
        Floorbase nowfloor = floorList[nowidx.x][nowidx.y];
        Floorbase nextfloor = floorList[nextidx.x][nextidx.y];

        if (nowfloor.GetOccurInPostEvent())
        {
            nowfloor.OnObjectExit(gameManager, this, obj);
        }

        if (nextfloor.GetOccurInPostEvent())
        {
            nextfloor.OnObjectEnter(gameManager, this, obj);
        }
    }

    // ������ ��� ���� ���� �Ǵ�
    public bool ItemUsable(Obj obj, Vector2Int playerPos, Direction direction)
    {
        Vector2Int vecDir;
        if (direction == Direction.LEFT) vecDir = Vector2Int.left;
        else if (direction == Direction.RIGHT) vecDir = Vector2Int.right;
        else if (direction == Direction.UP) vecDir = Vector2Int.up;
        else if (direction == Direction.DOWN) vecDir = Vector2Int.down;
        else throw new System.Exception("Error:wrong direction");

        Vector2Int targetPos = playerPos + vecDir;

        if (obj == Obj.HAMMER)
        {
            Itembase targetItem = itemList[targetPos.x][targetPos.y];
            if (targetItem != null && targetItem.obj == Obj.WOODENBOX)
            {
                return true;
            }
        }
        else
        {
            throw new System.Exception("Error:use wrong item");
        }

        return false;
    }

    // ������ ������ ��� ó��
    public void ItemUseEvent(Obj obj, Vector2Int playerPos, Direction direction)
    {
        Vector2Int vecDir;
        if (direction == Direction.LEFT) vecDir = Vector2Int.left;
        else if (direction == Direction.RIGHT) vecDir = Vector2Int.right;
        else if (direction == Direction.UP) vecDir = Vector2Int.up;
        else if (direction == Direction.DOWN) vecDir = Vector2Int.down;
        else throw new System.Exception("Error:wrong direction");

        Vector2Int targetPos = playerPos + vecDir;

        if (obj == Obj.HAMMER)
        {
            Itembase targetItem = itemList[targetPos.x][targetPos.y];
            if (targetItem.obj == Obj.WOODENBOX)
            {
                // Ÿ�� ������ ����
                itemList[targetPos.x][targetPos.y] = null;
                objectPool.ReturnObject(targetItem.gameObject);
                // Ÿ�� �ٴ��� ExitEvent
                floorList[targetPos.x][targetPos.y].OnObjectExit(gameManager, this, Obj.WOODENBOX);
            }
        }
        else
        {
            throw new System.Exception("Error:use wrong item");
        }
    }

    // ���� ��ư�� ��ǥ�� Ű������ powerDic���� ���� ��ǥ�� floor�� ������ �����
    public void PowerToggle(Vector2Int buttonIdx)
    {
        if (!powerDic.ContainsKey(buttonIdx))
        {
            return;
        }

        List<Vector2Int> consumerList = powerDic[buttonIdx];
        for (int i = 0; i < consumerList.Count; i++)
        {
            Vector2Int consumerIdx = consumerList[i];
            floorList[consumerIdx.x][consumerIdx.y].PowerToggle(gameManager);
        }
    }

    // ��Ż�� �ⱸ ��ǥ�� ����
    public Vector2Int GetPortalExit(Vector2Int entrance)
    {
        return portalDic[entrance];
    }

    // ���(���� ������Ʈ)�� �����ϰ� ���� ���࿡ �ʿ��� powerDic, portalDic�� ����
    public void SetBlock(MapData data)
    {
        mapData = data;

        mapwidth = mapData.mapWidth;
        mapheight = mapData.mapHeight;

        SetBlock();
    }

    public void SetBlock()
    {
        // ����Ʈ ����
        // floorList
        floorList = new List<List<Floorbase>>();

        for (int i = 0; i < mapwidth; i++)
        {
            floorList.Add(new List<Floorbase>());
            for (int j = 0; j < mapheight; j++)
            {
                // ���ӿ�����Ʈ ���� & List�� ������ ������Ʈ�� floorbase �߰�
                Vector3Int pos = new Vector3Int(i, j, 0);
                GameObject instant = objectPool.GetObject(GetFloorString(pos));
                instant.transform.position = pos;
                Floorbase floorInst = instant.GetComponent<Floorbase>();
                floorInst.SetIdx(new Vector2Int(i, j));
                floorList[i].Add(floorInst);
            }
        }

        // itemList
        itemList = new List<List<Itembase>>();

        for (int i = 0; i < mapwidth; i++)
        {
            itemList.Add(new List<Itembase>());
            for (int j = 0; j < mapheight; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                string itemname = GetItemString(pos);
                GameObject instant = null;
                if (itemname != "")
                {
                    instant = objectPool.GetObject(itemname);
                    instant.transform.position = pos;
                }
                Itembase item = null;
                if (instant != null) item = instant.GetComponent<Itembase>();
                itemList[i].Add(item);
            }
        }

        powerDic = new Dictionary<Vector2Int, List<Vector2Int>>();
        foreach (KeyValuePair<PairInt, List<PairInt>> keyValuePair in mapData.powerData)
        {
            PairInt key = keyValuePair.Key;
            List<PairInt> value = keyValuePair.Value;
            List<Vector2Int> list = new List<Vector2Int>();
            foreach (PairInt pairInt in value)
            {
                list.Add(new Vector2Int(pairInt.x, pairInt.y));
            }

            powerDic.Add(new Vector2Int(key.x, key.y), list);
        }

        portalDic = new Dictionary<Vector2Int, Vector2Int>();
        if (mapData.portalData != null)
        {
            foreach (KeyValuePair<PairInt, PairInt> keyValuePair in mapData.portalData)
            {
                PairInt key = keyValuePair.Key;
                PairInt value = keyValuePair.Value;

                portalDic.Add(new Vector2Int(key.x, key.y), new Vector2Int(value.x, value.y));
            }
        }

        tickTrapList = new List<Vector2Int>();
        if (mapData.trapData != null)
        {
            foreach (KeyValuePair<PairInt, ThreeInt> keyValuePair in mapData.trapData)
            {
                Vector2Int key = Vector2Int.zero;
                key.x = keyValuePair.Key.x;
                key.y = keyValuePair.Key.y;

                ThreeInt value = keyValuePair.Value;

                tickTrapList.Add(key);
                ((Trap)floorList[key.x][key.y]).SetWorkOnTick(value);
            }
        }

        tickGeneratorList = new List<Vector2Int>();
        for (int i = 0; i < mapwidth; i++)
        {
            for (int j = 0; j < mapheight; j++)
            {
                if (mapData.floorData[i][j].Contains("ItemGenerator_"))
                {
                    tickGeneratorList.Add(new Vector2Int(i, j));
                }
            }
        }


        // �������� �÷��� �ִ� ��ư�� ���� floor���� �۵���Ű�� ���� OnObjectEnter ����
        for (int i = 0; i < mapwidth; i++)
        {
            for (int j = 0; j < mapheight; j++)
            {
                Itembase item = itemList[i][j];
                if (item != null)
                {
                    floorList[i][j].OnObjectEnter(gameManager, this, item.obj);
                }
            }
        }

        // �÷��̾� ��ġ�� floor�� �۵���Ű�� ���� OnObjectEnter ����
        Vector2Int playeridx = gameManager.GetPlayerIdx();
        floorList[playeridx.x][playeridx.y].OnObjectEnter(gameManager, this, Obj.PLAYER);

        tickCount = 0;
    }

    // ������ ������ �ʱ�ȭ
    public void ResetBlock()
    {
        RemoveBlock();

        SetBlock();
    }

    // ��� ���(���� ������Ʈ) ����
    public void RemoveBlock()
    {
        for (int i = 0; i < mapwidth; i++)
        {
            for (int j = 0; j < mapheight; j++)
            {
                objectPool.ReturnObject(floorList[i][j].gameObject);
                if (itemList[i][j] != null)
                {
                    objectPool.ReturnObject(itemList[i][j].gameObject);
                }
            }
        }
    }

    public void GenerateItem(Obj obj, Vector2Int genIdx)
    {
        string objName = "";
        if (obj == Obj.WOODENBOX)
        {
            objName = "WoodenBox";
        }
        else if (obj == Obj.HAMMER)
        {
            objName = "Hammer";
        }
        else if (obj == Obj.LIFE)
        {
            objName = "Life";
        }


        if (objName == "")
        {
            throw new System.Exception("Error: unknown objName " + obj);
        }
        else
        {
            // �������� ������ ��ġ�� ����ִ� ��쿡�� ����
            if (genIdx != gameManager.GetPlayerIdx() && genIdx != gameManager.GetPlayerNextIdx() && itemList[genIdx.x][genIdx.y] == null)
            {
                GameObject instant = objectPool.GetObject(objName);
                instant.transform.position = (Vector2)genIdx;
                itemList[genIdx.x][genIdx.y] = instant.GetComponent<Itembase>();
            }
        }
    }
}
