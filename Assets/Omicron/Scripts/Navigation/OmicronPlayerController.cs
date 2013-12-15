/**************************************************************************************************
* THE OMICRON PROJECT
*-------------------------------------------------------------------------------------------------
* Copyright 2010-2013             Electronic Visualization Laboratory, University of Illinois at Chicago
* Authors:                                                                                
* Arthur Nishimoto                anishimoto42@gmail.com
*-------------------------------------------------------------------------------------------------
* Copyright (c) 2010-2013, Electronic Visualization Laboratory, University of Illinois at Chicago
* All rights reserved.
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
*
* Redistributions of source code must retain the above copyright notice, this list of conditions
* and the following disclaimer. Redistributions in binary form must reproduce the above copyright
* notice, this list of conditions and the following disclaimer in the documentation and/or other
* materials provided with the distribution.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
* IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
* FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
* CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
* USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
* WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
* ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*************************************************************************************************/

using UnityEngine;
using System.Collections;

public class OmicronPlayerController : OmicronWandUpdater {

	public CAVE2Manager.Axis forwardAxis = CAVE2Manager.Axis.LeftAnalogStickUD;
	public CAVE2Manager.Axis strafeAxis = CAVE2Manager.Axis.LeftAnalogStickLR;

	public float forward;
	public float strafe;
	public float movementScale = 2;
	public float flyMovementScale = 100;
	public float turnSpeed = 50;
	
	public Vector3 moveDirection;
	
	public CharacterController controller;
	public CharacterMotor motor;
	
	// Walk - Analog stick movement with physics
	// Fly - 6DoF movement with no physics
	// Drive - Same as fly without pitch/roll
	public enum NavigationMode { Walk, Drive, Freefly }
	
	public NavigationMode navMode = NavigationMode.Walk;
	public CAVE2Manager.Button freeFlyToggleButton = CAVE2Manager.Button.Button5;
	public CAVE2Manager.Button freeFlyButton = CAVE2Manager.Button.Button7;
	
	public enum HorizonalMovementMode { Strafe, Turn }
	public HorizonalMovementMode horizontalMovementMode = HorizonalMovementMode.Strafe;
	
	public enum AutoLevelMode { Disabled, OnGroundCollision };
	public AutoLevelMode autoLevelMode = AutoLevelMode.OnGroundCollision;
	public CAVE2Manager.Button autoLevelButton = CAVE2Manager.Button.Button6;
	
	public enum ForwardRef { None, Head, Wand }
	public ForwardRef forwardReference = ForwardRef.Wand;
	public int headID = 1;
	
	public GameObject head;
	Vector3 headPosition;
	//Quaternion headRotation;
	
	public float headBodyDistance;
	public Vector3 headWorldPos;
	
	public bool showCAVEFloorOnlyOnMaster = true;
	public GameObject CAVEFloor;
	
	Vector3 wandPosition;
	Quaternion wandRotation;

	
	
	
	bool freeflyButtonDown;
	bool freeflyInitVectorSet;
	Vector3 freeflyInitVector;
	
	Vector3 fly_x, fly_y, fly_z;
	
