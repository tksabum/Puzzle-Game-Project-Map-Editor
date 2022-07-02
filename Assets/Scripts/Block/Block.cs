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

    Vector2Int idx;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = DefaultSprite;

        providers = new HashSet<Vector2Int>();
        targets = new HashSet<Vector2Int>();
    }

    public void SetIdx(Vector2Int _idx)
    {
        idx = _idx;
    }

    public void Paint(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void Erase()
    {
        spriteRenderer.sprite = currentSprite;
    }

    // sprite가 교체되었으면 return true
    public bool PaintOver(Sprite sprite)
    {
        bool returnValue = false;
        if (currentSprite != sprite)
        {
            returnValue = true;
        }
        currentSprite = sprite;
        spriteRenderer.sprite = sprite;

        return returnValue;
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

    /*
     *  AddTarget
     *  Target을 추가할 경우 return 1
     *  Target의 변화가 없을 경우 return 0
     *  Target을 삭제할 경우 return -1
     */
    public int AddTarget(Vector2Int targetIdx)
    {
        string spriteName = currentSprite.name;
        bool containsButton = spriteName.Contains("Button_");
        bool containsPortal = spriteName.Contains("Portal_");

        if (targetIdx == idx) return 0;

        if (containsButton || containsPortal)
        {
            if (!targets.Contains(targetIdx))
            {
                if (containsButton && targets.Count < 20 || containsPortal && targets.Count < 1)
                {
                    targets.Add(targetIdx);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                targets.Remove(targetIdx);
                return -1;
            }
        }

        return 0;
    }

    public void RemoveTarget(Vector2Int targetIdx)
    {
        if (targets.Contains(targetIdx))
        {
            targets.Remove(targetIdx);
        }
    }

    public void AddProvider(Vector2Int providerIdx)
    {
        providers.Add(providerIdx);
    }

    public void RemoveProvider(Vector2Int providerIdx)
    {
        if (providers.Contains(providerIdx))
        {
            providers.Remove(providerIdx);
        }
    }
}
