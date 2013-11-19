using UnityEngine;
using System.Collections;

public class CameraCollisionScript : MonoBehaviour {
	public Vector3 collisionNormal;
	public bool colliding = false;
	
	// Use this for initialization
	void Start () {
		SphereCollider collider = gameObject.AddComponent("SphereCollider") as SphereCollider;
		collider.isTrigger = false;
		collider.radius = 0.1f;
		
		Rigidbody rb = gameObject.AddComponent("Rigidbody") as Rigidbody;
		rb.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter( Collision collision )
	{
		ContactPoint contact = collision.contacts[0];
		collisionNormal = contact.normal;
		
		colliding = true;
	}
	
	void OnCollisionExit( Collision other )
	{
		collisionNormal = Vector3.zero;
		colliding = false;
	}
}
