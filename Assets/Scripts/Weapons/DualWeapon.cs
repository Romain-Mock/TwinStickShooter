using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DualWeapon : Weapon
{
    public Transform[] cannons;

    public override void Shoot()
    {
        StartCoroutine(DualShoot());
    }

    IEnumerator DualShoot()
    {
        foreach (Transform c in cannons)
        {
            Vector3 direction = GetDirection(c);
            Ray newRay = new Ray(c.position, direction);
            //If the shot hit something
            RaycastHit hitInfo;
            if (Physics.Raycast(newRay, out hitInfo))
            {
                Debug.Log("Object hit : " + hitInfo.transform.name);
                TrailEffect(weaponData, c.position, hitInfo.point, hitInfo.normal);

                //If the object hit is an enemy
                if (hitInfo.transform.GetComponent<Enemy>())
                {
                    hitInfo.transform.GetComponent<Enemy>().TakeDamage(weaponData.damage);  //Enemy takes damage
                }
            }
            else
            {
                Debug.DrawRay(c.position, c.transform.forward * weaponData.range, Color.red);
                TrailEffect(weaponData, c.position, c.position + direction, Vector3.zero);
            }

            MuzzleEffect(weaponData, c.position);
            base.Shoot();
            yield return new WaitForSeconds(0.5f);
        }
    }
}