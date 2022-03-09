using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Manages weapons
public class WeaponManager : MonoBehaviour
{
    List<Weapon> allWeapons;    //All the weapons in the scene
    Transform weaponSlotOne;    //First weapon anchor
    Transform weaponSlotTwo;    //Second weapon anchor
    int activeWeapon = 1;       //Weapon currently equipped

    public GameObject weaponOne;    //First weapon, child of weaponSlotOne
    public GameObject weaponTwo;    //Second weapon, child of weaponSlotTwo
    Transform crosshair;

    float lastShootTime = 0;    //Last time the player shot (used to calculate when next he can shoot

    PlayerInputActions playerInputAction;

    void Awake()
    {
        playerInputAction = new PlayerInputActions();
        playerInputAction.Player.Enable();
        playerInputAction.Player.SwitchWeapon.performed += SwitchWeapon;
    }

    //Set position on the player and get all weapons
    void Start()
    {
        InstantiateStartingWeapons();

        //UpdateWeaponsList();   //Get all weapons in the scene

        //crosshair = GameManager.instance.GetChildByName(transform, "Crosshair").transform;
        weaponSlotOne = transform.Find("Slot1");
        weaponSlotTwo = transform.Find("Slot2");
    }

    private void Update()
    {
        Weapon currentWeapon = GetActiveWeapon().GetComponent<Weapon>();

        if (playerInputAction.Player.FireGamepad.ReadValue<Vector2>() != Vector2.zero && Time.time >= lastShootTime)
        {
            //Shoot periodically according to the fire rate
            lastShootTime = Time.time + 1f / currentWeapon.weaponData.fireRate;
            Shoot();
        }
    }

    //Update la liste des armes de la scene
    private void UpdateWeaponsList()
    {
        foreach (Weapon go in GameObject.FindObjectsOfType<Weapon>())
        {
            if (!go.equipped)           //Si l'arme est équippée on l'ajoute à la liste
                allWeapons.Add(go);
            else
                allWeapons.Remove(go);  //Sinon on la retire de la liste
        }
    }

    //Set the given weapon as active weapon, setting its position, rotation and parent to the anchor
    public void ReplaceWeapon(Weapon newWeapon)
    {
        Transform currentWeapon = GetActiveWeapon();    

        ReleaseActiveWeapon();  //Set active weapon's position, rotation and parent to 0

        //Set new weapon's position, rotation and parent to this
        newWeapon.transform.position = currentWeapon.transform.position;
        newWeapon.transform.rotation = currentWeapon.transform.rotation;
        newWeapon.transform.SetParent(currentWeapon.transform);
        newWeapon.equipped = true;  //Set new weapon as equipped

        newWeapon.crosshair = crosshair;

        UpdateWeaponsList();    //Update unequipped weapon list
    }
    
    //Unparent active weapon
    void ReleaseActiveWeapon()
    {
        GetActiveWeapon().SetParent(null);
        GetActiveWeapon().position = Vector3.zero;
        GetActiveWeapon().rotation = Quaternion.identity;
        GetActiveWeapon().GetComponent<Weapon>().equipped = false;
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
            return weaponSlotOne.GetChild(0);
        else
            return weaponSlotTwo.GetChild(0);
    }

    //Instantiate starting weapons in their slots and de activate the second one
    void InstantiateStartingWeapons()
    {
        GameObject go1 = Instantiate(weaponOne, transform.position, transform.rotation, transform.Find("Slot1"));
        go1.GetComponent<Weapon>().equipped = true;
        GameObject go2 = Instantiate(weaponTwo, transform.position, transform.rotation, transform.Find("Slot2"));
        go2.GetComponent<Weapon>().equipped = true;
        transform.Find("Slot2").gameObject.SetActive(false);
    }

    //Get the active weapon and shoot
    public void Shoot()
    {
        Weapon currentWeapon = GetActiveWeapon().GetComponent<Weapon>();

        //If the weapon is hitscan 
        if (currentWeapon.weaponData.weaponType == WeaponData.WeaponType.Hitscan)
        {
            GetActiveWeapon().GetComponent<Weapon>().ShootHitscan();
            ////Shoot the weapon
            //RaycastHit hitInfo = GetActiveWeapon().GetComponent<Weapon>().ShootHitscan();
            ////Instantiate the muzzle and trail vfx
            //SpawnEffects(hitInfo);
        }
    }

    //Spawn the vfx
    public void SpawnEffects(Vector3 hitPoint, Vector3 hitNormal)
    {
        Weapon weap = GetActiveWeapon().GetComponent<Weapon>();
        GameObject vfx = Instantiate(weap.weaponData.fireVFX, weap.cannon.position, transform.rotation);  //Instnatiate the particules effets on the cannon
        TrailRenderer trail = Instantiate(weap.weaponData.trailVFX, weap.cannon.position, Quaternion.identity).GetComponent<TrailRenderer>(); //Instantiate the trail particules along the raycast
        Destroy(vfx, 1f); //Destroying the cannon VFX
        StartCoroutine(MoveTrail(weap.weaponData, trail, hitPoint, hitNormal));
    }

    //Move the trail gameobject towards the hit point (or max range if nothing is hit) and instantiate impact VFX
    IEnumerator MoveTrail(WeaponData data, TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal)
    {
        Vector3 startPosition = trail.transform.position;

        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float startingDistance = distance;

        //Move the trail from its start position to the hit point each frame
        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * data.bulletSpeed;

            yield return null;
        }

        trail.transform.position = hitPoint;

        //If the raycast hit something, instantiate impact VFX
        if (hitNormal != Vector3.zero)
        {
            GameObject go = Instantiate(data.impactVFX, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(go, 1f);
        }

        Destroy(trail.gameObject, trail.time);  //Destroy object upon arriving
    }
}
