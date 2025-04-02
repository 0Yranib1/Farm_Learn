using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame

    public void FadeOut()
    {
        Color targetColor=new Color(1,1,1,Settings.targetAlpha);
        spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }
    public void FadeIn()
    {
    Color targetColor=new Color(1,1,1,1);
    spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }
}
