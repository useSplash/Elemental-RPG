using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Camera cam;
    private float shakeTimer;
    private float shakeIntensity;
    private Vector3 startPosition;
    private Vector3 offset;

    private void Awake(){
        Instance = this;
        cam = GetComponent<Camera>();
    }

    public void ShakeCamera(float intensity, float timer){
        startPosition = cam.transform.position;
        shakeTimer = timer;
        shakeIntensity = intensity;
    }

    private void Update(){
        if (shakeTimer > 0){
            Vector3 offset = Random.insideUnitSphere * shakeIntensity;
            cam.transform.position = startPosition + new Vector3(offset.x, offset.y, 0);
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0) {
                cam.transform.position = startPosition;
            }
        }

    }
}
