using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    public WeaponManager manager;
    public WeaponData weaponData;   //Stats and effects of the weapon
    public Transform cannon;        //Position of the cannon

    public bool isEquipped;

    new AudioSource audio;
    Vector3 direction;

    public static event Action OnWeaponDropped;

    //public bool IsEquipped
    //{
    //    get { return isEquipped; }
    //    set
    //    {
    //        if (isEquipped != value)
    //        {
    //            isEquipped = value;
    //            OnWeaponDropped?.Invoke();
    //        }
    //    }
    //}

    private void Awake()
    {
        manager = FindObjectOfType<WeaponManager>();
        SetAudioSource(weaponData.sfxData);
    }

    //Used by the player controller to shoot the weapon
    public void ShootHitscan()
    {
        

        //If the shot hit something
        RaycastHit hitInfo;
        if (Physics.Raycast(cannon.position, cannon.transform.forward, out hitInfo, weaponData.range))
        {
            manager.SpawnEffects(hitInfo.point, hitInfo.normal);
            Debug.Log("Object hit : " + hitInfo.transform.name);

            //If the object hit is an enemy
            if (hitInfo.transform.GetComponent<Enemy>())
            {
                hitInfo.transform.GetComponent<Enemy>().TakeDamage(weaponData.damage);  //Enemy takes damage
            }
        }
        else
        {
            Debug.DrawRay(cannon.position, cannon.transform.forward * weaponData.range, Color.red);
            manager.SpawnEffects(cannon.position + cannon.transform.forward * weaponData.range, Vector3.zero);
        }

        audio.Play();
    }

    public void ShootBullet()
    {
        GameObject go = Instantiate(weaponData.fireVFX, cannon.position, transform.localRotation);  //Instantiate bullet
        go.GetComponent<Bullet>().SetSpeed(weaponData.bulletSpeed); //Set bullet speed on the bullet scrpt
        go.transform.localRotation = transform.rotation;
    }

    //Set the audio source parameters of the weapon according to the audio settings of the sound
    void SetAudioSource(SFXData data)
    {
        audio = GetComponent<AudioSource>();

        audio.clip = data.audioClip;
        audio.priority = data.priority;
        audio.volume = data.volume;
        audio.pitch = data.pitch;
    }
}
