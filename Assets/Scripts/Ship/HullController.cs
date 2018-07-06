using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullController : MonoBehaviour {

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        if (rb.angularVelocity != Vector3.zero)
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime);

        if (rb.velocity != Vector3.zero)
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime);

        if (rb.angularVelocity.magnitude < 0.01f)
            rb.angularVelocity = Vector3.zero;

        if (rb.velocity.magnitude < 0.01f)
            rb.velocity = Vector3.zero;
        
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision! " + collision.relativeVelocity);
    }
}
