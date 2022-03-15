using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : Weapon
{
    public Transform cannon;

    public override void Shoot()
    {
        Vector3 direction = GetDirection(cannon);
        Ray newRay = new Ray(cannon.position, direction);
        //If the shot hit something
        RaycastHit hitInfo;
        //if (Physics.Raycast(cannon.position, cannon.forward, out hitInfo, weaponData.range))
        if (Physics.Raycast(newRay, out hitInfo))
        {
            Debug.Log("Object hit : " + hitInfo.transform.name);
            TrailEffect(weaponData, cannon.position, hitInfo.point, hitInfo.normal);

            //If the object hit is an enemy
            if (hitInfo.transform.GetComponent<Enemy>())
            {
                hitInfo.transform.GetComponent<Enemy>().TakeDamage(weaponData.damage);  //Enemy takes damage
            }
        }
        else
        {
            Debug.DrawRay(cannon.position, direction, Color.red);
            TrailEffect(weaponData, cannon.position, cannon.position + direction, Vector3.zero);
        }

        MuzzleEffect(weaponData, cannon.position);

        base.Shoot();
    }
}
