using UnityEngine;
using System.Collections;

public class FighterAIMovementScript : FighterMovementScript {

	public float currentYaw;
	public float targetYaw;
	public float currentPitch;
	public float targetPitch;

	public float turnSpeed = 1;
	
	public Vector3 destinationPos;
	
	public GameObject target;
	public float destinationDistance;
	
	public float destinationYaw;

	ArrayList flock = new ArrayList();
	public bool enableFlocking = true;
	public float flockingSeparationDistance = 10;
	public float flockingSeparationForce = 1;
	public float flockingYawForce;
	public float flockingPitchForce;

	public float yawDiff;
	public float pitchDiff;
	
	public bool enableStrafing = true;
	public bool strafing = false;
	public int strafingDistance = 600;
	public float strafingTimeLimit = 30;
	public float strafingTimer = 0;

	public float collisionDistance = 4;

	public bool targetByTag = false;
	public string targetTag;


	// 'Genetic' tweaking variables
	public bool enableRandomMods = true;
	public float turningAngleMod;
	public float turningSpeedMod;
	public float strafingMod;

    public bool followingWaypoint = false;

	// Use this for initialization
	void Start() {
		if( enableRandomMods )
		{
			turningAngleMod = Random.Range(-50,50) / 100.0f;
			strafingMod = Random.Range(-250,150);
			turningSpeedMod = Random.Range(-20,20) / 10.0f;
		}
	}

	// Update is called once per frame
	void Update () {

		if( target == null )
		{
			destinationPos = transform.position + new Vector3(0,0,100);

			if( targetByTag )
			{
				GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
				if( targets.Length > 0 )
				{
					target = targets[(int)Random.Range(0, targets.Length)].transform.root.gameObject;
				}
			}
		}
		else
		{
			destinationPos = target.transform.position;
		}

		MoveToDestination();

        if (followingWaypoint)
        {
            if (destinationDistance < 15)
            {
                target = null;
            }
        }
		Move();
	}
	
	void Kill()
	{

	}
	
	public bool yawFlag;
	
	public void MoveToDestination()
	{
		Vector3 position = transform.position;
		Vector3 rotation = transform.localEulerAngles;

		currentYaw = rotation.y * Mathf.Deg2Rad;
		currentPitch = rotation.x * Mathf.Deg2Rad;

		destinationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( destinationPos.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( destinationPos.z - position.z ), 2.0f) );

		Vector3 relativePos = destinationPos - position;
		Quaternion targetRotation = Quaternion.LookRotation(relativePos);

		Quaternion rotationDiff = Quaternion.Inverse(targetRotation) * transform.rotation;
		Vector3 eulerDiff = rotationDiff.eulerAngles;

		// Convert difference to left= -1 to -179, right= +1 to +179
		if( eulerDiff.y >= 180 )
			eulerDiff.y = -(180 - (eulerDiff.y - 180));
		if( eulerDiff.x >= 180 )
			eulerDiff.x = -(180 - (eulerDiff.x - 180));

		yawDiff = -eulerDiff.y * Mathf.Deg2Rad;
		pitchDiff = -eulerDiff.x * Mathf.Deg2Rad;