	public bool showGUI = true;
		
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
		controller = GetComponent<CharacterController>();
		motor = GetComponent<CharacterMotor>();
		
		
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		stream.Serialize( ref freeflyButtonDown );
		stream.Serialize( ref wandPosition );
		stream.Serialize( ref wandRotation );
	}
	
	// Use this for initialization
	new void Start () {
		InitOmicron();
	}

	// Update is called once per frame
	void Update () {
		
		if( getReal3D.Cluster.isMaster )
		{
			wandPosition = cave2Manager.getWand(wandID).GetPosition();
			wandRotation = cave2Manager.getWand(wandID).GetRotation();
			
			headPosition = cave2Manager.getHead(headID).GetPosition();
			
			//headWorldPos = transform.TransformPoint( headPosition );
			//headWorldPos = head.transform.position;
			//transform.position = new Vector3( headWorldPos.x, transform.position.y, headWorldPos.z );
			//headRotation = cave2Manager.getHead(headID).GetRotation();
			
			//headBodyDistance = Mathf.Sqrt( Mathf.Pow(transform.position.x - headWorldPos.x,2) + Mathf.Pow(transform.position.z - headWorldPos.z,2) );
			
			//Vector3 headOffsetVector = Vector3.zero;
			//if( headBodyDistance > 0.1f )
			//	headOffsetVector = headWorldPos;
			
			forward = cave2Manager.getWand(wandID).GetAxis(forwardAxis);	
			forward *= movementScale;
			
			strafe = cave2Manager.getWand(wandID).GetAxis(strafeAxis);	
			strafe *= movementScale;
			
			
			freeflyButtonDown = cave2Manager.getWand(wandID).GetButton(freeFlyButton);
			
			if( cave2Manager.getWand(wandID).GetButtonDown(freeFlyToggleButton) )
			{
				navMode++;
				if( (int)navMode > 2 )
					navMode = 0;
				clusterView.RPC("SetNavigationMode", (int)navMode);
			}
			
			if( cave2Manager.getWand(wandID).GetButtonDown(autoLevelButton) )
			{
				transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 );
			}
			
			if( !CAVEFloor.activeSelf )
				CAVEFloor.SetActive(true);
		}
		else
		{
			if( showCAVEFloorOnlyOnMaster && CAVEFloor.activeSelf )
				CAVEFloor.SetActive(false);
			else if( !showCAVEFloorOnlyOnMaster && !CAVEFloor.activeSelf )
				CAVEFloor.SetActive(true);
		}
		CAVEFloor.transform.rotation = Quaternion.identity;
		
		if( navMode == NavigationMode.Drive || navMode == NavigationMode.Freefly )
		{
			UpdateFreeflyMovement();
		}
		else
		{
			UpdateWalkMovement();
		}
	}
	
	void UpdateFreeflyMovement()
	{
		motor.enabled = false;
		controller.enabled = false;
		
		if( freeflyButtonDown && !freeflyInitVectorSet )
		{
			freeflyInitVector = wandPosition;
			freeflyInitVectorSet = true;
			
			Vector3 xVec = new Vector3(1.0f,0.0f,0.0f);
			Vector3 yVec = new Vector3(0.0f,1.0f,0.0f);
			Vector3 zVec = new Vector3(0.0f,0.0f,1.0f);
			fly_x = wandRotation * xVec;
			fly_y = wandRotation * yVec;
			fly_z = wandRotation * zVec;
		}
		else if( !freeflyButtonDown )
		{		
			freeflyInitVector = Vector3.zero;
			freeflyInitVectorSet = false;
			
		}
		else
		{
			Vector3 movementVector = (wandPosition - freeflyInitVector) + new Vector3(strafe, 0, forward);

			// Ported from Electro's Vortex application by Robert Kooima
			//Vector3 xVec = new Vector3(1.0f,0.0f,0.0f);
			Vector3 yVec = new Vector3(0.0f,1.0f,0.0f);
			Vector3 zVec = new Vector3(0.0f,0.0f,1.0f);
			//Vector3 x = wandRotation * xVec;
			Vector3 y = wandRotation * yVec;
			Vector3 z = wandRotation * zVec;
			
			float vx = fly_z.x - z.x;
			float vy = fly_z.y - z.y;
			float vz = fly_z.z - z.z;
			
			float wx = fly_y.x - y.x;
			float wy = fly_y.y - y.y;
			float wz = fly_y.z - y.z;
			
			float rX =  (vx * fly_y.x + vy * fly_y.y + vz * fly_y.z) * getReal3D.Cluster.deltaTime * turnSpeed;
			float rY = -(vx * fly_x.x + vy * fly_x.y + vz * fly_x.z) * getReal3D.Cluster.deltaTime * turnSpeed;
			float rZ =  (wx * fly_x.x + wy * fly_x.y + wz * fly_x.z) * getReal3D.Cluster.deltaTime * turnSpeed;

			transform.Translate( movementVector * getReal3D.Cluster.deltaTime * flyMovementScale);
			if( navMode == NavigationMode.Freefly )
				transform.Rotate( new Vector3( rX, rY + wandRotation.y, rZ) );
		}
		
		if( horizontalMovementMode == HorizonalMovementMode.Strafe )
			transform.Translate( new Vector3(strafe, 0, forward) * getReal3D.Cluster.deltaTime );
		else if( horizontalMovementMode == HorizonalMovementMode.Turn )
		{
			transform.Translate( new Vector3(0, 0, forward) * getReal3D.Cluster.deltaTime );
			transform.Rotate( new Vector3( 0, strafe, 0) * getReal3D.Cluster.deltaTime * turnSpeed );
		}
	}
	
	void UpdateWalkMovement()
	{
		motor.enabled = true;
		controller.enabled = true;
			
		if(controller.isGrounded)
		{
			if( horizontalMovementMode == HorizonalMovementMode.Strafe )
	    		moveDirection = new Vector3(strafe, 0, forward);
			else
				moveDirection = new Vector3(0, 0, forward);
	    	moveDirection = transform.TransformDirection(moveDirection); 
			
			if( autoLevelMode == AutoLevelMode.OnGroundCollision )
			{
				transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 );
			}
	    }
	    controller.Move(moveDirection * getReal3D.Cluster.deltaTime);
		
		if( horizontalMovementMode == HorizonalMovementMode.Turn )
		{
			transform.Rotate( new Vector3( 0, strafe, 0) * getReal3D.Cluster.deltaTime * turnSpeed );
		}
	}
	
	[getReal3D.RPC]
	void SetNavigationMode( int val )
	{
		navMode = (NavigationMode)val;
	}

    string[] navStrings = new string[] {"Walk", "Drive", "Freefly"};
	string[] horzStrings = new string[] {"Strafe", "Turn"};
	void OnGUI() {
		if( showGUI && getReal3D.Cluster.isMaster )
		{	
			GUI.Box(new Rect(0, 0, 250 , 275 ), "Omicron Player Controller");
			
			GUI.Label(new Rect(25, 20 * 1, 200, 20), "Position: " + transform.position);
			GUI.Label(new Rect(25, 20 * 2, 200, 20), "Head Position: " + headPosition);
			GUI.Label(new Rect(25, 20 * 3, 200, 20), "Wand Position: " + wandPosition);
			
			GUI.Label(new Rect(25, 20 * 4, 200, 20), "Navigation Mode: ");
			navMode = (NavigationMode)GUI.SelectionGrid(new Rect(25, 20 * 5, 200, 20), (int)navMode, navStrings, 3);
			
			GUI.Label(new Rect(25, 20 * 6, 200, 20), "Left Analog LR Mode: ");
			horizontalMovementMode = (HorizonalMovementMode)GUI.SelectionGrid(new Rect(25, 20 * 7, 200, 20), (int)horizontalMovementMode, horzStrings, 3);
			
			GUI.Label(new Rect(25, 20 * 8 + 5, 120, 20), "Walk Nav Scale: ");
	        movementScale = float.Parse(GUI.TextField(new Rect(150, 20 * 8 + 5, 75, 20), movementScale.ToString(), 25));
			
			GUI.Label(new Rect(25, 20 * 9 + 10, 120, 20), "Drive/Fly Nav Scale: ");
	        flyMovementScale = float.Parse(GUI.TextField(new Rect(150, 20 * 9 + 10, 75, 20), flyMovementScale.ToString(), 25));
			
			GUI.Label(new Rect(25, 20 * 10 + 15, 120, 20), "Rotate Scale: ");
	        turnSpeed = float.Parse(GUI.TextField(new Rect(150, 20 * 10 + 15, 75, 20), turnSpeed.ToString(), 25));

			//GUI.Label(new Rect(25, 20 * 9 + 15, 120, 20), "Auto Level On Ground Collision: ");
	        if( GUI.Toggle(new Rect(25, 20 * 11 + 15, 250, 200), (autoLevelMode == AutoLevelMode.OnGroundCollision), " Auto Level On Ground Collision") )
				autoLevelMode = AutoLevelMode.OnGroundCollision;
			else
				autoLevelMode = AutoLevelMode.Disabled;
			//trench = GUI.Toggle(new Rect(25, 200, 100, 30), trench, "Trench");
			//surfaceHardReset = GUI.Toggle(new Rect(25, 220, 100, 30), surfaceHardReset, "Reset");
		}
    }
}
