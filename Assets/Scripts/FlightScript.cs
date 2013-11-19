using UnityEngine;
using System.Collections;

public class FlightScript : MonoBehaviour {
	
	public bool railMode = true;
	
	public float translateSpeed = 0.2f;
	public float rotateSpeed = 1.5f;
	public float turnSpeed = 0.5f;
	
	public float movementSpeed = 0.5f;
	
	public float topBorder = 10.0f;
	public float bottomBorder = -10.0f;
	public float leftBorder = 11.0f;
	public float rightBorder = -11.0f;
	
	public bool invertY = true;
	
	public float rollAngle;
	public float pitchAngle;
	
	public float maxRollAngle = 30.0f;
	public float autoRollSpeed = 1.0f;
	
	float angleDeadzone = 1.5f;
	
	public float maxHorizontalAcceleration = 0.6f;
	public float horizontalAcceleration = 0;
	
	public float maxVerticalAcceleration = 0.6f;
	public float verticalAcceleration = 0;
	
	public float verticalInput;
	public float horizontalInput;
	
	// Use this for initialization
	void Start () {
	
	}// Start
	
	// Update is called once per frame
	void Update () {
		
		verticalInput = Input.GetAxis("Vertical");
		horizontalInput = Input.GetAxis("Horizontal");
		
		if( invertY )
			verticalInput *= -1;
		
		// Acceleration for smooth horizontal movement
		if( horizontalInput != 0 ){
			horizontalAcceleration += horizontalInput;
		} else if( horizontalInput != 1 && horizontalAcceleration > translateSpeed ){
			horizontalAcceleration -= Time.deltaTime;
		} else if( horizontalInput != -1 && horizontalAcceleration < -translateSpeed ){
			horizontalAcceleration += Time.deltaTime;
		} else if( horizontalInput != 1  && horizontalInput != -1 ){
			horizontalAcceleration = 0;
		}
		
		// Acceleration for smooth vertical movement
		if( verticalInput != 0 ){
			verticalAcceleration += verticalInput;
		} else if( verticalInput != 1 && verticalAcceleration > translateSpeed ){
			verticalAcceleration -= Time.deltaTime;
		} else if( verticalInput != -1 && verticalAcceleration < -translateSpeed ){
			verticalAcceleration += Time.deltaTime;
		} else if( verticalInput != 1  && verticalInput != -1 ){
			verticalAcceleration = 0;
		}
		
		// Check and bound max horizontal acceleration
		if( horizontalAcceleration > maxHorizontalAcceleration ){
			horizontalAcceleration = maxHorizontalAcceleration;
		}
		if( horizontalAcceleration < -maxHorizontalAcceleration ){
			horizontalAcceleration = -maxHorizontalAcceleration;
		}
		
		// Check and bound max vertical acceleration
		if( verticalAcceleration > maxVerticalAcceleration ){
			verticalAcceleration = maxVerticalAcceleration;
		}
		if( verticalAcceleration < -maxVerticalAcceleration ){
			verticalAcceleration = -maxVerticalAcceleration;
		}
		
		rollAngle = transform.eulerAngles.z;
		pitchAngle = transform.eulerAngles.x;
		
		if( railMode ){
			// Simple ship translation on input (for flight on rails)
			transform.Translate( horizontalAcceleration * translateSpeed, 0, 0, Space.World );
			
			if( pitchAngle > angleDeadzone && pitchAngle < 180 ){
				transform.Translate( 0, -Mathf.Abs(verticalAcceleration * translateSpeed), 0, Space.World );
			}else if( pitchAngle < 360 - angleDeadzone && pitchAngle > 180 ){
				transform.Translate( 0, Mathf.Abs(verticalAcceleration * translateSpeed), 0, Space.World );
			}
			
			// Clamp Y rotation to prevent drifting while moving vert. and horz. at the same time
			transform.eulerAngles = new Vector3( transform.eulerAngles.x, 0, transform.eulerAngles.z );
		} else {
			// Free flight mode
			transform.Translate( 0, 0, movementSpeed );
			transform.Rotate( 0, horizontalAcceleration * turnSpeed, 0, Space.World );
		}
		
		// --- Horizontal Movement ------------------------------------------------------------------------------
		
		// Rotate/roll ship left (-rollAngle) on -horizontal movement
		// AND if rollAngle is within limits ( 360 - 330 ) if maxRollAngle = 30
		if( horizontalInput < 0 && (rollAngle < maxRollAngle || rollAngle > 180 ) ){
			transform.Rotate( 0, 0, -horizontalInput * rotateSpeed );
		}
		
		// Rotate/roll ship right (+rollAngle) on +horizontal movement
		// AND if rollAngle is within limits ( 0 - 30 ) if maxRollAngle = 30
		if( horizontalInput > 0 && (rollAngle < 180 || rollAngle > 360.0 - maxRollAngle) ){
			transform.Rotate( 0, 0, -horizontalInput * rotateSpeed );
		}
			
		// If no input, auto-level ship - horizontal movement
		if( horizontalInput == 0 && rollAngle < 180 && rollAngle > angleDeadzone ){
			// Ship is rotated right (+), auto move back left (-)
			transform.Rotate( 0, 0, -autoRollSpeed );
		}
		if( horizontalInput == 0 && rollAngle > 180 && rollAngle < 360 - angleDeadzone ){
			// Ship is rotated left (-), auto move back right (+)
			transform.Rotate( 0, 0, autoRollSpeed );
		}
		
		// --- Vertical Movement ------------------------------------------------------------------------------
		
		// Rotate/roll ship left (-rollAngle) on -vertical movement
		// AND if pitchAngle is within limits ( 360 - 330 ) if maxRollAngle = 30
		if( verticalInput < 0 && (pitchAngle < 180 || pitchAngle > 360.0 - maxRollAngle) ){
			transform.Rotate( verticalInput * rotateSpeed, 0, 0 );
		}
		
		// Rotate/roll ship right (+rollAngle) on +vertical movement
		// AND if pitchAngle is within limits ( 0 - 30 ) if maxRollAngle = 30
		if( verticalInput > 0 && (pitchAngle < maxRollAngle || pitchAngle > 180 ) ){
			transform.Rotate( verticalInput * rotateSpeed, 0, 0 );
		}
			
		// If no input, auto-level ship - vertical movement
		if( verticalInput == 0 && pitchAngle < 180 && pitchAngle > angleDeadzone ){
			// Ship is rotated right (+), auto move back left (-)
			transform.Rotate( -autoRollSpeed, 0, 0 );
		}
		if( verticalInput == 0 && pitchAngle > 180 && pitchAngle < 360 - angleDeadzone ){
			// Ship is rotated left (-), auto move back right (+)
			transform.Rotate( autoRollSpeed, 0, 0 );
		}
		
	}// Update
}
