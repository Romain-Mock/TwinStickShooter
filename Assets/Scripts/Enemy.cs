using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Oh the misery
public class Enemy : Character
{
    public override void TakeDamage(float value)
    {
        base.TakeDamage(value);
    }
}
