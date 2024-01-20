using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public CameraMultiTarget cameraMultiTarget;
    void FixedUpdate()
    {
        cameraMultiTarget.SetTargets(GameObject.FindGameObjectsWithTag("Player"));
    }
}
