using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHider : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Animator anim;
    public CardEnemy card;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        if (sprite != null) sprite.enabled = false;
        if (anim != null) anim.enabled = false;
        if (card != null) card.enabled = false;
        yield return new WaitForSeconds(0.5f);
        if (sprite != null) sprite.enabled = true;
        if (anim != null) anim.enabled = true;
        yield return new WaitForSeconds(0.5f);
        if (card != null) card.enabled = true;
    }
}
