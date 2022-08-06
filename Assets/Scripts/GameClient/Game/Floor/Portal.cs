using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Floorbase
{
    [Header("- Sprite -")]
    public Sprite spriteOn;
    public Sprite spriteOff;

    [Header("- Power -")]
    [Tooltip("powerType이 False일 때 PowerOn 상태에서 Portal이 비활성화되고 PowerOff 상태에서 활성화됩니다.")]
    public bool powerType;
    public bool powerDefault;

    SpriteRenderer spriteRenderer;

    private new void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();

        Init();
    }

    void Init()
    {
        power = powerDefault;
        RefreshSprite();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPreObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

    }

    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

    }

    public override void OnPostObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        if (!(power ^ powerType) && (obj == BlockManager.Obj.PLAYER))
        {
            // 이동 이벤트
            gameManager.MovePortal(idx);
        }
    }

    public override void OnPreObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

    }

    public override void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

    }

    public override void OnPostObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

    }

    public override void PowerToggle(GameManager gameManager, BlockManager blockManager)
    {
        power = !power;
        RefreshSprite();
    }

    void RefreshSprite()
    {
        if (power ^ powerType)
        {
            spriteRenderer.sprite = spriteOff;
        }
        else
        {
            spriteRenderer.sprite = spriteOn;
        }
    }

    private void OnDisable()
    {
        Init();
    }
}
