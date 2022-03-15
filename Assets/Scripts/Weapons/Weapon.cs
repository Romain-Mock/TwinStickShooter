using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour
{
    protected WeaponManager manager;
    public WeaponData weaponData;   //Stats and effects of the weapon

    LayerMask layer;

    public bool isEquipped;

    private AudioSource audioSource;
    Vector3 direction;

    private void Awake()
    {
        manager = FindObjectOfType<WeaponManager>();
        audioSource = GetComponent<AudioSource>();
        SetAudioSource(weaponData.sfxData);
    }

    //Used by the player controller to shoot the weapon
    public virtual void Shoot()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    public virtual Vector3 GetDirection(Transform c)
    {
        return c.forward * weaponData.range;
    }

    //Set the audio source parameters of the weapon according to the audio settings of the sound
    void SetAudioSource(SFXData data)
    {
        audioSource.clip = data.audioClip;
        audioSource.priority = data.priority;
        audioSource.volume = data.volume;
        audioSource.pitch = data.pitch;
    }

    //Spawn the vfx
    public void MuzzleEffect(WeaponData data, Vector3 startPoint)
    {
        GameObject vfx = Instantiate(data.fireVFX, startPoint, Quaternion.identity);  //Instnatiate the particules effets on the cannon
        Destroy(vfx, 0.5f); //Destroying the cannon VFX
    }

    public void TrailEffect(WeaponData data, Vector3 startPoint, Vector3 hitPoint, Vector3 hitNormal)
    {
        TrailRenderer trail = Instantiate(data.trailVFX, startPoint, Quaternion.identity).GetComponent<TrailRenderer>(); //Instantiate the trail particules along the raycast
        StartCoroutine(MoveTrail(data, trail, hitPoint, hitNormal));
    }

    //Move the trail gameobject towards the hit point (or max range if nothing is hit) and instantiate impact VFX
    protected IEnumerator MoveTrail(WeaponData data, TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal)
    {
        Vector3 startPosition = trail.transform.position;

        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float startingDistance = distance;

        //Move the trail from its start position to the hit point each frame
        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * data.bulletSpeed;

            yield return null;
        }

        trail.transform.position = hitPoint;

        //If the raycast hit something, instantiate impact VFX
        if (hitNormal != Vector3.zero)
        {
            GameObject go = Instantiate(data.impactVFX, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(go, 0.5f);
        }

        Destroy(trail.gameObject, trail.time);  //Destroy object upon arriving
    }
}
