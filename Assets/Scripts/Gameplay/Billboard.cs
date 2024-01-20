using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Quaternion lastParentRotation;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera");   
        lastParentRotation = transform.parent.rotation;
    }
    void Update()
    {
        var oldangles = transform.eulerAngles;
        transform.LookAt(transform.position + target.transform.forward);
        var newangles = transform.eulerAngles;
        //interpolate
        newangles.x = Mathf.LerpAngle(oldangles.x, newangles.x, 3.0f * Time.deltaTime);
        newangles.y = Mathf.LerpAngle(oldangles.y, newangles.y, 3.0f * Time.deltaTime);
        newangles.z = Mathf.LerpAngle(oldangles.z, newangles.z, 3.0f * Time.deltaTime);
        transform.eulerAngles = newangles;
    }
}
