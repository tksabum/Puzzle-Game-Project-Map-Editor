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

    // 0.125초 마다 발생
    public void Tick()
    {
        // Trap 작동
        for(int i = 0; i < tickTrapList.Count; i++)
        {
            Vector2Int ticktrapidx = tickTrapList[i];
            Trap ticktrap = (Trap)floorList[ticktrapidx.x][ticktrapidx.y];

            if (ticktrap.ToggleOnCurrentTick(tickCount))
            {
                ticktrap.PowerToggle(gameManager);
            }
        }

        tickCount++;
    }

    // 이동 가능 여부 판단
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
            bool pushableItem = nextitem.pushable;
            bool getableItem = nextitem.getable;

            // 획득 가능한 아이템
            if (getableItem)
            {
                return true;
            }
            // 밀 수 있는 아이템
            else if (pushableItem)
            {
                Vector2Int pushidx = nextidx + move;
                // 밀리는 블록이 맵을 벗어나서 밀지 못하는 경우
                if (pushidx.x < 0 || mapwidth <= pushidx.x || pushidx.y < 0 || mapheight <= pushidx.y)
                {
                    return false;
                }
                // 아이템이 밀려날 자리에 다른 아이템이 있는 경우
                else if (itemList[pushidx.x][pushidx.y] != null)
                {
                    return false;
                }
                // 아닌 경우 floor가 아이템이 이동할 수 있는 floor인지 확인
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
            // 그 외의 아이템
            else
            {
                return false;
            }
        }
    }
    
    // ####### 이동시 이벤트는 Exit -> Enter 순서로 처리한다.
    // 이동 전 발생하는 이벤트
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

    // 이동중 이전 블록보다 다음 블록에 더 가까워진 순간 발생하는 이벤트
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

    // 다음 블록으로 완전히 이동 후 발생하는 이벤트
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
        List<Vector2Int> consumerList = powerDic[buttonIdx];
        for (int i = 0; i < consumerList.Count; i++)
        {
            Vector2Int consumerIdx = consumerList[i];
            floorList[consumerIdx.x][consumerIdx.y].PowerToggle(gameManager);
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
            foreach(KeyValuePair<PairInt, ThreeInt> keyValuePair in mapData.trapData)
            {
                Vector2Int key = Vector2Int.zero;
                key.x = keyValuePair.Key.x;
                key.y = keyValuePair.Key.y;

                ThreeInt value = keyValuePair.Value;

                tickTrapList.Add(key);
                ((Trap)floorList[key.x][key.y]).SetWorkOnTick(value);
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
                    floorList[i][j].OnObjectEnter(gameManager, this, item.obj);
                }
            }
        }

        // 플레이어 위치의 floor를 작동시키기 위해 OnObjectEnter 실행
        Vector2Int playeridx = gameManager.GetPlayerIdx();
        floorList[playeridx.x][playeridx.y].OnObjectEnter(gameManager, this, Obj.PLAYER);

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
}
