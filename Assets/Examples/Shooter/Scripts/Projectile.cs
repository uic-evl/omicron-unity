using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float speed = 12;

    public GameObject hitPrefab;

    Transform parent;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root != parent) // Prevent colliding with spawning object
        {
            GameObject explosionEffect = Instantiate(hitPrefab, transform.position, transform.rotation) as GameObject;
            Destroy(explosionEffect, 3); // Destroy effect after 3 seconds
            Destroy(gameObject); // Destroy this object
        }
    }

    public void SetParent(Transform parent)
    {
        this.parent = parent;
    }
}
