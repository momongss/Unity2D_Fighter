using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public const int WEAPON = 0;

    protected int itemType;
    
    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.layer == 10 || go.layer == 12) // Player, PlayerDamaged
        {
            ItemEvent(go);
        }
    }

    protected virtual void ItemEvent(GameObject player)
    {
        // 비워두기
    }
}
