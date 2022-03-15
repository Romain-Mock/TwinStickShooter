using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour
{
    private bool _isRolling;
    public bool IsRolling { get { return _isRolling; } }
    private PlayerController _controller;

    [Tooltip("Roll speed")]
    public float rollSpeed = 10f;

    public void StartRoll()
    {
        _isRolling = true;
    }

    public void EndRoll()
    {
        _isRolling = false;
    }
}
