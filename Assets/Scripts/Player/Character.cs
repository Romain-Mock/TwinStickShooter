using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected float _currentHealth;
    public float maxHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float value)
    {
        _currentHealth -= value;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Mourir");
    }
}