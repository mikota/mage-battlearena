using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private GameObject target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera");   
    }
    void Update()
    {
        transform.LookAt(transform.position + target.transform.forward);
    }
}
