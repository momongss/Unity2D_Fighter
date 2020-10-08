using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Seek : Bullet
{
    public float SeekRange;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        targetLayer = LAYER_ENEMY;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        RaycastHit2D[] rayHit = new RaycastHit2D[16];
        for (int i = 0; i < 16; i++)
        {
            float x = Mathf.Cos(Mathf.PI * i / 8);
            float y = Mathf.Sin(Mathf.PI * i / 8);
            Debug.DrawRay(rigid.position, (new Vector2(x, y)) * SeekRange, new Color(0, 1, 0));
            rayHit[i] = Physics2D.Raycast(rigid.position, new Vector2(x, y), SeekRange, LayerMask.GetMask("Enemy"));
        }

        string result = "";
        
        for (int i = 0; i < 16; i++)
        {
            if (rayHit[i].collider != null)
            {
                result += 1;
            }
            else
            {
                result += 0;
            }
            if (i % 4 == 3) result += " ";
        }

        Debug.Log(result);
    }
}
