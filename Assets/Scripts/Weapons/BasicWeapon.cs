using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : Weapon
{
    public Transform cannon;

    public override void Shoot(Vector3 direction, float range, float aimAssist)
    {
        base.Shoot(direction, range, aimAssist);

        RaycastHit hitInfo;
        if (Physics.SphereCast(cannon.position, aimAssist, shootDirection, out hitInfo, range, layer))
        {
            TrailEffect(weaponData, cannon.position, hitInfo.point, hitInfo.normal);
            Debug.DrawRay(cannon.position, shootDirection, Color.green);

            DamageEnemy(hitInfo);
        }
        else
        {
            Debug.DrawRay(cannon.position, direction, Color.red);
            
            TrailEffect(weaponData, cannon.position, cannon.position + shootDirection, Vector3.zero);
        }

        MuzzleEffect(weaponData, cannon.position);
    }
}
