using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Make the camera follow the target at a certain distance and height
public class CameraFollow : MonoBehaviour
{
    [Tooltip("Ce que la camera regarde")]
    public Transform target;
    [Tooltip("Hauteur de la camera")]
    public float height;
    [Tooltip("Distance entre la camera et la cible")]
    public float distance;

    private void Update()
    {
        transform.position = target.position + new Vector3(0, height, distance);
    }

    private void LateUpdate()
    {
        transform.LookAt(target);
    }
}
