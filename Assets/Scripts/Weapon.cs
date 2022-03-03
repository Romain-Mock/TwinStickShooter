using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData stats;

    LineRenderer line;

    float delayBetweenShots = 0f;

    IEnumerator currentCoroutine = null;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        SetGradient();

        delayBetweenShots = 60 / stats.fireRate;
    }

    public void Shoot()
    {
        Debug.DrawRay(transform.position, transform.forward * stats.range, Color.green);
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        float shotDistance = stats.range;

        if (Physics.Raycast(ray, out hitInfo, stats.range))
        {
            shotDistance = hitInfo.distance;

            if (hitInfo.transform.GetComponent<Enemy>())
            {
                hitInfo.transform.GetComponent<Enemy>().TakeDamage(stats.damage);
            }
        }

        currentCoroutine = RenderLine(delayBetweenShots, shotDistance);
        StartCoroutine(currentCoroutine);
    }

    public void StopShooting()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    IEnumerator RenderLine(float delay, float distance)
    {
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * distance);
        yield return new WaitForSeconds(delay);
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
