using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomClass;

public class DrawLayer : MonoBehaviour
{
    [Header("- Core -")]
    public Pallet pallet;
    public EditorManager editorManager;

    [Header("- Layer -")]
    public GameObject floorLayer;
    public GameObject itemLayer;
    public GameObject triggerLayer;
    public GameObject highlightLayer;

    [Header("- UI -")]
    public Toggle floorLayerSetting;
    public Toggle itemLayerSetting;

    public BlockSettingWindow blockSettingWindow;

    [Header("- Block -")]
    public GameObject floorBlock;
    public GameObject itemBlock;
    public GameObject triggerBlock;
    public GameObject highlightBlock;

    List<List<GameObject>> floorBlocks;
    List<List<GameObject>> itemBlocks;
    List<List<GameObject>> triggerBlocks;
    List<List<GameObject>> highlightBlocks;

    HashSet<Vector2Int> autoTrapSet;

    int mapWidth;
    int mapHeight;

    bool isOpenedBlockSetting;
    Block currentSelectedBlock;
    Vector2Int currentSelectedIdx;

    HighLightBlock.Color startIdxColor = HighLightBlock.Color.RED;

    HighLightBlock.Color emptyColor = HighLightBlock.Color.EMPTY;
    HighLightBlock.Color providerColor = HighLightBlock.Color.GREEN;
    HighLightBlock.Color targetColor = HighLightBlock.Color.BLUE;

    HighLightBlock.Color mainTrapColor = HighLightBlock.Color.GREEN;
    HighLightBlock.Color sameGroupColor = HighLightBlock.Color.YELLOW;
    HighLightBlock.Color differentGroupColor = HighLightBlock.Color.BLUE;

    bool isMouseEntered = false;
    Vector2Int mouseLastEnteredIdx;

    DisjointSet<Vector2Int> trapGroup;
    DisjointSet<Vector2Int> selectedTrap;

