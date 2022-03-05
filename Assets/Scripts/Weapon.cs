using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData stats;
    public Transform cannon;
    public Transform anchor;
    public Transform crosshair;

    LineRenderer line;

    float delayBetweenShots = 0f;
    float nextTimeToFire = 0;

    IEnumerator currentCoroutine = null;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        SetGradient();

        delayBetweenShots = 60 / stats.fireRate;

        transform.position = anchor.position;
        transform.SetParent(anchor);
        transform.LookAt(crosshair);
    }

    public void Shoot()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / stats.fireRate;

            Vector3 forwardXZ = cannon.forward;
            forwardXZ.y = 0;

            Debug.DrawRay(cannon.position, forwardXZ * stats.range, Color.green);
            Ray ray = new Ray(cannon.position, forwardXZ);
            RaycastHit hitInfo;
            float shotDistance = stats.range;

            GameObject go = (GameObject)Instantiate(stats.fireEffect, cannon.position, transform.localRotation);
            go.GetComponent<Bullet>().SetSpeed(stats.bulletSpeed);
            go.transform.localRotation = transform.rotation;
            Destroy(go, 2f);

            if (Physics.Raycast(ray, out hitInfo, stats.range))
            {
                shotDistance = hitInfo.distance;

                if (hitInfo.transform.GetComponent<Enemy>())
                {
                    hitInfo.transform.GetComponent<Enemy>().TakeDamage(stats.damage);
                }
            }

            currentCoroutine = RenderLine(delayBetweenShots, forwardXZ, shotDistance);
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

    void SetGradient()
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = stats.startColor;
        colorKey[0].time = 0f;
        colorKey[1].color = stats.endColor;
        colorKey[1].time = 1f;
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0f;
        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 1f;
        gradient.SetKeys(colorKey, alphaKey);

        line.colorGradient = gradient;
    }
}
