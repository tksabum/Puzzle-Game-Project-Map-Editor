using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Sprite DefaultSprite;

    SpriteRenderer spriteRenderer;

    Sprite currentSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = DefaultSprite;
    }

    public void Paint(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void Erase()
    {
        spriteRenderer.sprite = currentSprite;
    }

    public void PaintOver(Sprite sprite)
    {
        currentSprite = sprite;
        spriteRenderer.sprite = sprite;
    }

    public void Clear()
    {
        
    }

    
}
