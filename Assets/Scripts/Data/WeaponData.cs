using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WeaponData : ScriptableObject
{
    public float damage;
    public float fireRate;
    public float range;
    public float bulletSpeed;

    public Color startColor;
    public Color endColor;

    public GameObject fireEffect;
}
