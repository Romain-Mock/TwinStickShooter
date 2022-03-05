using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    public Transform cannon;
    public Transform anchor;
    public Transform crosshair;

    LineRenderer line;
    new AudioSource audio;

    float lastShootTime = 0;

    IEnumerator currentCoroutine = null;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        SetAudioSource(weaponData.sfxData);

        transform.position = anchor.position;
        transform.SetParent(anchor);
        //transform.LookAt(crosshair);
    }

    public void Shoot()
    {
        if (lastShootTime + weaponData.fireRate < Time.time)
        {
            switch (weaponData.weaponType)
            {
                case WeaponData.WeaponType.Hitscan:

                    Vector3 forwardXZ = cannon.forward;
                    forwardXZ.y = 0;

                    Debug.DrawRay(cannon.position, forwardXZ * weaponData.range, Color.green);
                    Ray ray = new Ray(cannon.position, forwardXZ);
                    RaycastHit hitInfo;
                    float shotDistance = weaponData.range;
                    GameObject vfx = Instantiate(weaponData.fireVFX, cannon.position, transform.rotation);
                    TrailRenderer trail = Instantiate(weaponData.trailVFX, cannon.position, transform.rotation).GetComponent<TrailRenderer>();

                    Destroy(vfx, 1f);
                    if (Physics.Raycast(ray, out hitInfo, weaponData.range))
                    {
                        shotDistance = hitInfo.distance;

                        StartCoroutine(SpawnTrail(trail, hitInfo));

                        if (hitInfo.transform.GetComponent<Enemy>())
                        {
                            hitInfo.transform.GetComponent<Enemy>().TakeDamage(weaponData.damage);
                        }
                    }
                    else
                    {
                        StartCoroutine(SpawnTrail(trail, weaponData.range));
                    }

                    break;

                case WeaponData.WeaponType.Instantiate:

                    GameObject go = Instantiate(weaponData.fireVFX, cannon.position, transform.localRotation);
                    go.GetComponent<Bullet>().SetSpeed(weaponData.bulletSpeed);
                    go.transform.localRotation = transform.rotation;

                    break;
            }

            audio.Play();

            lastShootTime = Time.time;

            //currentCoroutine = RenderLine(delayBetweenShots, forwardXZ, shotDistance);
            //StartCoroutine(currentCoroutine);
        }
        
    }

    public void StopShooting()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    IEnumerator RenderLine(float delay, Vector3 forward, float distance)
    {
        line.enabled = true;
        line.SetPosition(0, cannon.position);
        line.SetPosition(1, cannon.position + forward * distance);
        yield return null;
        line.enabled = false;
    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        Instantiate(weaponData.impactVFX, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

    IEnumerator SpawnTrail(TrailRenderer trail, float distance)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, startPosition + trail.transform.forward * distance, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        Destroy(trail.gameObject, trail.time);
    }

    void SetAudioSource(SFXData data)
    {
        audio = GetComponent<AudioSource>();

        audio.clip = data.audioClip;
        audio.priority = data.priority;
        audio.volume = data.volume;
        audio.pitch = data.pitch;
    }
}
