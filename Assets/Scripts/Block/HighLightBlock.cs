using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightBlock : MonoBehaviour
{
    public Sprite spriteBlack;
    public Sprite spriteBlue;
    public Sprite spriteGreen;
    public Sprite spriteRed;
    public Sprite spriteYellow;

    SpriteRenderer spriteRenderer;

    public enum Color
    {
        EMPTY,
        BLACK,
        BLUE,
        GREEN,
        RED,
        YELLOW
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        switch (color)
        {
            case Color.BLACK:
                spriteRenderer.sprite = spriteBlack;
                break;
            case Color.BLUE:
                spriteRenderer.sprite = spriteBlue;
                break;
            case Color.GREEN:
                spriteRenderer.sprite = spriteGreen;
                break;
            case Color.RED:
                spriteRenderer.sprite = spriteRed;
                break;
            case Color.YELLOW:
                spriteRenderer.sprite = spriteYellow;
                break;
            default:
                spriteRenderer.sprite = null;
                break;
        }
    }
}
