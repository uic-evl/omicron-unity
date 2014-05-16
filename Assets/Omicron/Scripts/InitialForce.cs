using UnityEngine;
using System.Collections;

public class InitialForce : MonoBehaviour {
	public Vector3 intialForce;

	// Use this for initialization
	void Start () {
		rigidbody.AddForce( intialForce );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
