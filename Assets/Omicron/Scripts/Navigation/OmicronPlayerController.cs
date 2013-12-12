using UnityEngine;
using System.Collections;

public class OmicronPlayerController : OmicronWandUpdater {
	
	public Transform head;
	public Transform body;
	
	public CAVE2Manager.Axis forwardAxis = CAVE2Manager.Axis.LeftAnalogStickUD;
	public CAVE2Manager.Axis strafeAxis = CAVE2Manager.Axis.LeftAnalogStickLR;

	public Vector3 headVector;
	
	float forward;
	float strafe;
	public float movementSpeed = 10;
	
	Vector3 moveDirection;
	public Vector3 headOffset;
	
	public bool enableCollisions = true;
	public bool collided;
	public float collisionThreshold = 0.2f;
	
	public Vector3 controllerOffset;
	Vector3 lastPosition;
	
	Stack lastPositions;
	int stackSize;
	
	getReal3D.ClusterView clusterView;
	
	public void Awake()
	{
		clusterView = gameObject.GetComponent<getReal3D.ClusterView>();
		if( clusterView == null )
		{
			gameObject.AddComponent<getReal3D.ClusterView>();
			clusterView = gameObject.GetComponent<getReal3D.ClusterView>();
			clusterView.observed = this;
		}
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		//stream.Serialize(ref controllerOffset);
	}
	
	// Use this for initialization
	new void Start () {
		InitOmicron();
		
		lastPositions = new Stack();
		controllerOffset = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		if( getReal3D.Cluster.isMaster )
		{
			forward = cave2Manager.getWand(wandID).GetAxis(forwardAxis);	
			forward *= movementSpeed;
			
			strafe = cave2Manager.getWand(wandID).GetAxis(strafeAxis);
			strafe *= movementSpeed;
		}
		
		headVector = head.position - body.position;
		
		if( Mathf.Abs ( headVector.x ) > collisionThreshold || Mathf.Abs ( headVector.z ) > collisionThreshold )
		{
			collided = true;
			if( enableCollisions )
			{
				if( lastPositions.Count > 0 )
				{
					controllerOffset = (Vector3)lastPositions.Pop();
					forward = 0;
					strafe = 0;
				}
			}
		}
		else
		{
			collided = false;
			lastPosition = controllerOffset;
			lastPositions.Push(controllerOffset);
		}
		
		CharacterController controller = GetComponentInChildren<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(headVector.x, 0, headVector.z);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= 50;            
        }
        controller.Move(moveDirection * getReal3D.Cluster.deltaTime);
		
		controllerOffset += new Vector3( strafe, 0, forward ) * getReal3D.Cluster.deltaTime;
		
		
		
		if( stackSize > 150 )
		{
			lastPositions.Clear();
		}
		stackSize = lastPositions.Count;
	}
}
