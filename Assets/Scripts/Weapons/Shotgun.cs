using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public Transform cannon;
    public Vector3 spread;
    public int nbShards;

    public override void Shoot()
    {
        base.Shoot();

        for (int i = 0; i < nbShards; i++)
        {
            Vector3 direction = GetDirection(cannon) * weaponData.range;
            Ray newRay = new Ray(cannon.position, direction);
            RaycastHit hitInfo;
            Debug.DrawRay(cannon.position, direction, Color.red, 3f);
            if (Physics.Raycast(newRay, out hitInfo))
            {
                TrailEffect(weaponData, cannon.position, hitInfo.point, hitInfo.normal);
                //TrailRenderer trail = Instantiate(weaponData.trailVFX, cannon.position, Quaternion.identity).GetComponent<TrailRenderer>(); //Instantiate the trail particules along the raycast
                //StartCoroutine(MoveTrail(weaponData, trail, hitInfo.point, hitInfo.normal));
            }
            else
            {
                TrailEffect(weaponData, cannon.position, cannon.position + direction, Vector3.zero);
                //TrailRenderer trail = Instantiate(weaponData.trailVFX, cannon.position, Quaternion.identity).GetComponent<TrailRenderer>(); //Instantiate the trail particules along the raycast
                //StartCoroutine(MoveTrail(weaponData, trail, cannon.position + direction, Vector3.zero));
            }
        }

        MuzzleEffect(weaponData, cannon.position);
    }

    public override Vector3 GetDirection(Transform c)
    {
        Vector3 direction = c.forward;
        direction += new Vector3(
            Random.Range(-spread.x, spread.x), 
            Random.Range(-spread.y, spread.y),
            Random.Range(-spread.z, spread.z));

        direction.Normalize();

        return direction;
    }
}
