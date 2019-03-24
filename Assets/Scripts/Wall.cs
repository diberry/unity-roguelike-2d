using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    // player hit wall
    public Sprite damageSprite;

    // hit points
    public int hitpoints = 4;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        // set sprite to damaged sprite - visual feedback that they have successfully attacked wall
        spriteRenderer.sprite = damageSprite;

        // subtract loss
        hitpoints = -loss;

        // when hit points are 0, wall is no longer active
        if (hitpoints <= 0)
            gameObject.SetActive(false);

    }
}
