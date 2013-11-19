using UnityEngine;
using System.Collections;

public class FighterMovementScript : MonoBehaviour {
	public float horizInput;
	public float vertInput;
	public float rollInput;
	
	public float maxSpeed = 100;
	public float targetSpeed = 100;
	public float forwardSpeed;
	public float acceleration;
	
	public bool fullFlightMode = true;
	
	public bool autoRoll = true;
	public float autoRollTime = 0.5f;
	public float maxRollAngle = 45;
	float rollVelocity;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public float pitch;
	public float yaw;
	public float roll;
	
	// Update is called once per frame
	void Update () {
	
		horizInput = getReal3D.Input.GetAxis("Strafe");
		vertInput = getReal3D.Input.GetAxis("Forward");
		rollInput = Input.GetAxis("Roll");
		
		
		Move();
		
		BroadcastMessage("SetParentSpeed", forwardSpeed / 50.0f, SendMessageOptions.DontRequireReceiver );		
	}
	
	public void Move()
	{
		// Direction
		if( !fullFlightMode )
		{
			pitch += vertInput;
			yaw += horizInput;
			
			if( roll <= maxRollAngle && roll >= -maxRollAngle )
			{
				roll += -horizInput;
			}
			else if( (roll > maxRollAngle) && horizInput > 0 )
			{
				roll += -horizInput;
			}
			else if( (roll < -maxRollAngle) && horizInput < 0 )
			{
				roll += -horizInput;
			}
			
			if( autoRoll && Mathf.Abs(horizInput) != 1 )
			{
				roll = Mathf.SmoothDampAngle( roll, 0, ref rollVelocity, autoRollTime );
			}
			
			transform.localEulerAngles = new Vector3( pitch, yaw, roll );
		}
		else
		{
			transform.Rotate( new Vector3( vertInput, horizInput, -horizInput + rollInput ) );
		}
		
		// Speed
		if( targetSpeed > forwardSpeed )
		{
			forwardSpeed += acceleration * Time.deltaTime;
		}
		else if( targetSpeed < forwardSpeed )
		{
			forwardSpeed -= acceleration * Time.deltaTime;
		}
		
		transform.Translate( Vector3.forward * forwardSpeed * Time.deltaTime );
	}
}
