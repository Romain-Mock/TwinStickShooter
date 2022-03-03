using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Weapon weapon;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputAction;

    Vector3 mouseWorldPos;

    Vector2 aimInput;

    private float xMovement;
    private float zMovement;
    public float moveSpeed = 5f;
    [Range(1, 10)]
    public float rotateSpeed = 6f;
    public bool rotateSmooth;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weapon = transform.GetChild(0).GetComponent<Weapon>();
        playerInput = GetComponent<PlayerInput>();

        playerInputAction = new PlayerInputActions();
        playerInputAction.Player.Enable();

        //playerInputAction.Player.FireMouse.performed += FireMouse;

        //playerInputAction.Player.FireGamepad.performed += FireGamepad;
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
            //If the player is aiming while moving
            if (aimDirection != Vector3.zero)
            {
                Rotate(aimDirection);
                weapon.Shoot();
            }
            else
            {
                Rotate(movement);
                weapon.StopShooting();
            }

            rb.position += movement * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            //If the player is aiming without moving
            if (aimDirection != Vector3.zero)
            {
                Rotate(aimDirection);
                weapon.Shoot();
            }
            else
                weapon.StopShooting();
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
}
