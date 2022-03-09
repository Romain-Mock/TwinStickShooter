using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class LookAtRigging : MonoBehaviour
{
    private Rig rig;
    private float targetWeight;

    private void Awake()
    {
        rig = GetComponent<Rig>();
    }

    private void Update()
    {
        rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * 10f);

        if (Keyboard.current.tKey.isPressed)
            targetWeight = 1f;
        if (!Keyboard.current.tKey.isPressed)
            targetWeight = 0f;
    }
}
