using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    public float speed;
    public float longevity;
    public float damage;

    void Start()
    {
        Debug.Log("Projectile Started in layer " + gameObject.layer);
        Physics.IgnoreCollision(GetComponentInChildren<Collider>(), transform.parent.GetComponentInChildren<Collider>());
        gameObject.transform.SetParent(null);
        Destroy(gameObject, longevity);
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter " + other.name + "\nWith Parent " + other.transform.parent.transform.parent.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision! " + collision.gameObject.name + " and " + name);
    }
}
