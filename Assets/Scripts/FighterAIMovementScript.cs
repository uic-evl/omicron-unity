using UnityEngine;
using System.Collections;

public class FighterAIMovementScript : FighterMovementScript {

	public float targetYaw;
	public float turnSpeed = 1;
	
	public Vector3 destinationPos;
	
	public GameObject target;
	public float destinationDistance;
	
	public float destinationYaw;

	public float flockingYawForce;
	
	public float yawDiff;
	
	// Use this for initialization
	void Start () {
		fullFlightMode = false;
	}
	
	
	// Update is called once per frame
	void Update () {
		if( target == null )
		{
			if( destinationDistance < 50 )
				destinationPos = new Vector3( Random.Range( -500, 500) , 50, Random.Range( -500, 500) );
		}
		else
			destinationPos = target.transform.position;
		
		MoveToDestination();
		
		BroadcastMessage("SetParentSpeed", forwardSpeed / 50.0f, SendMessageOptions.DontRequireReceiver );
		
		if( transform.childCount == 0 )
			Destroy(gameObject);
	}
	
	void Kill()
	{
		BroadcastMessage("DestroyAll", SendMessageOptions.DontRequireReceiver);
		Destroy(gameObject);
	}
	
	public bool yawFlag;
	
	public void MoveToDestination()
	{
		Vector3 position = transform.position;
		Vector3 rotation = transform.localEulerAngles;
		
		// Get the Distance to the destination
		destinationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( destinationPos.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( destinationPos.z - position.z ), 2.0f) );
		
		// ---- Rotation code from Fleet Commander ----
		destinationYaw = Mathf.Atan2( destinationPos.x - position.x, destinationPos.z - position.z );
		if( destinationYaw < 0 ){
	      destinationYaw = 2*Mathf.PI + Mathf.Atan2( destinationPos.x - position.x, destinationPos.z - position.z );
	    }

		destinationYaw *= Mathf.Rad2Deg;
		
		yawDiff = destinationYaw - rotation.y;
		float turnSpeedMod = turnSpeed;
		if( Mathf.Abs(yawDiff) < 10 )
		{
			turnSpeedMod = 10;
			yawFlag = true;
		}
		else
		{
			yawFlag = false;
		}
		
		float aimDistance = 11;
		if( target != null )
		{
			Vector3 aimPosition = target.transform.localPosition + new Vector3( aimDistance, 0, 0 );
			aimPosition = destinationPos;
			Debug.DrawLine( position, aimPosition, Color.white);
		}
		
		// Rotate toward destination by turning in the shorter direction
		if( yawDiff > 180 ){
			horizInput += -turnSpeedMod * Time.deltaTime;
		} else if( yawDiff < -180 ){
			horizInput += turnSpeedMod * Time.deltaTime;
		}
		
		// Simple turning not accounting 359-0 degree border (corrected using if-else above)
		else if( rotation.y - 10 > destinationYaw ){
			horizInput += -turnSpeedMod * Time.deltaTime;
		} else if( rotation.y + 10 < destinationYaw ){
			horizInput += turnSpeedMod * Time.deltaTime;
		}
		
		//horizInput += flockingYawForce;
		horizInput = Mathf.Clamp( horizInput, -1, 1 );
		//horizInput = Input.GetAxis("Horizontal");
		//vertInput = Input.GetAxis("Vertical");
		//rollInput = Input.GetAxis("Roll");
		
		// Speed
		if( targetSpeed > forwardSpeed )
		{
			forwardSpeed += acceleration * Time.deltaTime;
		}
		else if( targetSpeed < forwardSpeed )
		{
			forwardSpeed -= acceleration * Time.deltaTime;
		}
		
		Move();
	}
	
	public void OnTriggerEnter( Collider other )
	{
		Vector3 position = transform.position;
		Vector3 otherPos = other.transform.position;
		
		if( other.CompareTag(tag) )
		{
			FighterAIMovementScript otherAI = other.GetComponent<FighterAIMovementScript>();
			
			if( otherAI != null )
			{
				float separationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( otherPos.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( otherPos.z - position.z ), 2.0f) );
				
				if( separationDistance < 3.2f )
				{
					otherAI.Kill();
					Kill();
				}
			}
			
		}
		else
		{
			float otherYaw = Mathf.Atan2( otherPos.x - position.x, otherPos.z - position.z );
			if( otherYaw < 0 )
				targetSpeed = maxSpeed / 2;
		}
	}
	
	public void OnTriggerStay( Collider other )
	{
		if( other.CompareTag(tag) )
		{
			FighterAIMovementScript otherAI = other.GetComponent<FighterAIMovementScript>();
			
			if( otherAI != null )
			{
				Vector3 position = transform.position;
				Vector3 otherPos = other.transform.position;
				float otherYaw = Mathf.Atan2( otherPos.x - position.x, otherPos.z - position.z );
				
				float flockingSeparationForce = 1.1f;
				float separationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( otherPos.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( otherPos.z - position.z ), 2.0f) );
				
				flockingSeparationForce = 0.5f;
				
				flockingYawForce = flockingSeparationForce;
				otherAI.flockingYawForce = -flockingSeparationForce;
				
				//if( otherYaw > 0 ) // Wingman is in front, slow down
				//	targetSpeed = maxSpeed / 2;
			}
		}
	}
	
	public void OnTriggerExit( Collider other )
	{
		if( other.CompareTag(tag) )
		{
			flockingYawForce = 0;
		}
		else
			targetSpeed = maxSpeed;
	}
	
}
