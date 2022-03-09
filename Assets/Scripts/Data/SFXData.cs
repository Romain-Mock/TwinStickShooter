using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SFXData : ScriptableObject
{
    public AudioClip audioClip;
    [Range(0, 256)]
    public int priority = 128;
    [Range(0, 1)]
    public float volume = 0.5f;
    [Range(-3, 3)]
    public float pitch = 1;
}
