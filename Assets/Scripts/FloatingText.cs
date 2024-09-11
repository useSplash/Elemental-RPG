using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private float destroyTime = 3f;
    private Vector3 offset = new Vector3(0, -1.0f, -1.0f);
    private Vector3 randomOffset = new Vector3(0.5f, 0.5f, 0.0f);

    void Start(){
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
        transform.localPosition += new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z));
    }
}