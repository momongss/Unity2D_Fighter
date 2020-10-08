using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Weapon : Item
{
    public int weaponType;
    public int amount;

    void Awake()
    {
        itemType = 0;
    }

    void Update()
    {
        
    }

    protected override void ItemEvent(GameObject player)
    {
        Destroy(gameObject);
    }

}
