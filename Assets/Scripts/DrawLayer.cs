using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    int mapWidth;
    int mapHeight;

    bool isOpenedBlockSetting;
    Block currentSelectedBlock;
    Vector2Int currentSelectedIdx;

    HighLightBlock.Color startIdxColor = HighLightBlock.Color.RED;

    HighLightBlock.Color emptyColor = HighLightBlock.Color.EMPTY;
    HighLightBlock.Color providerColor = HighLightBlock.Color.GREEN;
    HighLightBlock.Color targetColor = HighLightBlock.Color.BLUE;


    private void Awake()
    {
        floorBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            floorBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                GameObject inst = Instantiate(floorBlock, new Vector3(i, j, 0), Quaternion.identity, floorLayer.transform);
                inst.GetComponent<Block>().SetIdx(new Vector2Int(i, j));
                floorBlocks[i].Add(inst);
            }
        }


        itemBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            itemBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                itemBlocks[i].Add(Instantiate(itemBlock, new Vector3(i, j, 0), Quaternion.identity, itemLayer.transform));
            }
        }


        triggerBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            triggerBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                GameObject inst = Instantiate(triggerBlock, new Vector3(i, j, 0), Quaternion.identity, triggerLayer.transform);
                inst.GetComponent<TriggerBlock>().SetIdx(new Vector2Int(i, j));
                triggerBlocks[i].Add(inst);
            }
        }

        highlightBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            highlightBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                highlightBlocks[i].Add(Instantiate(highlightBlock, new Vector3(i, j, 0), Quaternion.identity, highlightLayer.transform));
            }
        }
    }

    public void Init()
    {
        isOpenedBlockSetting = false;
        CloseBlockSettingWindow();
    }

    public void InitBlocks()
    {
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
                    floorBlocks[i][j].SetActive(true);
                    itemBlocks[i][j].SetActive(true);
                    triggerBlocks[i][j].SetActive(true);
                    highlightBlocks[i][j].SetActive(true);
                }
                else
                {
                    floorBlocks[i][j].SetActive(false);
                    itemBlocks[i][j].SetActive(false);
                    triggerBlocks[i][j].SetActive(false);
                    highlightBlocks[i][j].SetActive(false);
                }
            }
        }

        editorManager.ChangedAnyData();
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
                
                // block의 targets, providers 초기화
                if (isOver)
                {
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
    }


    public void TriggerMouseEnter(Vector2Int idx)
    {
        if (!isOpenedBlockSetting)
        {
            Paint(idx);
        }
    }

    public void TriggerMouseExit(Vector2Int idx)
    {
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
            int funcResult = currentSelectedBlock.AddTarget(idx);

            // 선택
            if (funcResult == 1)
            {
                highlightBlocks[idx.x][idx.y].GetComponent<HighLightBlock>().SetColor(targetColor);
                floorBlocks[idx.x][idx.y].GetComponent<Block>().AddProvider(currentSelectedIdx);
                blockSettingWindow.Refresh(currentSelectedIdx, currentSelectedBlock);
            }
            // 선택 취소
            else if (funcResult == -1)
            {
                highlightBlocks[idx.x][idx.y].GetComponent<HighLightBlock>().SetColor(emptyColor);
                floorBlocks[idx.x][idx.y].GetComponent<Block>().RemoveProvider(currentSelectedIdx);
                blockSettingWindow.Refresh(currentSelectedIdx, currentSelectedBlock);
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
            OpenBlockSettingWindow(idx);
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
        RefreshHighLightSettingMode();
    }

    public void RefreshHighLight()
    {
        if (isOpenedBlockSetting)
        {
            RefreshHighLightSettingMode();
        }
        else
        {
            HighLightPaintingMode();
        }
    }

    void RefreshHighLightSettingMode()
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

    void HighLightSettingMode()
    {
        RefreshHighLightSettingMode();

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
        isOpenedBlockSetting = blockSettingWindow.Open(selectedIdx, currentSelectedBlock);
        if (isOpenedBlockSetting)
        {
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
}
