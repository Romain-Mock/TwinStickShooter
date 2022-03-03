using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Transform weaponTransform;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputAction;
    private LineRenderer line;

    Vector3 mouseWorldPos;

    Vector2 aimInput;

    private float xMovement;
    private float zMovement;
    public float moveSpeed = 5f;
    [Range(1, 10)]
    public float rotateSpeed = 6f;
    public bool rotateSmooth;

    public float fireRate = 15f;
    public float delayBetweenShots = 0.19f;
    public float maxShotDistance = 20f;
    private float nextPossibleShootTime = 0f;
    IEnumerator currentCoroutine;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weaponTransform = transform.GetChild(0).transform;
        playerInput = GetComponent<PlayerInput>();
        line = weaponTransform.GetComponent<LineRenderer>();

        playerInputAction = new PlayerInputActions();
        playerInputAction.Player.Enable();

        //playerInputAction.Player.FireMouse.performed += FireMouse;

        //playerInputAction.Player.FireGamepad.performed += FireGamepad;
    }

    private void Start()
    {
        delayBetweenShots = 60 / fireRate;
    }

    void FixedUpdate()
    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
        Vector3 movement = new Vector3(inputVector.x, 0, inputVector.y).normalized;

        Vector2 aimInput = playerInputAction.Player.FireGamepad.ReadValue<Vector2>();
        Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;

        //If the player is moving
        if (movement != Vector3.zero)
        {
            //If the player is aiming and moving
            if (aimDirection != Vector3.zero)
            {
                Rotate(aimDirection);
                Shoot();
            }
            else
            {
                Rotate(movement);
                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);
            }

            rb.position += movement * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            //If the player is aiming without moving
            if (aimDirection != Vector3.zero)
            {
                Rotate(aimDirection);
                Shoot();
            }
            else
                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);
        }

    }

    public void LookMouse(InputAction.CallbackContext context)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(playerInputAction.Player.LookMouse.ReadValue<Vector2>());

        Plane p = new Plane(Vector3.up, Vector3.zero);

        if (p.Raycast(mouseRay, out float hitDist))
        {
            Vector3 hitPoint = mouseRay.GetPoint(hitDist);
            transform.LookAt(new Vector3(hitPoint.x, transform.position.y, hitPoint.z));
        }
    }

    public void FireGamepad(InputAction.CallbackContext context)
    {
        Vector2 aimInput = playerInputAction.Player.FireGamepad.ReadValue<Vector2>();
        Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;
        Rotate(aimDirection);

        Ray ray = new Ray(weaponTransform.position, weaponTransform.forward);
        RaycastHit hitInfo;
        float shotDistance = maxShotDistance;

        if (Physics.Raycast(ray, out hitInfo, maxShotDistance))
        {
            shotDistance = hitInfo.distance;
        }

        if (aimDirection != Vector3.zero)
        {
            StartCoroutine(Fire(delayBetweenShots, shotDistance));
        }
        else
        {
            StopCoroutine(Fire(delayBetweenShots, shotDistance));
        }
    }

    void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            if (rotateSmooth)
                targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * rotateSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(targetRotation);
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(weaponTransform.position, weaponTransform.forward);
        RaycastHit hitInfo;
        float shotDistance = maxShotDistance;

        if (Physics.Raycast(ray, out hitInfo, maxShotDistance))
        {
            shotDistance = hitInfo.distance;
        }

        currentCoroutine = Fire(delayBetweenShots, shotDistance);
        StartCoroutine(currentCoroutine);
    }

    IEnumerator Fire(float delay, float distance)
    {
        line.enabled = true;
        line.SetPosition(0, weaponTransform.position);
        line.SetPosition(1, weaponTransform.position + weaponTransform.forward * distance);
        yield return new WaitForSeconds(delay);
        line.enabled = false;
    }
}
