using UnityEngine;
using System.Collections;

public class Flocking3DScript : MonoBehaviour {
	public GameObject target;

	public float forwardMovementSpeed = 0.0f;
	public float turningSpeed = 1.0f;
	
	public float turnTolerance = 0.001f;
	
	public float destinationDistance = 0;
	public float destinationAngle = 0;
	
	// Use this for initialization
	void Start () {
		if( !target ){
			target = GameObject.Find("Target");
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate( 0, 0, forwardMovementSpeed * Time.deltaTime );
		
		Vector3 destinationPos = target.transform.position;
		Vector3 position = transform.position;
		Vector3 rotation = transform.localEulerAngles;
		
		// Get the Distance to the destination
		destinationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( destinationPos.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( destinationPos.z - position.z ), 2.0f) );
		
		// ---- Rotation code from Fleet Commander ----
		destinationAngle = Mathf.Atan2( destinationPos.y - position.y, destinationPos.z - position.z );
		if( destinationAngle < 0 ){
	      destinationAngle = 2*Mathf.PI + Mathf.Atan2( destinationPos.y - position.y, destinationPos.z - position.z );
	    }

		destinationAngle = 360 - (destinationAngle * Mathf.Rad2Deg);
		
		// Rotate toward destination by turning in the shorter direction
		if( destinationAngle - rotation.x > 180 ){
			transform.Rotate( turningSpeed * Time.deltaTime, 0, 0 );
		} else if( destinationAngle - rotation.x < -180 ){
			transform.Rotate( -turningSpeed * Time.deltaTime, 0, 0 );
		}
		
		// Simple turning not accounting 359-0 degree border (corrected using if-else above)
		else if( rotation.x > destinationAngle ){
			transform.Rotate( turningSpeed * Time.deltaTime, 0, 0 );
		} else if( rotation.x < destinationAngle ){
			transform.Rotate( -turningSpeed * Time.deltaTime, 0, 0 );
		}
		// --------------------------------------------
		
	}
	
	void OnTriggerExit( Collider other ){
	}
}
