using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBackgroundController : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    [SerializeField] Color baseColor;

    private bool isSelect;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        isSelect = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void setSelect(bool isSelect)
    {
        if(this.isSelect != isSelect)
        {
            if(isSelect)
            {
                spriteRenderer.color = selectedColor;
            }
            else
            {
                spriteRenderer.color = baseColor;
            }

            this.isSelect = isSelect;
        }
    }
}
