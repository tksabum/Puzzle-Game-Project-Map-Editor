using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlock : MonoBehaviour
{
    public Sprite defaultSprite;

    Sprite currentSprite;

    Pallet pallet;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        pallet = GameObject.Find("Pallet").GetComponent<Pallet>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentSprite = defaultSprite;
        spriteRenderer.sprite = defaultSprite;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        spriteRenderer.sprite = pallet.GetSelectedSprite();
    }

    private void OnMouseExit()
    {
        spriteRenderer.sprite = currentSprite;
    }

    private void OnMouseDown()
    {

    }
}
