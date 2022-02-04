using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed = 1f;
    public Vector3 rotateAxis = Vector3.up;


    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += rotateAxis * rotateSpeed * Time.deltaTime;
    }
}
