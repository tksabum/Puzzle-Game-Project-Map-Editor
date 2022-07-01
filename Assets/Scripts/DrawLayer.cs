using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLayer : MonoBehaviour
{
    [Header("- Core -")]
    public Pallet pallet;

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

    private void Awake()
    {
        floorBlocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            floorBlocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                floorBlocks[i].Add(Instantiate(floorBlock, new Vector3(i, j, 0), Quaternion.identity, floorLayer.transform));
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

        isOpenedBlockSetting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    public void ActiveFloorLayer()
    {
        floorLayer.SetActive(floorLayerSetting.isOn);
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
                floorBlocks[idx.x][idx.y].GetComponent<Block>().PaintOver(selectedSprite);
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

        }
    }

    public void TriggerMouseRightClick(Vector2Int idx)
    {
        OpenBlockSettingWindow(idx);
    }

    void ShowHighLight(Vector2Int selectedIdx)
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (i == selectedIdx.x && j == selectedIdx.y)
                {
                    highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(HighLightBlock.Color.GREEN);
                }
                else
                {
                    highlightBlocks[i][j].GetComponent<HighLightBlock>().SetColor(HighLightBlock.Color.EMPTY);
                }
            }
        }

        highlightLayer.SetActive(true);
    }

    void HideHighLight()
    {
        highlightLayer.SetActive(false);
    }

    void OpenBlockSettingWindow(Vector2Int selectedIdx)
    {
        currentSelectedBlock = floorBlocks[selectedIdx.x][selectedIdx.y].GetComponent<Block>();
        isOpenedBlockSetting = blockSettingWindow.Open(selectedIdx, currentSelectedBlock);
        if (isOpenedBlockSetting)
        {
            floorBlocks[selectedIdx.x][selectedIdx.y].GetComponent<Block>().Erase();
            itemBlocks[selectedIdx.x][selectedIdx.y].GetComponent<Block>().Erase();
            ShowHighLight(selectedIdx);
        }
        else
        {
            Paint(selectedIdx);
            HideHighLight();
        }
    }

    void CloseBlockSettingWindow()
    {
        isOpenedBlockSetting = false;
        blockSettingWindow.Close();
    }
}
