using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Floorbase
{
    [Header("- Sprite -")]
    public Sprite spriteOn;
    public Sprite spriteOff;

    [Header("- Power -")]
    [Tooltip("powerType�� False�� �� PowerOn ���¿��� Trap�� ��Ȱ��ȭ�ǰ� PowerOff ���¿��� Ȱ��ȭ�˴ϴ�.")]
    public bool powerType;
    public bool powerDefault;

    bool isWorkOnTick;
    int firstDelay; // ù toggle ���� �� ����
    int oddDelay; // Ȧ����° toggle �� �����ð�
    int evenDelay; // ¦����° toggle �� �����ð�

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
        isWorkOnTick = false;

        RefreshSprite();
    }

    public void SetWorkOnTick(ThreeInt threeInt)
    {
        SetWorkOnTick(threeInt.x, threeInt.y, threeInt.z);
    }

    public void SetWorkOnTick(int firstdelay, int odddelay, int evendelay)
    {
        isWorkOnTick = true;

        firstDelay = firstdelay;
        oddDelay = odddelay;
        evenDelay = evendelay;
    }

    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        if (!(power ^ powerType) && (obj == BlockManager.Obj.PLAYER))
        {
            gameManager.AttackedPlayer();
        }
    }

    public override void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        
    }

    public override void PowerToggle(GameManager gameManager)
    {
        power = !power;
        RefreshSprite();

        if (!(power ^ powerType))
        {
            gameManager.AttackedPos(idx);
        }
    }

    public bool ToggleOnCurrentTick(int tickCount)
    {
        if (!isWorkOnTick)
        {
            return false;
        }

        if (tickCount >= firstDelay)
        {
            int currentCount = (tickCount - firstDelay) % (oddDelay + evenDelay);

            if (currentCount == 0 || currentCount == oddDelay)
            {
                return true;
            }
        }

        return false;
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
