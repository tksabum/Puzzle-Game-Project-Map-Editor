using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Sprite DefaultSprite;

    SpriteRenderer spriteRenderer;

    Sprite currentSprite;

    HashSet<Vector2Int> providers;
    HashSet<Vector2Int> targets;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = DefaultSprite;

        providers = new HashSet<Vector2Int>();
        targets = new HashSet<Vector2Int>();
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

    public Sprite GetSprite()
    {
        return currentSprite;
    }

    public HashSet<Vector2Int> GetProviders()
    {
        return providers;
    }

    public HashSet<Vector2Int> GetTargets()
    {
        return targets;
    }
}
