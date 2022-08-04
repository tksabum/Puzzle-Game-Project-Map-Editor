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

    bool isObjectEntered;

    private new void Awake()
    {
        base.Awake();

        Init();
    }

    void Init()
    {
        isObjectEntered = false;
        power = powerDefault;
    }


    //// ItemGenerator���� �����Ǵ� ������ ����
    //public void SetProduct(BlockManager.Obj obj)
    //{
    //    product = obj;
    //}

    //// ������ ������� ����
    //public void SetGeneratorType(GeneratorType type)
    //{
    //    generatorType = type;
    //}


    // ���ӽ��۽� �÷��̾�, �������� ���� �ִٸ� ����Ǿ� ��
    // �÷��̾� �̵��� PreMoveEvent���� ����Ǿ� ��
    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        isObjectEntered = true;
    }

    public override void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        isObjectEntered = false;
    }

    public override void PowerToggle(GameManager gameManager)
    {
        power = !power;

        // ������ ���� �� ���� ������ �������� ���� ���
        if (generatorType == GeneratorType.POWER && power && !isObjectEntered)
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
        if (generatorType == GeneratorType.TIME && tickCount % generationCycle == 0 && !isObjectEntered)
        {
            GenerateItem(blockManager);
        }
    }

    void OnDisable()
    {
        Init();
    }
}
