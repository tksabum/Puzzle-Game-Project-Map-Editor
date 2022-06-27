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


    [Header("- ETC -")]
    public Sprite spriteEmpty;

    BlockType currentBlockType;
    int currentPage;
    int maxPage;

    // Start is called before the first frame update
    void Start()
    {
        SelectPallet(BlockType.FLOOR);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelectPallet(BlockType blockType)
    {
        currentBlockType = blockType;

        if (blockType == BlockType.FLOOR)
        {
            maxPage = (floorList.Count - 1) / 10 + 1;
            DrawTitlePallet(0, blockType);
        }
        else if (blockType == BlockType.ITEM)
        {
            maxPage = (itemList.Count - 1) / 10 + 1;
            DrawTitlePallet(0, blockType);
        }
    }
    public void SelectPallet(PalletSelectorComponent palletSelectorComponent)
    {
        SelectPallet(palletSelectorComponent.blockType);
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

    void DrawTitlePallet(BlockType blockType)
    {
        if (currentBlockType == blockType) return;
        currentBlockType = blockType;
        DrawTitlePallet();
    }

    void DrawTitlePallet(int page, BlockType blockType)
    {
        currentPage = page;
        currentBlockType = blockType;
        DrawTitlePallet();
    }
}
