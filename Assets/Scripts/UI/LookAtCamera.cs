using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
