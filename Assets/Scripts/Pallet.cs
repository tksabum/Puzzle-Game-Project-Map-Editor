using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pallet : MonoBehaviour
{
    [Header("- BlockList -")]
    public List<BlockSetComponent> floorList;
    public List<BlockSetComponent> itemList;

    [Header("- Title Pallet -")]
    public List<Image> titleObjects;
    public TMP_Text pageViewer;

    [Header("- Detail Pallet -")]
    public GameObject detailPallet;

    [Header("- Selected Pallet -")]
    public Image selectedPallet;

    [Header("- ETC -")]
    public Sprite spriteEmpty;

    Sprite selectedSprite;
    BlockType selectedBlockType;

    BlockType currentBlockType;
    int currentPage;
    int maxPage;

    int currentTitle;

    List<GameObject> detailPalletBlocksets;
    List<List<GameObject>> detailPalletBlocks;

    private void Awake()
    {
        detailPalletBlocksets = new List<GameObject>();
        detailPalletBlocks = new List<List<GameObject>>();

        for (int i = 0; i < detailPallet.transform.childCount; i++)
        {
            detailPalletBlocks.Add(new List<GameObject>());
            GameObject blockset = detailPallet.transform.GetChild(i).gameObject;
            detailPalletBlocksets.Add(blockset);
            for (int j = 0; j < blockset.transform.childCount; j++)
            {
                detailPalletBlocks[i].Add(blockset.transform.GetChild(j).gameObject);
            }
        }

        Init();
    }

    public void Init()
    {
        SelectBlock(spriteEmpty, BlockType.FLOOR);
        SelectPalletType(BlockType.FLOOR);
        SelectTitle(-1);
    }

    void SelectPalletType(BlockType blockType)
    {
        currentBlockType = blockType;

        if (blockType == BlockType.FLOOR)
        {
            maxPage = (floorList.Count - 1) / 10 + 1;
            DrawTitlePallet(0);
        }
        else if (blockType == BlockType.ITEM)
        {
            maxPage = (itemList.Count - 1) / 10 + 1;
            DrawTitlePallet(0);
        }
    }
    public void SelectPalletType(PalletSelectorComponent palletSelectorComponent)
    {
        SelectPalletType(palletSelectorComponent.blockType);
    }

    void DrawTitlePallet()
    {
        if (currentBlockType == BlockType.FLOOR)
        {
            for (int i = 0; i < 10; i++)
            {
                int index = i + 10 * currentPage;
                if (index < floorList.Count)
                {
                    titleObjects[i].sprite = floorList[index].titleSprite;
                }
                else
                {
                    titleObjects[i].sprite = spriteEmpty;
                }
            }

            pageViewer.text = (currentPage + 1) + " / " + maxPage;
        }
        else if (currentBlockType == BlockType.ITEM)
        {
            for (int i = 0; i < 10; i++)
            {
                int index = i + 10 * currentPage;
                if (index < itemList.Count)
                {
                    titleObjects[i].sprite = itemList[index].titleSprite;
                }
                else
                {
                    titleObjects[i].sprite = spriteEmpty;
                }
            }

            pageViewer.text = (currentPage + 1) + " / " + maxPage;
        }
    }

    void DrawTitlePallet(int page)
    {
        currentPage = Mathf.Clamp(page, 0, maxPage - 1);
        SelectTitle(-1);
        DrawTitlePallet();
    }

    public void DrawPrevTitlePallet()
    {
        DrawTitlePallet(currentPage - 1);
    }

    public void DrawNextTitlePallet()
    {
        DrawTitlePallet(currentPage + 1);
    }

    public void SelectTitle (int selectedTitle)
    {
        currentTitle = selectedTitle;
        DrawDetailPallet();
    }

    void DrawDetailPallet()
    {
        if (currentTitle == -1)
        {
            detailPallet.SetActive(false);
            return;
        }

        detailPallet.SetActive(true);

        int index = currentTitle + 10 * currentPage;
        if (currentBlockType == BlockType.FLOOR && index < floorList.Count)
        {
            Vector2Int palletSize = floorList[index].size;
            
            for (int i = 0; i < detailPalletBlocks.Count; i++)
            {
                // detail pallet 크기 조정
                if (i < palletSize.x)
                {
                    detailPalletBlocksets[i].SetActive(true);
                }
                else
                {
                    detailPalletBlocksets[i].SetActive(false);
                }

                for (int j = 0; j < detailPalletBlocks[i].Count; j++)
                {
                    if (i < palletSize.x && j < palletSize.y)
                    {
                        // detail pallet 크기 조정
                        detailPalletBlocks[i][j].SetActive(true);

                        // detail pallet에 sprites 넣기
                        if (palletSize.x * j + i < floorList[index].sprites.Count)
                        {
                            detailPalletBlocks[i][j].GetComponent<Image>().sprite = floorList[index].sprites[palletSize.x * j + i];
                        }
                        else
                        {
                            detailPalletBlocks[i][j].GetComponent<Image>().sprite = spriteEmpty;
                        }
                    }
                    else
                    {
                        // detail pallet 크기 조정
                        detailPalletBlocks[i][j].SetActive(false);
                    }
                }
            }
        }
        else if (currentBlockType == BlockType.ITEM && index < itemList.Count)
        {
            Vector2Int palletSize = itemList[index].size;

            for (int i = 0; i < detailPalletBlocks.Count; i++)
            {
                // detail pallet 크기 조정
                if (i < palletSize.x)
                {
                    detailPalletBlocksets[i].SetActive(true);
                }
                else
                {
                    detailPalletBlocksets[i].SetActive(false);
                }

                for (int j = 0; j < detailPalletBlocks[i].Count; j++)
                {
                    if (i < palletSize.x && j < palletSize.y)
                    {
                        // detail pallet 크기 조정
                        detailPalletBlocks[i][j].SetActive(true);

                        // detail pallet에 sprites 넣기
                        if (palletSize.x * j + i < itemList[index].sprites.Count)
                        {
                            detailPalletBlocks[i][j].GetComponent<Image>().sprite = itemList[index].sprites[palletSize.x * j + i];
                        }
                        else
                        {
                            detailPalletBlocks[i][j].GetComponent<Image>().sprite = spriteEmpty;
                        }
                    }
                    else
                    {
                        // detail pallet 크기 조정
                        detailPalletBlocks[i][j].SetActive(false);
                    }
                }
            }
        }
        else
        {
            detailPallet.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)detailPallet.transform);
    }

    void DrawSelectedPallet()
    {
        selectedPallet.sprite = selectedSprite;
        SelectTitle(-1);
    }

    public void SelectBlock(Sprite sprite, BlockType blockType)
    {
        selectedSprite = sprite;
        selectedBlockType = blockType;
        DrawSelectedPallet();
    }

    public void SelectDetail(Image image)
    {
        if (image.sprite != spriteEmpty)
        {
            selectedSprite = image.sprite;
            selectedBlockType = currentBlockType;
            DrawSelectedPallet();
        }
    }

    public Sprite GetSelectedSprite()
    {
        return selectedSprite;
    }

    public BlockType GetSelectedBlockType()
    {
        return selectedBlockType;
    }

    public Sprite GetFloorSprite(string floorName)
    {
        foreach(BlockSetComponent floorBlockSet in floorList)
        {
            foreach (Sprite sprite in floorBlockSet.sprites)
            {
                if (floorName == sprite.name)
                {
                    return sprite;
                }
            }
        }

        throw new System.Exception("Error: floorList에 " + floorName + "이 존재하지 않음");
    }

    public Sprite GetItemSprite(string itemName)
    {
        foreach(BlockSetComponent itemBlockSet in itemList)
        {
            foreach(Sprite sprite in itemBlockSet.sprites)
            {
                if (sprite != null && itemName == sprite.name)
                {
                    return sprite;
                }
            }
        }

        throw new System.Exception("Error: itemList에 " + itemName + "이 존재하지 않음");
    }
}
