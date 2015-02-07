using UnityEngine;
using System.Collections;

public class GravityField : MonoBehaviour {

	public float force = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( force > 250 )
		{
			Explode();
		}
		else if( force > 20 )
			force -= Time.deltaTime * 10;
	}

	void OnTriggerStay( Collider other )
	{
		other.rigidbody.AddForce( (transform.position - other.transform.position) * force );
	}

	void OnWandButtonHold()
	{
		force++;
	}

	void Explode()
	{
		Destroy( gameObject );
	}
}
