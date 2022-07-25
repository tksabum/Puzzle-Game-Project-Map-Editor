using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public List<GameObject> lifeImages;
    public GameObject pauseImage;
    public GameObject gameoverImage;
    public GameObject gameclearImage;
    public GameObject nextButton;
    public Text mapNameText;
    public Image itemImage;

    [Header("Sprite")]
    public Sprite hammerSprite;

    int nowlife;

    private void Awake()
    {
        nowlife = 5;
    }

    public void Reset(int life, string mapName, BlockManager.Obj obj)
    {
        SetLife(life);
        SetMapName(mapName);
        SetNextButton(mapName);
        SetPause(false);
        SetGameOver(false);
        SetGameClear(false);
        SetItem(obj);
    }

    public void SetLife(int life)
    {
        if (nowlife == -1)
        {
            for (int i = 0; i < life; i++)
            {
                lifeImages[i].SetActive(true);
            }
            nowlife = life;
        }
        else
        {
            if (nowlife < life)
            {
                for (int i = nowlife; i < life; i++)
                {
                    lifeImages[i].SetActive(true);
                }
                nowlife = life;
            }
            else
            {
                for (int i = life; i < nowlife; i++)
                {
                    lifeImages[i].SetActive(false);
                }
                nowlife = life;
            }
        }
    }

    public void SetPause(bool pause)
    {
        if (pause)
        {
            pauseImage.SetActive(true);
        }
        else
        {
            pauseImage.SetActive(false);
        }
    }

    public void SetGameOver(bool gameover)
    {
        if (gameover)
        {
            gameoverImage.SetActive(true);
        }
        else
        {
            gameoverImage.SetActive(false);
        }
    }

    public void SetGameClear(bool gameclear)
    {
        if (gameclear)
        {
            gameclearImage.SetActive(true);
        }
        else
        {
            gameclearImage.SetActive(false);
        }
    }

    public void SetNextButton(string mapName)
    {
        if (mapName.Length > 5 && mapName.Substring(0, 5) == "Story" && mapName[mapName.Length - 2] == '-')
        {
            nextButton.SetActive(true);
        }
        else
        {
            nextButton.SetActive(false);
        }
    }

    public void SetMapName(string mapName)
    {
        mapNameText.text = mapName;
    }

    public void SetItem(BlockManager.Obj obj)
    {
        if (obj == BlockManager.Obj.EMPTY)
        {
            itemImage.gameObject.SetActive(false);
        }
        else if (obj == BlockManager.Obj.HAMMER)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = hammerSprite;
        }
        else
        {
            throw new System.Exception("Error:set wrong item");
        }
    }
}
