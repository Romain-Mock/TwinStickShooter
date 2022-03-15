using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

//Manages weapons
public class WeaponManager : MonoBehaviour
{
    private PickUp pickUp;
    public List<Weapon> unequippedWeapons;    //All the weapons in the scene
    Transform weaponSlotOne;    //First weapon anchor
    Transform weaponSlotTwo;    //Second weapon anchor
    int activeWeapon = 1;       //Weapon currently equipped
    public Weapon currentWeapon;
    private GameObject sceneWeaponHolder;

    public GameObject weaponOne;    //First weapon, child of weaponSlotOne
    public GameObject weaponTwo;    //Second weapon, child of weaponSlotTwo

    float lastShootTime = 0;    //Last time the player shot (used to calculate when next he can shoot

    PlayerInputActions playerInputAction;

    void Awake()
    {
        pickUp = GetComponent<PickUp>();
        playerInputAction = new PlayerInputActions();
        playerInputAction.Player.Enable();
        playerInputAction.Player.SwitchWeapon.performed += SwitchWeapon;
        playerInputAction.Player.PickUp.performed += ReplaceWeapon;

        unequippedWeapons = new List<Weapon>();
    }

    //Set position on the player and get all weapons
    void Start()
    {
        if (!sceneWeaponHolder)
            sceneWeaponHolder = new GameObject("Weapons");

        InstantiateStartingWeapons();

        UpdateWeaponsList();   //Get all weapons in the scene

        weaponSlotOne = transform.Find("Slot1");
        weaponSlotTwo = transform.Find("Slot2");
    }

    private void Update()
    {
        currentWeapon = GetActiveWeapon().GetChild(0).GetComponent<Weapon>();

        if (playerInputAction.Player.FireGamepad.ReadValue<Vector2>() != Vector2.zero && Time.time >= lastShootTime)
        {
            //Shoot periodically according to the fire rate
            lastShootTime = Time.time + 1f / currentWeapon.weaponData.fireRate;
            Shoot();
        }
    }

    //Update la liste des armes disponibles de la scene (celles qui ne sont pas tenues par le joueur ou les ennemis
    private void UpdateWeaponsList()
    {
        Debug.Log("Updating weapon list");
        foreach (Weapon go in GameObject.FindObjectsOfType<Weapon>())
        {
            if (!go.isEquipped && !unequippedWeapons.Contains(go))           //Si l'arme est n'est pas équippée on l'ajoute à la liste
            {
                unequippedWeapons.Add(go);
                go.transform.parent = sceneWeaponHolder.transform;
            }
            else if (go.isEquipped)
            {
                unequippedWeapons.Remove(go);  //Sinon on la retire de la liste
            }
        }
    }

    //Set the given weapon as active weapon, setting its position, rotation and parent to the anchor
    public void ReplaceWeapon(InputAction.CallbackContext context)
    {
        Weapon newWeapon = pickUp.closestObject.GetComponent<Weapon>();

        if (currentWeapon)
            DropWeapon(currentWeapon);  //Set active weapon's position, rotation and parent to 0

        PickUpWeapon(newWeapon);

        UpdateWeaponsList();    //Update unequipped weapon list
    }

    void PickUpWeapon(Weapon newWeapon)
    {
        Debug.Log("Pick weapon : " + newWeapon.name);
        //Set new weapon's position, rotation and parent to this
        newWeapon.transform.position = GetActiveWeapon().transform.position;
        newWeapon.transform.rotation = GetActiveWeapon().transform.rotation;
        newWeapon.transform.SetParent(GetActiveWeapon().transform);
        newWeapon.isEquipped = true;  //Set new weapon as equipped
    }
    
    //Unparent given weapon
    void DropWeapon(Weapon weapon)
    {
        Debug.Log("Drop weapon : " + weapon.name);
        weapon.transform.SetParent(null);
        weapon.transform.GetComponent<Weapon>().isEquipped = false;
        UpdateWeaponsList();
    }

    //Switch between both equipped weapons
    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        Debug.Log("Weapon switch");

        //De activate the active weapon and activate the other
        if (activeWeapon == 1)
        {
            weaponSlotOne.gameObject.SetActive(false);
            weaponSlotTwo.gameObject.SetActive(true);
            activeWeapon = 2;
        }
        else
        {
            weaponSlotOne.gameObject.SetActive(true);
            weaponSlotTwo.gameObject.SetActive(false);
            activeWeapon = 1;
        }
    }

    //Return the gameobject of the active weapon
    public Transform GetActiveWeapon()
    {
        if (activeWeapon == 1)
            return weaponSlotOne;
        else
            return weaponSlotTwo;
    }

    //Instantiate starting weapons in their slots and de activate the second one
    void InstantiateStartingWeapons()
    {
        GameObject go1 = Instantiate(weaponOne, transform.position, transform.rotation, transform.Find("Slot1"));
        go1.GetComponent<Weapon>().isEquipped = true;
        go1.name = "Starting Weapon 1";
        GameObject go2 = Instantiate(weaponTwo, transform.position, transform.rotation, transform.Find("Slot2"));
        go2.GetComponent<Weapon>().isEquipped = true;
        go2.name = "Starting Weapon 2";
        transform.Find("Slot2").gameObject.SetActive(false);
    }

    //Get the active weapon and shoot
    public void Shoot()
    {
        //If the weapon is hitscan 
        if (currentWeapon.weaponData.weaponType == WeaponData.WeaponType.Hitscan)
        {
            currentWeapon.Shoot();
        }
    }

    
}
