using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerpTime;

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, targetObject.transform.position);
        transform.position = Vector3.Slerp(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(targetObject.transform.position.x + offset.x, targetObject.transform.position.y + offset.y, targetObject.transform.position.z + offset.z), lerpTime * distance * Time.deltaTime);
    }
}
