using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private float _radius = 3f;          //Radius for the closest object
    private GameObject _pickUpSprite;   //Sprite to intantiate over closest weapon
    private Transform _closestObject;   //Closest weapon to pick up
    private WeaponManager _manager;     //Reference to the weapon manager

    //Getters and setters
    public Transform ClosestObject { get { return _closestObject; } }

    void Start()
    {
        _manager = GetComponent<WeaponManager>();
    }

    void Update()
    {   //If there are at least 1 weapon that can be picked up, return the closest weapon
        if (_manager.UnequippedWeapons.Count > 0)
            _closestObject = GetClosestWeapon(_manager.UnequippedWeapons);
    }

    //Return the closest weapon within the list of all the weapons that can be picked up
    Transform GetClosestWeapon(List<Weapon> weapons)
    {
        Transform closestWeapon = null;     //Initialize the closest weapon to null
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        //For each weapon in the given list
        //check if the distance between the player and the weapon is smaller than the closestDistanceSqr
        //if it is, set the weapon as the closest weapon and set its distance to the player to the closestDistanceSqr
        foreach (Weapon potentialTarget in weapons)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestWeapon = potentialTarget.transform;
            }
        }

        //If the distance between the closest weapon and the player is smaller than the radius
        //return the weapon, else return null
        if (Vector3.Distance(transform.position, closestWeapon.position) < _radius)
            return closestWeapon;
        else
            return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
