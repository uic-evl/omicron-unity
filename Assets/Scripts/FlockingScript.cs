using UnityEngine;
using System.Collections;

public class FlockingScript : MonoBehaviour {
	public GameObject target;

	public float forwardMovementSpeed = 0.0f;
	public float turningSpeed = 1.0f;
	
	public float turnTolerance = 0.001f;
	
	public float destinationDistance = 0;
	public float destinationAngle = 0;
	
	public FlockingSeparationScript separationScript;
	
	// Use this for initialization
	void Start () {
		separationScript = gameObject.GetComponentInChildren<FlockingSeparationScript>();
		
		if( !target ){
			target = GameObject.Find("Target");
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate( 0, 0, forwardMovementSpeed * Time.deltaTime );
		
		Vector3 destinationPos = Vector3.zero;
		
		if( target )
			destinationPos = target.transform.position;
		Vector3 position = transform.position;
		Vector3 rotation = transform.localEulerAngles;
		
		// Final Movement Vector ---------------------------------------------
		Vector3 movementVector = new Vector3();
		
		if( separationScript )
		{
			movementVector += destinationPos;
			movementVector += separationScript.GetSeparationVector();
		} else {
			//movementVector += cohesionVector;
			movementVector = destinationPos;
		}
		
		// -------------------------------------------------------------------
		
		// Get the Distance to the destination
		destinationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( movementVector.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( movementVector.z - position.z ), 2.0f) );
		
		// ---- Rotation code from Fleet Commander ----
		destinationAngle = Mathf.Atan2( movementVector.x - position.x, movementVector.z - position.z );
		if( destinationAngle < 0 ){
	      destinationAngle = 2*3.14159265f + Mathf.Atan2( movementVector.x - position.x, movementVector.z - position.z );
	    }

		destinationAngle *= Mathf.Rad2Deg;
		
		// Rotate toward destination by turning in the shorter direction
		if( destinationAngle - rotation.y > 180 ){
			transform.Rotate( 0, -turningSpeed * Time.deltaTime, 0 );
		} else if( destinationAngle - rotation.y < -180 ){
			transform.Rotate( 0, turningSpeed * Time.deltaTime, 0 );
		}
		
		// Simple turning not accounting 359-0 degree border (corrected using if-else above)
		else if( rotation.y > destinationAngle ){
			transform.Rotate( 0, -turningSpeed * Time.deltaTime, 0 );
		} else if( rotation.y < destinationAngle ){
			transform.Rotate( 0, turningSpeed * Time.deltaTime, 0 );
		}
		// --------------------------------------------
		
	}
	
	void OnTriggerExit( Collider other ){
	}
}
