using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy_Normal : Enemy
{
    protected override void setVelocity(Vector2 playerPosition, Vector2 position)
    {
        // 기본 이동 재정의 : 땅에서 움직임
        Vector2 moveDirc = new Vector2((playerPosition - position).x, 0).normalized;
        rigid.velocity = moveDirc * normalSpeed;
    }
}
