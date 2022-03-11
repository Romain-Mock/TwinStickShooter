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
    private float maxDistance;

    //Set la max distance au radius
    //Récupère la liste de toutes les armes non équipées
    //Loop through la liste et compare la distance entre l'objet et ce transform
    //Si cette distance est plus petite que la distance max, stocke l'objet dans closestObject
    //A la fin de la boucle, si la distance entre l'objet le plus proche et ce tranform est inférieure au radius

    void Start()
    {
        manager = GetComponent<WeaponManager>();
        weaponInRange = new List<Weapon>();
    }

    void Update()
    {
        maxDistance = radius;

        closestObject = GetClosestWeapon(manager.allWeapons);
    }

    Transform GetClosestWeapon(List<Weapon> weapons)
    {
        Transform closestObject = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Weapon potentialTarget in weapons)
        {
            Debug.Log(potentialTarget.name);
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestObject = potentialTarget.transform;
            }
        }

        return closestObject;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
