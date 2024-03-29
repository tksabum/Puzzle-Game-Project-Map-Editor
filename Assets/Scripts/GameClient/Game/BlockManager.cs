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

    public enum Obj { EMPTY, PLAYER, WOODENBOX, GOAL, HAMMER, LIFE, STURDYBOX, STEELBOX, ROCK };
    public enum Direction { LEFT, RIGHT, UP, DOWN };

    int mapwidth;
    int mapheight;

    MapData mapData;

    int tickCount;

    Itembase moveItem;
    Vector2Int moveItemStartidx;
    Vector2Int moveItemEndidx;
    bool isMove;

    string GetFloorString(Vector3Int vector)
    {
        return mapData.floorData[vector.x][vector.y];
    }

    string GetItemString(Vector3Int vector)
    {
        return mapData.itemData[vector.x][vector.y];
    }

    // 0.125초 마다 발생
    public void Tick()
    {
        // Trap 작동
        for (int i = 0; i < tickTrapList.Count; i++)
        {
            Vector2Int ticktrapidx = tickTrapList[i];
            Trap ticktrap = (Trap)floorList[ticktrapidx.x][ticktrapidx.y];

            if (ticktrap.ToggleOnCurrentTick(tickCount))
            {
                ticktrap.PowerToggle(gameManager, this);
            }
        }

        // Generator 작동
        for (int i = 0; i < tickGeneratorList.Count; i++)
        {
            Vector2Int tickgeneratoridx = tickGeneratorList[i];
            ItemGenerator tickgenerator = (ItemGenerator)floorList[tickgeneratoridx.x][tickgeneratoridx.y];

            tickgenerator.Tick(this, tickCount);
        }

        tickCount++;
    }

    // 이동 가능 여부 판단
    public bool Movable(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        // 이동할 위치가 맵 밖인 경우
        if (nextidx.x < 0 || mapwidth <= nextidx.x || nextidx.y < 0 || mapheight <= nextidx.y)
        {
            return false;
        }

        // 방향계산
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

        // 움직일 오브젝트가 플레이어인 경우
        if (obj == Obj.PLAYER)
        {
            // 이동할 자리에 아이템이 없는 경우
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
            // 이동할 자리에 아이템이 있는 경우
            else
            {
                Vector2Int pushidx = nextidx + move;
                if (nextitem.getable || (nextitem.pushable && Movable(nextitem.obj, nextidx, pushidx)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        // 움직일 오브젝트가 아이템인 경우
        else
        {
            if (itemList[nowidx.x][nowidx.y].pushable && itemList[nowidx.x][nowidx.y].pushCost < gameManager.GetLife())
            {
                if (itemList[nextidx.x][nextidx.y] == null)
                {
                    if (enterableFloor && exitableFloor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    // ####### 이동시 이벤트는 item -> floor 순서로 처리한다.
    // ####### 이동시 이벤트는 Exit -> Enter 순서로 처리한다.
    // 이동 전 발생하는 이벤트
    public void PreMoveEvent(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        // item
        Itembase nextitem = itemList[nextidx.x][nextidx.y];
        if (nextitem != null)
        {
            nextitem.OnPrePlayerEnter(gameManager, this, nowidx, nextidx);
        }

        // floor
        Floorbase nowfloor = floorList[nowidx.x][nowidx.y];
        Floorbase nextfloor = floorList[nextidx.x][nextidx.y];

        nowfloor.OnPreObjectExit(gameManager, this, obj);
        nextfloor.OnPreObjectEnter(gameManager, this, obj);
    }

    // 이동중 이전 블록보다 다음 블록에 더 가까워진 순간 발생하는 이벤트
    public void MoveEvent(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        // item
        Itembase nextitem = itemList[nextidx.x][nextidx.y];
        if (nextitem != null)
        {
            nextitem.OnPlayerEnter(gameManager, this, nowidx, nextidx);
        }

        // floor
        Floorbase nowfloor = floorList[nowidx.x][nowidx.y];
        Floorbase nextfloor = floorList[nextidx.x][nextidx.y];

        nowfloor.OnObjectExit(gameManager, this, obj);
        nextfloor.OnObjectEnter(gameManager, this, obj);
    }

    // 다음 블록으로 완전히 이동 후 발생하는 이벤트
    public void PostMoveEvent(Obj obj, Vector2Int nowidx, Vector2Int nextidx)
    {
        // item
        Itembase nextitem = itemList[nextidx.x][nextidx.y];
        if (nextitem != null)
        {
            nextitem.OnPostPlayerEnter(gameManager, this, nowidx, nextidx);
        }

        // floor
        Floorbase nowfloor = floorList[nowidx.x][nowidx.y];
        Floorbase nextfloor = floorList[nextidx.x][nextidx.y];

        nowfloor.OnPostObjectExit(gameManager, this, obj);
        nextfloor.OnPostObjectEnter(gameManager, this, obj);
    }

    // 아이템 사용 가능 여부 판단
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

    // 각각의 아이템 사용 처리
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
                // 타겟 아이템 제거
                itemList[targetPos.x][targetPos.y] = null;
                objectPool.ReturnObject(targetItem.gameObject);
                // 타겟 바닥의 ExitEvent
                floorList[targetPos.x][targetPos.y].OnObjectExit(gameManager, this, Obj.WOODENBOX);
            }
        }
        else
        {
            throw new System.Exception("Error:use wrong item");
        }
    }

    // 누른 버튼의 좌표를 키값으로 powerDic에서 얻은 좌표의 floor의 전원을 토글함
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
            floorList[consumerIdx.x][consumerIdx.y].PowerToggle(gameManager, this);
        }
    }

    // 포탈의 출구 좌표를 리턴
    public Vector2Int GetPortalExit(Vector2Int entrance)
    {
        return portalDic[entrance];
    }

    // 블록(게임 오브젝트)를 생성하고 게임 진행에 필요한 powerDic, portalDic을 생성
    public void SetBlock(MapData data)
    {
        mapData = data;

        mapwidth = mapData.mapWidth;
        mapheight = mapData.mapHeight;

        SetBlock();
    }

    public void SetBlock()
    {
        // 리스트 생성
        // floorList
        floorList = new List<List<Floorbase>>();

        for (int i = 0; i < mapwidth; i++)
        {
            floorList.Add(new List<Floorbase>());
            for (int j = 0; j < mapheight; j++)
            {
                // 게임오브젝트 생성 & List에 생성된 오브젝트의 floorbase 추가
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


        // 아이템이 올려져 있는 버튼과 같은 floor들을 작동시키기 위해 OnObjectEnter 실행
        for (int i = 0; i < mapwidth; i++)
        {
            for (int j = 0; j < mapheight; j++)
            {
                Itembase item = itemList[i][j];
                if (item != null)
                {
                    floorList[i][j].OnPreObjectEnter(gameManager, this, item.obj);
                    floorList[i][j].OnObjectEnter(gameManager, this, item.obj);
                    floorList[i][j].OnPostObjectEnter(gameManager, this, item.obj);
                }
            }
        }

        // 플레이어 위치의 floor를 작동시키기 위해 OnObjectEnter 실행
        Vector2Int playeridx = gameManager.GetPlayerIdx();
        floorList[playeridx.x][playeridx.y].OnPreObjectEnter(gameManager, this, Obj.PLAYER);
        floorList[playeridx.x][playeridx.y].OnObjectEnter(gameManager, this, Obj.PLAYER);
        floorList[playeridx.x][playeridx.y].OnPostObjectEnter(gameManager, this, Obj.PLAYER);

        tickCount = 0;
    }

    // 동일한 맵으로 초기화
    public void ResetBlock()
    {
        RemoveBlock();

        SetBlock();
    }

    // 모든 블록(게임 오브젝트) 제거
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
        else if (obj == Obj.STURDYBOX)
        {
            objName = "SturdyBox";
        }
        else if (obj == Obj.STEELBOX)
        {
            objName = "SteelBox";
        }

        if (objName == "")
        {
            throw new System.Exception("Error: unknown objName " + obj);
        }
        else
        {
            // 아이템이 생성될 위치가 비어있는 경우에만 생성
            if (genIdx != gameManager.GetPlayerIdx() && genIdx != gameManager.GetPlayerNextIdx() && itemList[genIdx.x][genIdx.y] == null && (!isMove || moveItemEndidx != genIdx))
            {
                // 아이템 생성
                GameObject instant = objectPool.GetObject(objName);
                instant.transform.position = (Vector2)genIdx;
                itemList[genIdx.x][genIdx.y] = instant.GetComponent<Itembase>();

                // 아이템이 생성된 위치의 바닥에 EnterEvent 발생
                floorList[genIdx.x][genIdx.y].OnPreObjectEnter(gameManager, this, obj);
                floorList[genIdx.x][genIdx.y].OnObjectEnter(gameManager, this, obj);
                floorList[genIdx.x][genIdx.y].OnPostObjectEnter(gameManager, this, obj);
            }
        }
    }

    public void PreItemMoveEvent(Itembase item, Vector2Int nowidx, Vector2Int nextidx)
    {
        moveItem = item;
        moveItemStartidx = nowidx;
        moveItemEndidx = nextidx;
        isMove = true;

        floorList[nowidx.x][nowidx.y].OnPreObjectExit(gameManager, this, item.obj);
        floorList[nextidx.x][nextidx.y].OnPreObjectEnter(gameManager, this, item.obj);
    }

    public void ItemMoveEvent(Itembase item, Vector2Int nowidx, Vector2Int nextidx)
    {
        floorList[nowidx.x][nowidx.y].OnObjectExit(gameManager, this, item.obj);
        floorList[nextidx.x][nextidx.y].OnObjectEnter(gameManager, this, item.obj);
    }

    public void PostItemMoveEvent(Itembase item, Vector2Int nowidx, Vector2Int nextidx)
    {
        floorList[nowidx.x][nowidx.y].OnPostObjectExit(gameManager, this, item.obj);
        floorList[nextidx.x][nextidx.y].OnPostObjectEnter(gameManager, this, item.obj);
    }

    public void ItemMoveUpdate(Vector2 itemPos, bool isChangedidx)
    {
        if (isMove)
        {
            moveItem.transform.position = itemPos;

            if (isChangedidx)
            {
                itemList[moveItemEndidx.x][moveItemEndidx.y] = moveItem;
                itemList[moveItemStartidx.x][moveItemStartidx.y] = null;

                gameManager.AttackedPlayer(moveItem.pushCost);
            }

            if (itemPos == (Vector2)moveItemEndidx)
            {
                isMove = false;
            }
        }
    }

    public void RemoveItem(Itembase item, Vector2Int idx)
    {
        itemList[idx.x][idx.y] = null;

        // 아이템이 놓여있던 바닥의 ExitEvent
        floorList[idx.x][idx.y].OnPreObjectExit(gameManager, this, item.obj);
        floorList[idx.x][idx.y].OnObjectExit(gameManager, this, item.obj);
        floorList[idx.x][idx.y].OnPostObjectExit(gameManager, this, item.obj);

        objectPool.ReturnObject(item.gameObject);
    }

    public Direction VtoDir(Vector2Int nowidx, Vector2Int nextidx)
    {
        if (nextidx - nowidx == Vector2Int.left) return Direction.LEFT;
        else if (nextidx - nowidx == Vector2Int.right) return Direction.RIGHT;
        else if (nextidx - nowidx == Vector2Int.up) return Direction.UP;
        else if (nextidx - nowidx == Vector2Int.down) return Direction.DOWN;
        else throw new System.Exception("Error: wrong Direction");
    }
}
