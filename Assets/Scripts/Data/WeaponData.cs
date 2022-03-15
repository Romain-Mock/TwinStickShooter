using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WeaponData : ScriptableObject
{
    new public string name;
    public enum WeaponType { Hitscan, Instantiate};
    [Header("Weapon type")]
    public WeaponType weaponType;

    [Header("Weapon stats")]
    public float damage;
    public float fireRate;
    public float range;
    public bool addSpread;
    public bool dualWielding;

    [Header("SFX")]
    public SFXData sfxData;

    [Header("Bullet prefab")]
    public GameObject bullet;
    public float bulletSpeed;

    [Header("VFX")]
    public GameObject fireVFX;
    public GameObject impactVFX;
    public GameObject trailVFX;
}
