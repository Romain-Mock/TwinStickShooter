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
    private Animator animator;

    Vector3 mouseWorldPos;

    Vector2 aimInput;

    private float xMovement;
    private float zMovement;
    public float moveSpeed = 5f;
    [Range(1, 10)]
    public float rotateSpeed = 6f;
    public bool rotateSmooth;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weapon = transform.GetComponentInChildren<Weapon>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        playerInputAction = new PlayerInputActions();
        playerInputAction.Player.Enable();
    }

    void FixedUpdate()
    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();   //Movement input (ZQSD / Left stick)
        Vector3 movement = new Vector3(inputVector.x, 0, inputVector.y).normalized; //Movement normalized in 3D

        Vector2 aimInput = playerInputAction.Player.FireGamepad.ReadValue<Vector2>();   //Aiming input (Mouse / Right stick)
        Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;   //Aiming normalized in 3D

        //If the player is moving
        if (movement != Vector3.zero)
        {
            //If the player is aiming while moving
            if (aimDirection != Vector3.zero)
            {
                Rotate(aimDirection);  

                float normalizedAngle = Vector3.Angle(movement, aimDirection) / 180;    //Calaculate the angle between movement and aim and normalize it from 0 to 1

                animator.SetBool("isShooting", true);   //Animation conditions
                animator.SetFloat("angle", normalizedAngle);

                weapon.Shoot();
                
            }
            else
            {
                Rotate(movement);

                animator.SetBool("isShooting", false);

                weapon.StopShooting();
            }

            animator.SetBool("isMoving", true);
            //Movement
            rb.position += movement * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            animator.SetBool("isMoving", false);
            //If the player is aiming without moving
            if (aimDirection != Vector3.zero)
            {
                Rotate(aimDirection);

                animator.SetBool("isShooting", true);

                weapon.Shoot();
            }
            else
            {
                weapon.StopShooting();

                animator.SetBool("isShooting", false);
            }
        }
    }

    //Rotate the player to look at the mouse
    public void LookMouse(InputAction.CallbackContext context)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(playerInputAction.Player.LookMouse.ReadValue<Vector2>());   //Tranform the mouse pos in screen value to world value

        Plane p = new Plane(Vector3.up, Vector3.zero);  //Create a plane for the mouse raycast to hit

        //If the raycast hit the plane
        if (p.Raycast(mouseRay, out float hitDist))
        {
            Vector3 hitPoint = mouseRay.GetPoint(hitDist); //Locate the mouse
            transform.LookAt(new Vector3(hitPoint.x, transform.position.y, hitPoint.z)); //Rotate the player
        }
    }

    //Rotate the player to a given vector
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
