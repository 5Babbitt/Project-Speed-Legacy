using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPickup : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        CarController car = collision.gameObject.GetComponent<CarController>();
        car.GiveBarrier();
    }
}
