using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPTurret : Turret
{
    public void attacked(Vector2 collisionDir, float damage, int damageType, float effectDuration)
    {
        OnDamaged(collisionDir, damage, damageType, effectDuration);
    }

    private void OnDamaged(Vector2 collisionDir, float damage, int damageType, float effectDuration)
    {
        Debug.Log("Hit");
    }
}
