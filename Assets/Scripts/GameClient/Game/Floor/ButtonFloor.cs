using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFloor : Floorbase
{
    [Header("- Sprite -")]
    public Sprite spritePressed;
    public Sprite spriteReleased;

    SpriteRenderer spriteRenderer;

    bool isPressed = false;


    private new void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();

        Init();
    }

    void Init()
    {
        isPressed = false;
        RefreshSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        if (obj == BlockManager.Obj.PLAYER || obj == BlockManager.Obj.WOODENBOX)
        {
            blockManager.PowerToggle(idx);
            isPressed = true;
            RefreshSprite();
        }
    }

    public override void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        if (obj == BlockManager.Obj.PLAYER || obj == BlockManager.Obj.WOODENBOX)
        {
            blockManager.PowerToggle(idx);
            isPressed = false;
            RefreshSprite();
        }
    }

    public override void PowerToggle(GameManager gameManager)
    {
        
    }

    void RefreshSprite()
    {
        if (isPressed)
        {
            spriteRenderer.sprite = spritePressed;
        }
        else
        {
            spriteRenderer.sprite = spriteReleased;
        }
    }

    private void OnDisable()
    {
        Init();
    }
}
