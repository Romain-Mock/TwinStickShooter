using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float radius = 3f;
    public GameObject pickUpSprite;
    private GameObject instance;
    public Transform closestObject;
    private WeaponManager manager;
    public List<Weapon> weaponInRange;

    void Start()
    {
        manager = GetComponent<WeaponManager>();
        weaponInRange = new List<Weapon>();
    }

    void Update()
    {
        if (weaponInRange.Count > 0)
            closestObject = GetClosestWeapon(manager.unequippedWeapons);
    }

    Transform GetClosestWeapon(List<Weapon> weapons)
    {
        Transform closestObject = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Weapon potentialTarget in weapons)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestObject = potentialTarget.transform;
            }
        }

        if (Vector3.Distance(transform.position, closestObject.position) < radius)
            return closestObject;
        else
            return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
