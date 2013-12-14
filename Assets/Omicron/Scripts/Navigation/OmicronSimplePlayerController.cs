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

public class OmicronSimplePlayerController : OmicronWandUpdater {

	public CAVE2Manager.Axis forwardAxis = CAVE2Manager.Axis.LeftAnalogStickUD;
	public CAVE2Manager.Axis strafeAxis = CAVE2Manager.Axis.LeftAnalogStickLR;

	public float forward;
	public float strafe;
	public float movementScale = 2;
	public float flyMovementScale = 100;
	public float flyRotationSpeed = 100;
	
	public Vector3 moveDirection;
	
	public CharacterController controller;
	public CharacterMotor motor;

	public bool freeflyMode = false;
	public CAVE2Manager.Button freeFlyToggleButton = CAVE2Manager.Button.Button5;
	public CAVE2Manager.Button freeFlyButton = CAVE2Manager.Button.Button7;
	
	Vector3 wandPosition;
	Quaternion wandRotation;
	bool freeflyButtonDown;
	bool freeflyInitVectorSet;
	Vector3 freeflyInitVector;
	
	Vector3 fly_x, fly_y, fly_z;
	
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
			forward = cave2Manager.getWand(wandID).GetAxis(forwardAxis);	
			forward *= movementScale;
			
			strafe = cave2Manager.getWand(wandID).GetAxis(strafeAxis);
			strafe *= movementScale;
			
			wandPosition = cave2Manager.getWand(wandID).GetPosition();
			wandRotation = cave2Manager.getWand(wandID).GetRotation();
			
			freeflyButtonDown = cave2Manager.getWand(wandID).GetButton(freeFlyButton);
			
			if( cave2Manager.getWand(wandID).GetButtonDown(freeFlyToggleButton) )
			{
				freeflyMode = !freeflyMode;
				clusterView.RPC("SetFreeflyMode", freeflyMode);
			}
		}

		if( freeflyMode )
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
			
			float rX =  (vx * fly_y.x + vy * fly_y.y + vz * fly_y.z) * getReal3D.Cluster.deltaTime * flyRotationSpeed;
			float rY = -(vx * fly_x.x + vy * fly_x.y + vz * fly_x.z) * getReal3D.Cluster.deltaTime * flyRotationSpeed;
			float rZ =  (wx * fly_x.x + wy * fly_x.y + wz * fly_x.z) * getReal3D.Cluster.deltaTime * flyRotationSpeed;

			transform.Translate( movementVector * getReal3D.Cluster.deltaTime * flyMovementScale);
			transform.Rotate( new Vector3( rX, rY, rZ) );
		}
		transform.Translate( new Vector3(strafe, 0, forward) * getReal3D.Cluster.deltaTime );
	}
	
	void UpdateWalkMovement()
	{
		motor.enabled = true;
		controller.enabled = true;
			
		if(controller.isGrounded)
		{
	    	moveDirection = new Vector3(strafe, 0, forward);
	    	moveDirection = transform.TransformDirection(moveDirection);          
	    }
		
	    controller.Move(moveDirection * getReal3D.Cluster.deltaTime);
	}
	
	[getReal3D.RPC]
	void SetFreeflyMode( bool val )
	{
		freeflyMode = val;
	}
}
