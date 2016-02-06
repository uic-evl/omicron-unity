using UnityEngine;
using System.Collections;

public class FighterMovementScript : MonoBehaviour {
	public float horizInput;
	public float vertInput;
	public float rollInput;
	public float accelInput;
	public float decelInput;

	Vector3 inputAcceleration = new Vector3();
	public float inputDecelerationMultiplier = 0.9f;

	public float maxSpeed = 100;
	public float targetSpeed = 100;
	public float forwardSpeed;
	public float acceleration;
	public float maxTurnSpeed = 50f;
	public float turnAcceleration = 1f;

	public Vector3 velocity;
	Vector3 lastPosition;

	public bool autoRoll = false;
	public float autoRollTime = 0.5f;
	public float maxRollAngle = 45;
	float rollVelocity;
		
	float pitch;
	float yaw;
	float roll;

    public void Init()
    {
        lastPosition = transform.position;
    }

	// Update is called once per frame
	void Update () {

	}

	public void Move()
	{
		inputAcceleration.y += horizInput * turnAcceleration;
		inputAcceleration.x += vertInput * turnAcceleration;
		inputAcceleration.z += rollInput * turnAcceleration;

		if( vertInput > 0 && inputAcceleration.x > vertInput )
			inputAcceleration.x = vertInput;
		else if( vertInput < 0 && inputAcceleration.x < vertInput )
			inputAcceleration.x = vertInput;

		if( horizInput > 0 && inputAcceleration.y > horizInput )
			inputAcceleration.y = horizInput;
		else if( horizInput < 0 && inputAcceleration.y < horizInput )
			inputAcceleration.y = horizInput;

		if( inputAcceleration.z > 1 )
			inputAcceleration.z = 1;
		else if( inputAcceleration.z < -1 )
			inputAcceleration.z = -1;

		// Smooth turning deceleration
		if( horizInput == 0 )
		{
			inputAcceleration.y *= inputDecelerationMultiplier;
		}
		if( vertInput == 0 )
		{
			inputAcceleration.x *= inputDecelerationMultiplier;
		}

		// Direction
		pitch += inputAcceleration.y;
		yaw += inputAcceleration.x;

		inputAcceleration.z = 0;

		if( roll <= maxRollAngle && roll >= -maxRollAngle )
		{
			inputAcceleration.z += -horizInput;
			roll += -horizInput;
		}
		else if( (roll > maxRollAngle) && horizInput > 0 )
		{
			inputAcceleration.z += -horizInput;
			roll += -horizInput;
		}
		else if( (roll < -maxRollAngle) && horizInput < 0 )
		{
			inputAcceleration.z += -horizInput;
			roll += -horizInput;
		}

		inputAcceleration.z += -rollInput;
		if( rollInput == 0 )
		{
			inputAcceleration.z *= inputDecelerationMultiplier;
		}

		// Fighter roll rotation
		transform.Rotate( inputAcceleration * Time.deltaTime * maxTurnSpeed );

		// Turret rotation
		//transform.Rotate( new Vector3(0, inputAcceleration.y, 0) * Time.deltaTime * maxTurnSpeed, Space.World );
		//transform.Rotate( new Vector3(inputAcceleration.x, 0, 0) * Time.deltaTime * maxTurnSpeed);

		if( autoRoll && Mathf.Abs(horizInput) != 1 )
		{
			roll = Mathf.SmoothDampAngle( roll, 0, ref rollVelocity, autoRollTime );
			transform.localEulerAngles = new Vector3( transform.localEulerAngles.x, transform.localEulerAngles.y, roll );
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

		velocity = (transform.position - lastPosition) / Time.deltaTime;

		lastPosition = transform.position;
	}
}
