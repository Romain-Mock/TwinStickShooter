using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour
{
    //Inputs
    PlayerInputActions _playerInput;            //Player input

    protected WeaponManager manager;
    public WeaponData weaponData;   //Stats and effects of the weapon
    public int currentAmmos;
    public int ammosInMag;
    private float _lastShootTime = 0;           //Last time the player shot (used to calculate when next he can shoot

    private bool _isReloading;
    public bool IsReloading { get { return _isReloading; } }
    public bool OutOfAmmo { get { return ammosInMag + currentAmmos == 0; } }

    public LayerMask layer;

    private bool _isEquipped;
    public bool IsEquipped { get { return _isEquipped; } set { _isEquipped = value; OnWeaponUpdateAmmo?.Invoke(currentAmmos, ammosInMag); } }
    public bool CanShoot { get { return IsEquipped && !IsReloading && !manager.IsSwitching; } }

    protected AudioSource audioSource;
    protected Vector3 shootDirection;

    //Events
    public static event Action<float> OnReload;
    public static event Action<int, int> OnWeaponUpdateAmmo;    //Update ammo count to display to canvas

    private void OnEnable()
    {
        OnWeaponUpdateAmmo?.Invoke(currentAmmos, ammosInMag);
    }

    private void Awake()
    {
        _playerInput = new PlayerInputActions();
        _playerInput.Player.Enable();

        manager = FindObjectOfType<WeaponManager>();
        audioSource = GetComponent<AudioSource>();

        currentAmmos = weaponData.magazineCapacity;
        ammosInMag = weaponData.magazineCapacity;

        //ammosInMag = int.MaxValue;
    }

    private void Start()
    {
        OnWeaponUpdateAmmo?.Invoke(currentAmmos, ammosInMag);
    }

    public void StartShoot(Vector3 direction, float range, float fireRate, float aimAssist)
    {
        if (Time.time >= _lastShootTime && CanShoot)
        {
            if (!OutOfAmmo)
            {
                //Shoot periodically according to the fire rate
                _lastShootTime = Time.time + 1f / fireRate;
                Shoot(direction, range, aimAssist);

                if (transform.parent.tag == "Player")
                    CalculateAmmo();
            }
            else
            {
                PlaySound(weaponData.emptySfx);
            }
        }
    }

    public virtual void Shoot(Vector3 direction, float range, float aimAssist)
    {
        shootDirection = direction * range;
        PlaySound(weaponData.shotSfx);
    }

    /// <summary>
    /// Calculate the ammo count of the weapon and shoot or reload if possible
    /// </summary>
    public void CalculateAmmo()
    {
        if (ammosInMag == 0)
        {
            if (currentAmmos > 0)
            {
                StartCoroutine(Reload(weaponData.reloadTime));

                if (currentAmmos >= weaponData.magazineCapacity)
                {
                    currentAmmos -= weaponData.magazineCapacity;
                    ammosInMag += weaponData.magazineCapacity;
                }
                else
                {
                    ammosInMag += currentAmmos;
                    currentAmmos -= currentAmmos;
                }
            }
        }
        else
        {
            ammosInMag--;
        }

        OnWeaponUpdateAmmo?.Invoke(currentAmmos, ammosInMag);
    }

    protected void DamageEnemy(RaycastHit hitInfo)
    {
        //If the object hit is an enemy
        //if (hitInfo.transform.root.GetComponent<Enemy>() || hitInfo.transform.root.GetComponent<Player>())
        if (hitInfo.transform.root.GetComponent<Character>())
        {
            hitInfo.transform.root.GetComponent<Character>().TakeDamage(weaponData.damage);  //Enemy takes damage
            hitInfo.transform.root.GetComponent<FloatingDamageText>().SpawnText(weaponData.damage);
        }
    }

    /// <summary>
    /// Set the specified audio clip to the audio source and play it
    /// </summary>
    /// <param name="clip">Clip to play</param>
    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    /// <summary>
    /// Spawn the muzzle particule effect
    /// </summary>
    /// <param name="data">The weapon data that stores the VFX prefab</param>
    /// <param name="startPoint">The point to spawn the effect</param>
    public void MuzzleEffect(WeaponData data, Vector3 startPoint)
    {
        GameObject vfx = Instantiate(data.fireVFX, startPoint, Quaternion.identity);  //Instnatiate the particules effets on the cannon
        Destroy(vfx, 0.5f); //Destroying the cannon VFX
    }
    
    /// <summary>
    /// Spawns and moves the trail to the point hit (or the weapon range) and spawns the impact effect
    /// </summary>
    /// <param name="data">Weapon data that stores the VFXs and the range of the weapon</param>
    /// <param name="startPoint">The point where the trail start</param>
    /// <param name="hitPoint">The point hit by the raycast, or forward if nothing is hit</param>
    /// <param name="hitNormal">The normal of the object hit (Vector3.zero if nothing is hit)</param>
    public void TrailEffect(WeaponData data, Vector3 startPoint, Vector3 hitPoint, Vector3 hitNormal)
    {
        TrailRenderer trail = Instantiate(data.trailVFX, startPoint, Quaternion.identity).GetComponent<TrailRenderer>(); //Instantiate the trail particules along the raycast
        StartCoroutine(MoveTrail(data, trail, hitPoint, hitNormal));
    }

    //Move the trail gameobject towards the hit point (or max range if nothing is hit) and instantiate impact VFX
    protected IEnumerator MoveTrail(WeaponData data, TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal)
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
            Destroy(go, 0.5f);
        }

        Destroy(trail.gameObject, trail.time);  //Destroy object upon arriving
    }

    public IEnumerator Reload(float reloadTime)
    {
        OnReload?.Invoke(weaponData.reloadTime);
        _isReloading = true;
        PlaySound(weaponData.reloadSfx);
        yield return new WaitForSecondsRealtime(reloadTime);
        _isReloading = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + shootDirection, weaponData.aimAssist);
    }
}