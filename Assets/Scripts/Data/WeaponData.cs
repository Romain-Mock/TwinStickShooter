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
    public int magazineCapacity;
    public float reloadTime;
    public float aimAssist;

    [Header("SFX")]
    public AudioClip shotSfx;
    public AudioClip reloadSfx;
    public AudioClip emptySfx;

    [Header("Bullet prefab")]
    public BulletData bulletSettings;

    [Header("VFX")]
    public GameObject fireVFX;
    public GameObject impactVFX;
    public GameObject trailVFX;
}
