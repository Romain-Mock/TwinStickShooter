using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DualWeapon : Weapon
{
    public Transform[] cannons;

    public override void Shoot(Vector3 direction, float range, float aimAssist)
    {
        StartCoroutine(DualShoot(direction, range, aimAssist));
    }

    IEnumerator DualShoot(Vector3 direction, float range, float aimAssist)
    {
        foreach (Transform c in cannons)
        {
            base.Shoot(direction, range, aimAssist);

            //If the shot hit something
            RaycastHit hitInfo;
            if (Physics.SphereCast(c.position, weaponData.aimAssist, shootDirection, out hitInfo, range, layer))
            {
                TrailEffect(weaponData, c.position, hitInfo.point, hitInfo.normal);

                DamageEnemy(hitInfo);
            }
            else
            {
                Debug.DrawRay(c.position, c.transform.forward * weaponData.range, Color.red);
                TrailEffect(weaponData, c.position, c.position + shootDirection, Vector3.zero);
            }

            MuzzleEffect(weaponData, c.position);

            yield return new WaitForSeconds(0.5f);
        }   
    }
}