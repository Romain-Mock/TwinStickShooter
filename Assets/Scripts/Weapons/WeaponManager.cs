using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class WeaponManager : MonoBehaviour
{
    //Inputs
    PlayerInputActions _playerInput;            //Player input
    
    //Pick up weapons
    private PickUp _pickUp;                     //Script that returns closest weapon to pick up
    private List<Weapon> _unequippedWeapons;    //All the unequipped weapons in the scene
    private GameObject _sceneWeaponHolder;      //Gameobject that contains all unequipped weapons
    
    //Equipped weapons
    [SerializeField]
    private GameObject[] _startingWeapons;
    [SerializeField]
    private Transform _weaponSlotOne;           //First weapon anchor
    [SerializeField]
    private Transform _weaponSlotTwo;           //Second weapon anchor
    private int _activeWeapon = 1;              //Weapon currently equipped
    public Weapon _currentWeapon;              //The weapon that is activated

    public bool IsSwitching { get; private set; }
    private bool _isReloading { get { return _currentWeapon.IsReloading; } }
    [SerializeField]
    private float switchSpeed;

    //Getters and setters
    public List<Weapon> UnequippedWeapons { get { return _unequippedWeapons; } }

    public static event Action<int, int> OnWeaponUpdateAmmo;    //Update ammo count to display to canvas

    //Register and un register input events
    private void OnEnable()
    {
        _playerInput.Player.SwitchWeapon.performed += SwitchWeapon;
        _playerInput.Player.PickUp.performed += ReplaceWeapon;
    }

    private void OnDisable()
    {
        _playerInput.Player.SwitchWeapon.performed -= SwitchWeapon;
        _playerInput.Player.PickUp.performed -= ReplaceWeapon;
    }

    void Awake()
    {
        _pickUp = GetComponent<PickUp>();
        _playerInput = new PlayerInputActions();
        _playerInput.Player.Enable();

        _unequippedWeapons = new List<Weapon>();
    }

    //Set position on the player and get all weapons
    void Start()
    {
        //If it doesn't exist already, create a gameobject to store all the weapons in the scene
        _sceneWeaponHolder = GameObject.Find("Weapons");
        if (!_sceneWeaponHolder)
            _sceneWeaponHolder = new GameObject("Weapons");

        InstantiateStartingWeapons();   //Instantiate starting weapons and equip them to the player

        UpdateWeaponsList();   //Put all unequipped weapons in the scene in a list
    }

    private void Update()
    {
        _currentWeapon = GetActiveWeapon().GetChild(0).GetComponent<Weapon>();
    }

    //Update la liste des armes disponibles de la scene (celles qui ne sont pas tenues par le joueur ou les ennemis
    private void UpdateWeaponsList()
    {
        Debug.Log("Updating weapon list");
        foreach (Weapon go in FindObjectsOfType<Weapon>())
        {
            if (!go.IsEquipped && !_unequippedWeapons.Contains(go))           //Si l'arme est n'est pas équippée on l'ajoute à la liste
            {
                _unequippedWeapons.Add(go);
                go.transform.parent = _sceneWeaponHolder.transform;
            }
            else if (go.IsEquipped)
            {
                _unequippedWeapons.Remove(go);  //Sinon on la retire de la liste
            }
        }
    }

    //Set the given weapon as active weapon, setting its position, rotation and parent to the anchor
    public void ReplaceWeapon(InputAction.CallbackContext context)
    {
        Weapon newWeapon;

        if (_pickUp.ClosestObject != null && !_isReloading)
        {
            newWeapon = _pickUp.ClosestObject.GetComponent<Weapon>();

            if (_currentWeapon)
                DropWeapon(_currentWeapon);  //Set active weapon's position, rotation and parent to 0

            PickUpWeapon(newWeapon);

            UpdateWeaponsList();    //Update unequipped weapon list
        }
    }

    void PickUpWeapon(Weapon newWeapon)
    {
        Debug.Log("Pick weapon : " + newWeapon.name);

        newWeapon.GetComponent<Rigidbody>().isKinematic = true;
        //Set new weapon's position, rotation and parent to this
        newWeapon.transform.position = GetActiveWeapon().transform.position;
        newWeapon.transform.rotation = GetActiveWeapon().transform.rotation;
        newWeapon.transform.SetParent(GetActiveWeapon().transform);
        newWeapon.IsEquipped = true;  //Set new weapon as equipped
        OnWeaponUpdateAmmo?.Invoke(newWeapon.currentAmmos, newWeapon.ammosInMag);
    }
    
    //Unparent given weapon
    void DropWeapon(Weapon weapon)
    {
        Debug.Log("Drop weapon : " + weapon.name);

        weapon.transform.SetParent(null);
        weapon.transform.GetComponent<Weapon>().IsEquipped = false;
        weapon.GetComponent<Rigidbody>().isKinematic = false;
        weapon.GetComponent<Rigidbody>().AddForce(transform.forward * 7, ForceMode.Impulse);
        UpdateWeaponsList();
    }

    //Switch between both equipped weapons
    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        Debug.Log("Weapon switch");

        if (!_isReloading && !IsSwitching)
            StartCoroutine(SwitchWeapons());
    }

    IEnumerator SwitchWeapons()
    {
        //De activate the active weapon and activate the other
        if (_activeWeapon == 1)
        {
            _weaponSlotOne.gameObject.SetActive(false);
            _weaponSlotTwo.gameObject.SetActive(true);
            _activeWeapon = 2;
        }
        else
        {
            _weaponSlotOne.gameObject.SetActive(true);
            _weaponSlotTwo.gameObject.SetActive(false);
            _activeWeapon = 1;
        }

        IsSwitching = true;

        GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(switchSpeed);

        IsSwitching = false;
    }

    //Return the gameobject of the active weapon
    public Transform GetActiveWeapon()
    {
        if (_activeWeapon == 1)
            return _weaponSlotOne;
        else
            return _weaponSlotTwo;
    }

    //Instantiate starting weapons in their slots and de activate the second one
    void InstantiateStartingWeapons()
    {
        GameObject weapon1 = Instantiate(_startingWeapons[0], transform.position, transform.rotation, _weaponSlotOne);
        weapon1.GetComponent<Weapon>().IsEquipped = true;
        weapon1.GetComponent<Rigidbody>().isKinematic = true;
        weapon1.name = "Starting Weapon 1";

        GameObject weapon2 = Instantiate(_startingWeapons[1], transform.position, transform.rotation, _weaponSlotTwo);
        weapon2.GetComponent<Weapon>().IsEquipped = true;
        weapon2.GetComponent<Rigidbody>().isKinematic = true;
        weapon2.name = "Starting Weapon 2";
        _weaponSlotTwo.gameObject.SetActive(false);
    }
}