		if( enableStrafing && target ){
			if( destinationDistance < strafingDistance + strafingMod && Mathf.Abs(yawDiff) > Mathf.PI/2 )
				strafing = true;
			else if( strafing && destinationDistance > strafingDistance + strafingMod)
				strafing = false;

			if( maxSpeed > forwardSpeed )
			{
				forwardSpeed += acceleration * Time.deltaTime;
			}
			horizInput = 0;

			strafingTimer += Time.deltaTime;

			if( strafingTimer > strafingTimeLimit )
				strafing = false;
		}
		if( !strafing )
		{
			strafingTimer = 0;

			// Slow down for sharper turns
			if( Mathf.Abs (yawDiff) > Mathf.PI/2 + turningAngleMod || Mathf.Abs (pitchDiff) > Mathf.PI/2 + turningAngleMod )
				targetSpeed = maxSpeed * 0.33f;
			else
				targetSpeed = maxSpeed;

			// horizInput: +right, -left

			// Turn toward the target
			if( Mathf.Abs(yawDiff) < Mathf.PI )
			{
				horizInput = yawDiff * (turnSpeed + turningSpeedMod);
			}
			else
			{
				// Flip direction when the diff sign flips
				// crossing the 359-0 degree border
				horizInput = -yawDiff * (turnSpeed + turningSpeedMod);
			}
			if( Mathf.Abs(pitchDiff) < Mathf.PI )
			{
				vertInput = pitchDiff * (turnSpeed + turningSpeedMod);
			}
			else
			{
				// Flip direction when the diff sign flips
				// crossing the 359-0 degree border
				vertInput = -pitchDiff * (turnSpeed + turningSpeedMod);
			}

			if( enableFlocking )
				ProcessFlocking();

			horizInput = Mathf.Clamp( horizInput, -1, 1 );
			vertInput = Mathf.Clamp( vertInput, -1, 1 );

			// Speed
			if( targetSpeed > forwardSpeed )
			{
				forwardSpeed += acceleration * Time.deltaTime;
			}
			else if( targetSpeed < forwardSpeed )
			{
				forwardSpeed -= acceleration * Time.deltaTime;
			}
		}
	}

	public void OnTriggerEnter( Collider other )
	{
		if( enableFlocking && other.transform.root && other.transform.root != transform )
		{
			FighterAIMovementScript otherAI = other.transform.root.GetComponent<FighterAIMovementScript>();
			if( otherAI )
			{
				Vector3 position = transform.position;
				Vector3 otherPos = otherAI.transform.position;
				float separationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( otherPos.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( otherPos.z - position.z ), 2.0f) );

				//Debug.Log(gameObject.name + " onTriggerEnter by " + other.transform.parent );
				// Add to flock if:
				//	Separation distance is met
				//	Other is not already in flock
				//	this is not listed in other's flock (only one in the pairr will calculate flocking)
				if( !flock.Contains( otherAI ) && !otherAI.flock.Contains( this ) && separationDistance <= flockingSeparationDistance )
					flock.Add(otherAI);
			}
		}
		else
		{
			//Debug.Log(gameObject.name + " onTriggerEnter (self) by " + other.name );
		}
	}

	public int flockSize;

	public void ProcessFlocking()
	{
		flockSize = flock.Count;

		ArrayList activeFlock = new ArrayList();
		foreach( FighterAIMovementScript otherAI in flock )
		{
			if( otherAI )
			{
				Vector3 position = transform.position;
				Vector3 otherPos = otherAI.transform.position;

				Vector3 relativePos = otherPos - position;
				Quaternion targetRotation = Quaternion.LookRotation(relativePos);
				
				Quaternion rotationDiff = Quaternion.Inverse(targetRotation) * transform.rotation;
				Vector3 eulerDiff = rotationDiff.eulerAngles;

				// Convert difference to left= -1 to -179, right= +1 to +179
				if( eulerDiff.y >= 180 )
					eulerDiff.y = -(180 - (eulerDiff.y - 180));
				if( eulerDiff.x >= 180 )
					eulerDiff.x = -(180 - (eulerDiff.x - 180));

				// Brake if other directly ahead
				if( Mathf.Abs(eulerDiff.y) > 90 )
				{
					otherAI.targetSpeed = otherAI.maxSpeed * 0.5f;
				}
				else
				{
					targetSpeed = maxSpeed * 0.5f;
				}

				float yawSepDiff = -eulerDiff.y * Mathf.Deg2Rad;
				float pitchSepDiff = -eulerDiff.x * Mathf.Deg2Rad;
				
				flockingYawForce += yawSepDiff * flockingSeparationForce;
				otherAI.flockingYawForce += -yawSepDiff * flockingSeparationForce;
				
				flockingPitchForce += pitchSepDiff * flockingSeparationForce;
				otherAI.flockingPitchForce += -pitchSepDiff * flockingSeparationForce;

				activeFlock.Add(otherAI);
			}
		}
		flock = activeFlock;

		horizInput -= flockingYawForce;
		vertInput -= flockingPitchForce;
		flockingYawForce = 0;
		flockingPitchForce = 0;
	}

	/*
	public float separationDistance;
	public void OnTriggerStay( Collider other )
	{
		if( other.transform.parent && other.transform.parent != transform )
		{
			FighterAIMovementScript otherAI = other.transform.parent.GetComponent<FighterAIMovementScript>();

			if( otherAI != null )
			{
				Vector3 position = transform.position;
				Vector3 otherPos = other.transform.position;
				//float otherYaw = Mathf.Atan2( otherPos.x - position.x, otherPos.z - position.z );

				separationDistance = Mathf.Sqrt( Mathf.Pow( Mathf.Abs( otherPos.x - position.x ), 2.0f) + Mathf.Pow( Mathf.Abs( otherPos.z - position.z ), 2.0f) );
				//if( otherYaw > 0 ) // Wingman is in front, slow down
					//	targetSpeed = maxSpeed / 2;
			}
		}
	}
	*/

	public void OnTriggerExit( Collider other )
	{
		if( enableFlocking && other.transform.parent && other.transform.parent != transform )
		{
			if( other.transform.parent.GetComponent<FighterAIMovementScript>() )
			{
				//Debug.Log(gameObject.name + " onTriggerExit by " + other.transform.parent );
				flock.Remove(other.transform.parent.GetComponent<FighterAIMovementScript>());
			}
		}
	}

    public bool enableCollisionEvasion = true;
    Ray collisionDetectorUp;
    Ray collisionDetectorDown;
    Ray collisionDetectorLeft;
    Ray collisionDetectorRight;

    public float detectionWidth;
    public float detectionWidthSpeed = 0.1f;
    public float detectionDistance = 1000;
    public bool collisionDetected = false;
    public bool evasionVectorFound = false;
    public Vector3 collisionEvadeDirection = Vector3.zero;
    Vector3 collisionPoint;

    public Vector3[] lastCollisionPoints = new Vector3[4];
    public Vector3 evadeWaypointPosition;
    public GameObject evadeWaypoint;
    public GameObject lastTarget;
    RaycastHit hitInfo;

    void CollisionDetection()
    {
        if (!evadeWaypoint)
        {
            evadeWaypoint = new GameObject(gameObject.name+" evade waypoint");
        }

        collisionDetectorRight = new Ray(transform.position, transform.forward + Vector3.right * detectionWidth);
        collisionDetectorLeft = new Ray(transform.position, transform.forward + Vector3.left * detectionWidth);
        collisionDetectorUp = new Ray(transform.position, transform.forward + Vector3.up * detectionWidth);
        collisionDetectorDown = new Ray(transform.position, transform.forward + Vector3.down * detectionWidth);

        CollisionEvasionRayCast();

        if (collisionDetected)
        {
            detectionWidth += Time.deltaTime * detectionWidthSpeed;

            if( detectionWidth > 1.5f )
                detectionWidth = 0;
        }
        else
        {

        }

        if (evasionVectorFound)
        {
            if (evadeWaypoint)
            {
                evadeWaypoint.transform.position = evadeWaypointPosition;
                evadeWaypoint.SetActive(true); 
            }
            if (showCollisionEvasionDebug)
                Debug.DrawLine(transform.position, evadeWaypointPosition, Color.yellow);
            float distToEvadePoint = Vector3.Distance(transform.position, evadeWaypointPosition);

            Vector3 relativePos = evadeWaypointPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(relativePos);

            Quaternion rotationDiff = Quaternion.Inverse(targetRotation) * transform.rotation;
            Vector3 eulerDiff = rotationDiff.eulerAngles;

            // Disband evasion point reached point or if point is behind fighter
            if (distToEvadePoint < 10 || Mathf.Abs(eulerDiff.x) > 90 || Mathf.Abs(eulerDiff.y) > 90)
            {
                target = lastTarget;
                evasionVectorFound = false;
                collisionDetected = false;
                detectionWidth = 0;
            }
        }
        else
        {
            if( evadeWaypoint )
                evadeWaypoint.SetActive(false);
        }

    }


    public bool showCollisionEvasionDebug = false;

    void CollisionEvasionRayCast()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(collisionDetectorRight, out hitInfo, detectionDistance))
        {
            if(showCollisionEvasionDebug)
                Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            collisionDetected = true;
            collisionPoint = hitInfo.point;
            lastCollisionPoints[1] = collisionPoint;
        }
        else if (collisionDetected && !evasionVectorFound)
        {
            evasionVectorFound = true;
            evadeWaypointPosition = lastCollisionPoints[1] + Vector3.right * 50;
            lastTarget = target;
            target = evadeWaypoint;
        }
        if (Physics.Raycast(collisionDetectorLeft, out hitInfo, detectionDistance))
        {
            if (showCollisionEvasionDebug)
                Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            collisionDetected = true;
            collisionPoint = hitInfo.point;
            lastCollisionPoints[0] = collisionPoint;
        }
        else if (collisionDetected && !evasionVectorFound)
        {
            collisionEvadeDirection = Vector3.left;
            evasionVectorFound = true;

            evadeWaypointPosition = lastCollisionPoints[0] + Vector3.left * 50;
            lastTarget = target;
            target = evadeWaypoint;
        }

        if (Physics.Raycast(collisionDetectorUp, out hitInfo, detectionDistance))
        {
            if (showCollisionEvasionDebug)
                Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            collisionDetected = true;
            collisionPoint = hitInfo.point;
            lastCollisionPoints[2] = collisionPoint;
        }
        else if (collisionDetected && !evasionVectorFound)
        {
            collisionEvadeDirection = Vector3.up;
            evasionVectorFound = true;

            evadeWaypointPosition = lastCollisionPoints[2] + Vector3.up * 50;
            lastTarget = target;
            target = evadeWaypoint;
        }

        if (Physics.Raycast(collisionDetectorDown, out hitInfo, detectionDistance))
        {
            if (showCollisionEvasionDebug)
                Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            collisionDetected = true;
            collisionPoint = hitInfo.point;
            lastCollisionPoints[3] = collisionPoint;
        }
        else if (collisionDetected && !evasionVectorFound)
        {
            collisionEvadeDirection = Vector3.down;
            evasionVectorFound = true;

            evadeWaypointPosition = lastCollisionPoints[3] + Vector3.down * 50;
            lastTarget = target;
            target = evadeWaypoint;
        }
    }

    public void SetWaypoint(Transform waypoint)
    {
        target = waypoint.gameObject;
        followingWaypoint = true;
    }
}
