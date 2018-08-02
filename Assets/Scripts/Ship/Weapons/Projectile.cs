using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    public float speed;
    public float longevity;
    public float damage;

    RaycastHit hit;
    private Ray ray;

    private void Awake()
    {
        ray = new Ray(transform.position, Vector3.forward);
        Physics.IgnoreCollision(GetComponent<Collider>(), transform.parent.GetComponentInChildren<Collider>());
        gameObject.transform.SetParent(null);
        Destroy(gameObject, longevity);
    }

    void Start()
    {
        Debug.Log("Projectile Started in layer " + gameObject.layer);
    }

    // Update is called once per frame
    void Update () {
	}

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Debug.DrawRay(transform.position, transform.forward, Color.red, 100f);

        if (Physics.Raycast(ray, out hit, 20f)) {
            Debug.Log("Raycast hit " + hit.collider.gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter " + other.name + "\nWith Parent " + other.transform.parent.transform.parent.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile Hit " + collision.gameObject.name);
        Destroy(gameObject);
    }
}