    private void Awake()
    {
        floorBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            floorBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                floorBlocks[i].Add(null);
            }
        }


        itemBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            itemBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                itemBlocks[i].Add(null);
            }
        }


        triggerBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            triggerBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                triggerBlocks[i].Add(null);
            }
        }

        highlightBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            highlightBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                highlightBlocks[i].Add(null);
            }
        }

        autoTrapSet = new HashSet<Vector2Int>();
        trapGroup = new DisjointSet<Vector2Int>();
    }

    public void Init()
    {
        CloseBlockSettingWindow();
    }

    public void InitBlocks()
    {
        autoTrapSet = new HashSet<Vector2Int>();
        trapGroup = new DisjointSet<Vector2Int>();

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                floorBlocks[i][j].GetComponent<Block>().Clear();
                itemBlocks[i][j].GetComponent<Block>().Clear();
            }
        }
    }

    public void InitBlocks(MapData mapData)
    {
        Dictionary<string, Sprite> floorDic = new Dictionary<string, Sprite>();
        Dictionary<string, Sprite> itemDic = new Dictionary<string, Sprite>();

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                floorBlocks[i][j].GetComponent<Block>().Clear();
            }
        }

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                string floorName = mapData.floorData[i][j];
                Sprite floorSprite = null;
                if (floorDic.ContainsKey(floorName))
                {
                    floorSprite = floorDic[floorName];
                }
                else
                {
                    floorSprite = pallet.GetFloorSprite(floorName);
                    floorDic[floorName] = floorSprite;
                }

                floorBlocks[i][j].GetComponent<Block>().PaintOver(floorSprite);


                string itemName = mapData.itemData[i][j];
                Sprite itemSprite = null;
                if (itemName != "")
                {
                    if (itemDic.ContainsKey(itemName))
                    {
                        itemSprite = itemDic[itemName];
                    }
                    else
                    {
                        itemSprite = pallet.GetItemSprite(itemName);
                        itemDic[itemName] = itemSprite;
                    }
                }

                itemBlocks[i][j].GetComponent<Block>().PaintOver(itemSprite);
            }
        }

        foreach (KeyValuePair<PairInt, List<PairInt>> keyValuePair in mapData.powerData)
        {
            PairInt pairInt = keyValuePair.Key;
            List<PairInt> list = keyValuePair.Value;
            Vector2Int provider = new Vector2Int(pairInt.x, pairInt.y);

            foreach(PairInt pair in list)
            {
                Vector2Int target = new Vector2Int(pair.x, pair.y);
                floorBlocks[provider.x][provider.y].GetComponent<Block>().AddTarget(target);
                floorBlocks[target.x][target.y].GetComponent<Block>().AddProvider(provider);
            }
        }

        foreach (KeyValuePair<PairInt, PairInt> keyValuePair in mapData.portalData)
        {
            PairInt key = keyValuePair.Key;
            PairInt value = keyValuePair.Value;
            Vector2Int provider = new Vector2Int(key.x, key.y);
            Vector2Int target = new Vector2Int(value.x, value.y);

            floorBlocks[provider.x][provider.y].GetComponent<Block>().AddTarget(target);
            floorBlocks[target.x][target.y].GetComponent<Block>().AddProvider(provider);
        }

        autoTrapSet = new HashSet<Vector2Int>();

        if (mapData.trapData != null)
        {
            foreach (KeyValuePair<PairInt, ThreeInt> keyValuePair in mapData.trapData)
            {
                PairInt key = keyValuePair.Key;
                ThreeInt value = keyValuePair.Value;

                autoTrapSet.Add(new Vector2Int(key.x, key.y));

                floorBlocks[key.x][key.y].GetComponent<Block>().SetTrapDelay(value);
            }
        }

        trapGroup = new DisjointSet<Vector2Int>();
        Dictionary<KeyValuePair<bool, Vector3Int>, Vector2Int> dic = new Dictionary<KeyValuePair<bool, Vector3Int>, Vector2Int>();
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                Block block = floorBlocks[i][j].GetComponent<Block>();

                if (block.GetSprite().name.Contains("Trap_"))
                {
                    trapGroup.AddElement(new Vector2Int(i, j));

                    ThreeInt trapdelayThreeInt = floorBlocks[i][j].GetComponent<Block>().GetTrapDelay();
                    Vector3Int trapdelay = new Vector3Int(trapdelayThreeInt.x, trapdelayThreeInt.y, trapdelayThreeInt.z);
                    bool iscontain = autoTrapSet.Contains(new Vector2Int(i, j));
                    if (dic.ContainsKey(new KeyValuePair<bool, Vector3Int>(iscontain, trapdelay)))
                    {
                        trapGroup.Union(dic[new KeyValuePair<bool, Vector3Int>(iscontain, trapdelay)], new Vector2Int(i, j));
                    }
                    else
                    {
                        dic.Add(new KeyValuePair<bool, Vector3Int>(iscontain, trapdelay), new Vector2Int(i, j));
                    }
                }
            }
        }

        RefreshHighLight();
    }

    public void Resize(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;

        for (int i = 0; i < 99; i++)
        {
            for (int j = 0; j < 99; j++)
            {
                if (i < width && j < height)
                {
                    // Block이 없을 경우 생성
                    if (floorBlocks[i][j] == null)
                    {
                        GameObject inst = Instantiate(floorBlock, new Vector3(i, j, 0), Quaternion.identity, floorLayer.transform);
                        inst.GetComponent<Block>().SetIdx(new Vector2Int(i, j));
                        floorBlocks[i][j] = inst;
                    }

                    if (itemBlocks[i][j] == null)
                    {
                        itemBlocks[i][j] = Instantiate(itemBlock, new Vector3(i, j, 0), Quaternion.identity, itemLayer.transform);
                    }

                    if (triggerBlocks[i][j] == null)
                    {
                        GameObject inst = Instantiate(triggerBlock, new Vector3(i, j, 0), Quaternion.identity, triggerLayer.transform);
                        inst.GetComponent<TriggerBlock>().SetIdx(new Vector2Int(i, j));
                        triggerBlocks[i][j] = inst;
                    }

                    if (highlightBlocks[i][j] == null)
                    {
                        highlightBlocks[i][j] = Instantiate(highlightBlock, new Vector3(i, j, 0), Quaternion.identity, highlightLayer.transform);
                    }

                    floorBlocks[i][j].SetActive(true);
                    itemBlocks[i][j].SetActive(true);
                    triggerBlocks[i][j].SetActive(true);
                    highlightBlocks[i][j].SetActive(true);
                }
                else
                {
                    // Block이 없을 경우 무시
                    if (floorBlocks[i][j] != null) floorBlocks[i][j].SetActive(false);
                    if (itemBlocks[i][j] != null) itemBlocks[i][j].SetActive(false);
                    if (triggerBlocks[i][j] != null) triggerBlocks[i][j].SetActive(false);
                    if (highlightBlocks[i][j] != null) highlightBlocks[i][j].SetActive(false);
                }
            }
        }
    }

    public void ActiveFloorLayer()
    {
        floorLayer.SetActive(floorLayerSetting.isOn);
        if (!floorLayer.activeInHierarchy)
        {
            CloseBlockSettingWindow();
        }
    }

    public void ActiveItemLayer()
    {
        itemLayer.SetActive(itemLayerSetting.isOn);
    }

    void Paint(Vector2Int idx)
    {
        if (pallet.GetSelectedBlockType() == BlockType.FLOOR)
        {
            Sprite selectedSprite = pallet.GetSelectedSprite();
            if (selectedSprite != pallet.spriteEmpty)
            {
                floorBlocks[idx.x][idx.y].GetComponent<Block>().Paint(selectedSprite);
            }
        }
        else if (pallet.GetSelectedBlockType() == BlockType.ITEM)
        {
            Sprite selectedSprite = pallet.GetSelectedSprite();
            if (selectedSprite != pallet.spriteEmpty)
            {
                itemBlocks[idx.x][idx.y].GetComponent<Block>().Paint(selectedSprite);
            }
        }
    }

    void PaintOver(Vector2Int idx)
    {
        if (pallet.GetSelectedBlockType() == BlockType.FLOOR)
        {
            Sprite selectedSprite = pallet.GetSelectedSprite();
            if (selectedSprite != pallet.spriteEmpty)
            {
                Block block = floorBlocks[idx.x][idx.y].GetComponent<Block>();
                bool isOver = block.PaintOver(selectedSprite);
                
                // 이전과 다른 sprite로 변경된 경우
                if (isOver)
                {
                    //trap관련 데이터 변경
                    if (selectedSprite.name.Contains("Trap_"))
                    {
                        if (!trapGroup.ContainsKey(idx))
                        {
                            trapGroup.AddElement(idx);
                        }
                    }
                    else
                    {
                        if (trapGroup.ContainsKey(idx))
                        {
                            trapGroup.RemoveElement(idx);
                        }

                        if (autoTrapSet.Contains(idx))
                        {
                            autoTrapSet.Remove(idx);
                        }
                    }

                    // block의 targets, providers 초기화
                    HashSet<Vector2Int> targets = block.GetTargets();
                    HashSet<Vector2Int> providers = block.GetProviders();

                    foreach (Vector2Int target in targets)
                    {
                        floorBlocks[target.x][target.y].GetComponent<Block>().RemoveProvider(idx);
                    }
                    
                    foreach(Vector2Int provider in providers)
                    {
                        floorBlocks[provider.x][provider.y].GetComponent<Block>().RemoveTarget(idx);
                    }
                    targets.Clear();
                    providers.Clear();
                }
            }
        }
        else if (pallet.GetSelectedBlockType() == BlockType.ITEM)
        {
            Sprite selectedSprite = pallet.GetSelectedSprite();
            if (selectedSprite != pallet.spriteEmpty)
            {
                itemBlocks[idx.x][idx.y].GetComponent<Block>().PaintOver(selectedSprite);
            }
        }

        editorManager.ChangedAnyData();
    }

    public void TriggerMouseEnter(Vector2Int idx)
    {
        isMouseEntered = true;
        mouseLastEnteredIdx = idx;

        if (!isOpenedBlockSetting)
        {
            Paint(idx);
        }
    }

    public void TriggerMouseExit(Vector2Int idx)
    {
        isMouseEntered = false;

        floorBlocks[idx.x][idx.y].GetComponent<Block>().Erase();
        itemBlocks[idx.x][idx.y].GetComponent<Block>().Erase();
    }

    public void TriggerMouseDown(Vector2Int idx)
    {
        if (!isOpenedBlockSetting)
        {
            PaintOver(idx);
        }
    }

    public void TriggerMouseLeftClick(Vector2Int idx)
    {
        if (isOpenedBlockSetting)
        {
            if (currentSelectedBlock.GetSprite().name.Contains("Trap_"))
            {
                TrapLeftClickEvent(idx);
            }

            // target 추가 button, portal에만 추가됨
            int funcResult = currentSelectedBlock.AddTarget(idx);

            // 선택
            if (funcResult == 1)
            {
                highlightBlocks[idx.x][idx.y].GetComponent<HighLightBlock>().SetColor(targetColor);
                floorBlocks[idx.x][idx.y].GetComponent<Block>().AddProvider(currentSelectedIdx);
                blockSettingWindow.Refresh(currentSelectedIdx, currentSelectedBlock);

                editorManager.ChangedAnyData();
            }
            // 선택 취소
            else if (funcResult == -1)
            {
                highlightBlocks[idx.x][idx.y].GetComponent<HighLightBlock>().SetColor(emptyColor);
                floorBlocks[idx.x][idx.y].GetComponent<Block>().RemoveProvider(currentSelectedIdx);
                blockSettingWindow.Refresh(currentSelectedIdx, currentSelectedBlock);

                editorManager.ChangedAnyData();
            }
            // 선택 불가
            else if (funcResult == 0) { }
        }
    }

    public void TriggerMouseRightClick(Vector2Int idx)
    {
        bool isFloorActive = floorLayer.activeInHierarchy;
        if (true == isFloorActive)
        {
            if (isOpenedBlockSetting && currentSelectedBlock != null && currentSelectedBlock.GetSprite().name.Contains("Trap_"))
            {
                TrapRightClickEvent(idx);
            }
            else
            {
                OpenBlockSettingWindow(idx);
            }
        }
    }

    void TrapLeftClickEvent(Vector2Int idx)
    {
        // 선택된 메인블록을 다시 클릭한 경우
        if (currentSelectedIdx == idx)
        {
            // 아무런 반응을 하지않음
        }
        else
        {
            Block block = floorBlocks[idx.x][idx.y].GetComponent<Block>();

            // Trap을 클릭한 경우
            if (block.GetSprite().name.Contains("Trap_"))
            {
                // 선택되어 있는 경우
                if (selectedTrap.GroupCheck(currentSelectedIdx, idx))
                {
                    // 선택된 메인블록과 trapGroup에서도 같은 그룹인 경우
                    if (trapGroup.GroupCheck(currentSelectedIdx, idx))
                    {
                        // 새로운 집합으로 분리함
                        selectedTrap.SplitElements(currentSelectedIdx, trapGroup.GetAllElementsList(idx));
                    }
                    // 선택된 메인블록과 trapGroup에서 다른 그룹인 경우
                    else
                    {
                        // trapGroup에서의 집합으로 돌려보냄
                        selectedTrap.SplitElements(currentSelectedIdx, trapGroup.GetAllElementsList(idx), trapGroup.GetRoot(idx));
                    }
                }
                // 선택되어 있지 않은 경우
                else
                {
                    // trapgroup에서 같은 그룹인 모든 trap을 합침
                    foreach (Vector2Int vector2Int in trapGroup.GetAllElementsList(idx))
                    {
                        selectedTrap.Union(currentSelectedIdx, vector2Int);
                    }

                    // BlockSettingWindow 새로고침
                    blockSettingWindow.RefreshTrap(idx, floorBlocks[idx.x][idx.y].GetComponent<Block>(), autoTrapSet.Contains(idx));
                }

                RefreshHighLight();
            }
            else
            {
                // Trap이 아닌 블록을 클릭한 경우 반응하지 않음
            }
        }
    }

    // Trap이 선택된 상태로 우클릭한 경우
    void TrapRightClickEvent(Vector2Int idx)
    {
        // 선택된 메인블록을 다시 클릭한 경우
        if (currentSelectedIdx == idx)
        {
            OpenBlockSettingWindow(idx);
        }
        else
        {
            Block block = floorBlocks[idx.x][idx.y].GetComponent<Block>();

            // Trap을 클릭한 경우
            if (block.GetSprite().name.Contains("Trap_"))
            {
                // 선택되어 있는 경우
                if (selectedTrap.GroupCheck(currentSelectedIdx, idx))
                {
                    // 원래 같은 그룹인 경우
                    if (trapGroup.GroupCheck(currentSelectedIdx, idx))
                    {
                        // 새로운 그룹으로 분리
                        selectedTrap.SplitElement(idx);
                    }
                    else
                    {
                        // 원래 속한 그룹으로 이동
                        selectedTrap.SplitElement(idx, trapGroup.GetRoot(idx));
                    }
                }
                // 선택되어 있지 않은 경우
                else
                {
                    // 이전에 속해있던 그룹에서 제거하고 현재 선택된 그룹에 추가
                    selectedTrap.SplitElement(idx);
                    selectedTrap.Union(currentSelectedIdx, idx);

                    // BlockSettingWindow 새로고침
                    blockSettingWindow.RefreshTrap(idx, floorBlocks[idx.x][idx.y].GetComponent<Block>(), autoTrapSet.Contains(idx));
                }
                RefreshHighLight();
            }
            else
            {
                // Trap이 아닌 블록을 클릭한 경우 반응하지 않음
            }
        }
    }

    public void ResetTargets()
    {
        HashSet<Vector2Int> targets = currentSelectedBlock.GetTargets();
        foreach (Vector2Int target in targets)
        {
            floorBlocks[target.x][target.y].GetComponent<Block>().RemoveProvider(currentSelectedIdx);
        }
        targets.Clear();
        blockSettingWindow.Refresh(currentSelectedIdx, currentSelectedBlock);
        RefreshHighLight();
    }

    public void RefreshHighLight()
    {
        if (isOpenedBlockSetting)
        {
            HighLightSettingMode();
        }
        else
        {
            HighLightPaintingMode();
        }
    }

    void HighLightSettingMode()
    {
        // 선택된 블록이 Trap인 경우
        if (currentSelectedBlock.GetSprite().name.Contains("Trap_"))
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (i == currentSelectedIdx.x && j == currentSelectedIdx.y)
                    {
                        highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(mainTrapColor);
                    }
                    else if (floorBlocks[i][j].GetComponent<Block>().GetSprite().name.Contains("Trap_") && selectedTrap.GroupCheck(currentSelectedIdx, new Vector2Int(i, j)))
                    {
                        if (trapGroup.GroupCheck(currentSelectedIdx, new Vector2Int(i, j)))
                        {
                            highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(sameGroupColor);
                        }
                        else
                        {
                            highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(differentGroupColor);
                        }
                    }
                    else
                    {
                        highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(HighLightBlock.Color.EMPTY);
                    }
                }
            }
        }
        // 선택된 블록이 Trap이 아닌 경우
        else
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (i == currentSelectedIdx.x && j == currentSelectedIdx.y)
                    {
                        highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(providerColor);
                    }
                    else
                    {
                        highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(HighLightBlock.Color.EMPTY);
                    }
                }
            }

            HashSet<Vector2Int> targets = currentSelectedBlock.GetTargets();
            foreach (Vector2Int target in targets)
            {
                highlightBlocks[target.x][target.y].GetComponent<HighLightBlock>().SetColor(targetColor);
            }
        }

        highlightLayer.SetActive(true);
    }

    void HighLightPaintingMode()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (i == editorManager.GetStartIdx().x && j == editorManager.GetStartIdx().y)
                {
                    highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(startIdxColor);
                }
                else
                {
                    highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(HighLightBlock.Color.EMPTY);
                }
            }
        }

        highlightLayer.SetActive(true);
    }

    void OpenBlockSettingWindow(Vector2Int selectedIdx)
    {
        currentSelectedBlock = floorBlocks[selectedIdx.x][selectedIdx.y].GetComponent<Block>();
        currentSelectedIdx = selectedIdx;
        isOpenedBlockSetting = blockSettingWindow.Open(selectedIdx, currentSelectedBlock, autoTrapSet.Contains(selectedIdx));
        if (isOpenedBlockSetting)
        {
            selectedTrap = trapGroup.Copy();

            floorBlocks[selectedIdx.x][selectedIdx.y].GetComponent<Block>().Erase();
            itemBlocks[selectedIdx.x][selectedIdx.y].GetComponent<Block>().Erase();
            HighLightSettingMode();
        }
        else
        {
            Paint(selectedIdx);
            HighLightPaintingMode();
        }
    }

    void CloseBlockSettingWindow()
    {
        isOpenedBlockSetting = false;
        HighLightPaintingMode();
        blockSettingWindow.Close();
    }

    public Vector2Int GetMapSize()
    {
        return new Vector2Int(mapWidth, mapHeight);
    }

    public Block GetItemBlock(int x, int y)
    {
        return itemBlocks[x][y].GetComponent<Block>();
    }

    public Block GetFloorBlock(int x, int y)
    {
        return floorBlocks[x][y].GetComponent<Block>();
    }

    public HashSet<Vector2Int> GetAutoTrapSet()
    {
        return autoTrapSet;
    }

    public void Paint()
    {
        if (isMouseEntered)
        {
            Paint(mouseLastEnteredIdx);
        }
    }

    public void SetAutoTrap(bool isAutoTrap)
    {
        if (!isOpenedBlockSetting)
        {
            return;
        }

        List<Vector2Int> list = selectedTrap.GetAllElementsList(currentSelectedIdx);
        if (isAutoTrap)
        {
            foreach (Vector2Int trap in list)
            {
                autoTrapSet.Add(trap);
            }
        }
        else
        {
            foreach (Vector2Int trap in list)
            {
                if (autoTrapSet.Contains(trap))
                {
                    autoTrapSet.Remove(trap);
                }
            }
        }

        trapGroup = selectedTrap.Copy();

        RefreshHighLight();
    }

    public void SetDelay(int firstdelay, int odddelay, int evendelay)
    {
        List<Vector2Int> list = selectedTrap.GetAllElementsList(currentSelectedIdx);
        for (int i = 0; i < list.Count; i++)
        {
            floorBlocks[list[i].x][list[i].y].GetComponent<Block>().SetTrapDelay(firstdelay, odddelay, evendelay);
        }
    }
}
