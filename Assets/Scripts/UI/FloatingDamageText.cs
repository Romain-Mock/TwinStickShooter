using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    public GameObject textObject;
    public float height = 5f;
    public float random = 2f;

    public void SpawnText(float value)
    {
        GameObject clone = Instantiate(textObject, RandomPos(), Quaternion.identity);
        clone.GetComponent<TextMeshPro>().text = value.ToString();
        Destroy(clone, 1f);
    }

    private Vector3 RandomPos()
    {
        Vector3 newPos = transform.position + Vector3.up * height;
        newPos += new Vector3(Random.Range(-random, random), Random.Range(0, random), 0);

        return newPos;
    }
}
