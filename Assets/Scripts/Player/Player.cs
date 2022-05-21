using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Character
{
    public static event Action<float> OnPlayerUpdateHealth;

    private void Start()
    {
        OnPlayerUpdateHealth?.Invoke(_currentHealth);
    }

    public override void TakeDamage(float value)
    {
        base.TakeDamage(value);
        OnPlayerUpdateHealth?.Invoke(_currentHealth);
    }
}
