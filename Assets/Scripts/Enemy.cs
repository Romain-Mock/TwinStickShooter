using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData stats;

    float health;

    // Start is called before the first frame update
    void Awake()
    {
        health = stats.health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health >= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
