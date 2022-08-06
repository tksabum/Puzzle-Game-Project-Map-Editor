using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : Floorbase
{
    public enum GeneratorType
    {
        POWER,
        TIME
    }

    [Header("- Item Generator -")]
    public bool powerDefault;
    public GeneratorType generatorType;
    public int generationCycle;
    public BlockManager.Obj product;

    private new void Awake()
    {
        base.Awake();

        Init();
    }

    void Init()
    {
        power = powerDefault;
    }


    //// ItemGenerator에서 생성되는 아이템 설정
    //public void SetProduct(BlockManager.Obj obj)
    //{
    //    product = obj;
    //}

    //// 아이템 생성방식 설정
    //public void SetGeneratorType(GeneratorType type)
    //{
    //    generatorType = type;
    //}


    // 게임시작시 플레이어, 아이템이 위에 있다면 실행되야 함
    // 플레이어 이동시 PreMoveEvent에서 실행되야 함

    public override void OnPreObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

    }

    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

    }

    public override void OnPostObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {

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

        // 전원이 켜질 때
        if (generatorType == GeneratorType.POWER && power)
        {
            GenerateItem(gameManager.blockManager);
        }
    }

    public void GenerateItem(BlockManager blockManager)
    {
        blockManager.GenerateItem(product, idx);
    }

    public void Tick(BlockManager blockManager, int tickCount)
    {
        if (generatorType == GeneratorType.TIME && tickCount % generationCycle == 0)
        {
            GenerateItem(blockManager);
        }
    }

    void OnDisable()
    {
        Init();
    }
}
