using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;   //Stats and effects of the weapon
    public Transform cannon;        //Position of the cannon
    public Transform crosshair;     //Position of the crosshair
    public bool equipped;

    new AudioSource audio;
    Vector3 direction;

    private void Awake()
    {
        SetAudioSource(weaponData.sfxData);

        if (equipped)
        {
            crosshair = GameObject.Find("Crosshair").transform;
        }
    }

    //Used by the player controller to shoot the weapon
    public RaycastHit ShootHitscan()
    {
        RaycastHit hitPoint;
        
        if (crosshair)
            direction = (crosshair.position - cannon.position); //Making sure the direction of the shot is towards the crosshair
        else
            direction = Vector3.forward;

        Debug.DrawRay(cannon.position, direction * weaponData.range, Color.red);

        //If the shot hit something
        RaycastHit hitInfo;
        if (Physics.Raycast(cannon.position, direction, out hitInfo, weaponData.range))
        {
            hitPoint = hitInfo;

            Debug.Log("Object hit : " + hitInfo.transform.name);

            //If the object hit is an enemy
            if (hitInfo.transform.GetComponent<Enemy>())
            {
                hitInfo.transform.GetComponent<Enemy>().TakeDamage(weaponData.damage);  //Enemy takes damage
            }
        }

        audio.Play();

        return hitInfo;
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
