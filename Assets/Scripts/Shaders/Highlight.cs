using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Highlight : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;
    private Material mat;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        mat = _renderer.sharedMaterial;
    }

    private void Update()
    {
        //if (transform.parent.GetComponent<Weapon>().isEquipped)
    }

    public void ApplyGlow(int onOff)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("ApplyGlow", onOff);
        _renderer.SetPropertyBlock(_mpb);
    }
}
