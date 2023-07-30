using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    public float angle;
    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, Vector3.right, angle * Time.deltaTime);
        transform.LookAt(Vector3.zero);
    }
}
