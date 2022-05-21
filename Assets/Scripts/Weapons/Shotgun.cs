using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public Transform cannon;
    public Vector3 spread;
    public int nbShards;

    public override void Shoot(Vector3 direction, float range, float aimAssist)
    {
        base.Shoot(direction, range, aimAssist);

        for (int i = 0; i < nbShards; i++)
        {
            shootDirection += AddSpread();

            RaycastHit hitInfo;
            Debug.DrawRay(cannon.position, shootDirection, Color.red, 3f);
            if (Physics.Raycast(cannon.position, shootDirection, out hitInfo, range))
            {
                TrailEffect(weaponData, cannon.position, hitInfo.point, hitInfo.normal);

                DamageEnemy(hitInfo);
            }
            else
                TrailEffect(weaponData, cannon.position, cannon.position + shootDirection, Vector3.zero);
        }

        MuzzleEffect(weaponData, cannon.position);
    }

    public Vector3 AddSpread()
    {
        Vector3 newSpread = new Vector3(
            Random.Range(-spread.x, spread.x), 
            Random.Range(-spread.y, spread.y),
            Random.Range(-spread.z, spread.z));

        newSpread.Normalize();

        return newSpread;
    }
}
