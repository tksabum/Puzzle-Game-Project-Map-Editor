using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortCutKeySet : MonoBehaviour
{
    public Image image;

    BlockType blockType;
    Sprite sprite;

    public void SetShortCut(BlockType _blockType, Sprite _sprite)
    {
        blockType = _blockType;
        sprite = _sprite;
        image.sprite = _sprite;
    }

    public BlockType GetBlockType()
    {
        return blockType;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
}
