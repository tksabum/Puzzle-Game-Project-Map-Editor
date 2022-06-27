using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLayer : MonoBehaviour
{
    public GameObject block;

    List<List<GameObject>> blocks;

    private void Awake()
    {
        blocks = new List<List<GameObject>>();
        for (int i = 0; i < 99; i++)
        {
            blocks.Add(new List<GameObject>());
            for (int j = 0; j < 99; j++)
            {
                blocks[i].Add(Instantiate(block, new Vector3(i, j, 0), Quaternion.identity, transform));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resize(int width, int height)
    {
        for (int i = 0; i < 99; i++)
        {
            for (int j = 0; j < 99; j++)
            {
                if (i < width && j < height)
                {
                    blocks[i][j].SetActive(true);
                }
                else
                {
                    blocks[i][j].SetActive(false);
                }
            }
        }
    }
}
