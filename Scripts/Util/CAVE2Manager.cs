/**************************************************************************************************
* THE OMICRON PROJECT
*-------------------------------------------------------------------------------------------------
* Copyright 2010-2015             Electronic Visualization Laboratory, University of Illinois at Chicago
* Authors:                                                                                
* Arthur Nishimoto                anishimoto42@gmail.com
*-------------------------------------------------------------------------------------------------
* Copyright (c) 2010-2015, Electronic Visualization Laboratory, University of Illinois at Chicago
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
using omicron;
using omicronConnector;

public class HeadTrackerState
{
	public int sourceID;
	public Vector3 position;
	public Quaternion rotation;
	
	public HeadTrackerState(int ID)
	{
		sourceID = ID;
		position = new Vector3();
		rotation = new Quaternion();
	}
	
	public void Update( Vector3 position, Quaternion orientation )
	{
		this.position = position;
		this.rotation = orientation;
	}
	
	public Vector3 GetPosition()
	{
		return position;
	}
	
	public Quaternion GetRotation()
	{
		return rotation;
	}
}

public class CAVE2Manager : OmicronEventClient {

	public bool CAVE2QuickSettings = false;
	HeadTrackerState head1;
	HeadTrackerState head2;
	
	public WandState wand1;
	public WandState wand2;
	
	public enum Axis { None, LeftAnalogStickLR, LeftAnalogStickUD, RightAnalogStickLR, RightAnalogStickUD, AnalogTriggerL, AnalogTriggerR,
		LeftAnalogStickLR_Inverted, LeftAnalogStickUD_Inverted, RightAnalogStickLR_Inverted, RightAnalogStickUD_Inverted, AnalogTriggerL_Inverted, AnalogTriggerR_Inverted
	};
	public enum Button { Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8, Button9, SpecialButton1, SpecialButton2, SpecialButton3, ButtonUp, ButtonDown, ButtonLeft, ButtonRight, None };
	
	// Note these represent Omicron sourceIDs
	public int Head1 = 0; 
	public int Wand1 = 1; // Controller ID
	public int Wand1Mocap = 3; // 3 = Xbox
	
	public int Head2 = 4; // 4 = Head_Tracker2
	public int Wand2 = 2;
	public int Wand2Mocap = 5;

	public float axisSensitivity = 1f;
	public float axisDeadzone = 0.2f;

	public bool keyboardEventEmulation = false;
	public bool wandMousePointerEmulation = false;
	public bool mocapEmulation = false;
	public bool lockWandToHeadTransform = false;

	public Vector3 headEmulatedPosition = new Vector3(0, 1.5f, 0);
	public Vector3 headEmulatedRotation = new Vector3(0, 0, 0);

	public Vector3 wandEmulatedPosition = new Vector3(0.175f, 1.2f, 0.6f);
	public Vector3 wandEmulatedRotation = new Vector3(0, 0, 0);

	public enum TrackerEmulated { CAVE, Head, Wand };
	public enum TrackerEmulationMode { Translate, Rotate };
	string[] trackerEmuStrings = {"CAVE", "Head", "Wand1"};
	string[] trackerEmuModeStrings = {"Translate", "Rotate" };

	public TrackerEmulated WASDkeys = TrackerEmulated.CAVE;
	public TrackerEmulationMode WASDkeyMode = TrackerEmulationMode.Translate;

	public TrackerEmulated IJKLkeys = TrackerEmulated.Head;
	public TrackerEmulationMode IJKLkeyMode = TrackerEmulationMode.Translate;
	
	public float emulatedTranslateSpeed = 0.05f;
	public float emulatedRotationSpeed = 0.05f;

	public int framerateCap = 60;
	public static string machineName;

	// Use this for initialization
	new void Start () {
		base.Start();
		
		head1 = new HeadTrackerState(Head1);
		head2 = new HeadTrackerState(Head2);
		
		if( wand1 == null )
			wand1 = new WandState(Wand1, Wand1Mocap);
		else
		{
			wand1.sourceID = Wand1;
			wand1.mocapID = Wand1Mocap;
		}
		if( wand2 == null )
			wand2 = new WandState(Wand2, Wand2Mocap);
		else
		{
			wand2.sourceID = Wand2;
			wand2.mocapID = Wand2Mocap;
		}

		Application.targetFrameRate = framerateCap;
		machineName = System.Environment.MachineName;
	}

	public static bool UsingGetReal3D()
	{
		if( Application.HasProLicense() && (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static bool OnCAVE2Master()
	{
		machineName = System.Environment.MachineName;
		if( machineName.Equals("LYRA-WIN") )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static bool OnCAVE2Display()
	{
		machineName = System.Environment.MachineName;
		if( machineName.Contains("LYRA") && !getReal3D.Cluster.isMaster )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// Update is called once per frame
	void Update () {
		if( CAVE2QuickSettings )
		{
			keyboardEventEmulation = false;
			wandMousePointerEmulation = false;
			mocapEmulation = false;
			lockWandToHeadTransform = false;

            OmicronManager omgManager = GetComponent<OmicronManager>();
            if( !omgManager.connectToServer )
                omgManager.ConnectToServer();
		}

		wand1.UpdateState(Wand1, Wand1Mocap);
		wand2.UpdateState(Wand2, Wand2Mocap);
		
		if( keyboardEventEmulation )
		{
			float vertical = Input.GetAxis("Vertical") * axisSensitivity;
			float horizontal = Input.GetAxis("Horizontal") * axisSensitivity;
			float lookHorizontal = Input.GetAxis("LookHorizontal") * axisSensitivity;
            float forward = Input.GetAxis("Forward") * axisSensitivity;

			uint flags = 0;
			
			// Arrow keys -> DPad
			if( Input.GetKey( KeyCode.UpArrow ) )
				flags += (int)EventBase.Flags.ButtonUp;
			if( Input.GetKey( KeyCode.DownArrow ) )
				flags += (int)EventBase.Flags.ButtonDown;
			if( Input.GetKey( KeyCode.LeftArrow ) )
				flags += (int)EventBase.Flags.ButtonLeft;
			if( Input.GetKey( KeyCode.RightArrow ) )
				flags += (int)EventBase.Flags.ButtonRight;
			
			// F -> Wand Button 2 (Circle)
			if( Input.GetKey( KeyCode.F ) || Input.GetMouseButton(1) )
				flags += (int)EventBase.Flags.Button2;
			// R -> Wand Button 3 (Cross)
			if( Input.GetKey( KeyCode.R ) || Input.GetMouseButton(0) )
				flags += (int)EventBase.Flags.Button3;

			Vector2 wandAnalog = new Vector2();
			Vector2 wandAnalog2 = new Vector2();
            Vector2 wandAnalog3 = new Vector2();
			if( WASDkeys == TrackerEmulated.CAVE )
			{
				wandAnalog = new Vector2(horizontal,vertical);
				wandAnalog2 = new Vector2(lookHorizontal,0);
                wandAnalog3 = new Vector2(forward,0);
			}
			else if( WASDkeys == TrackerEmulated.Head )
			{
				if( WASDkeyMode == TrackerEmulationMode.Translate )
				{
					headEmulatedPosition += new Vector3( horizontal, 0, vertical ) * Time.deltaTime;
				}
				else if( WASDkeyMode == TrackerEmulationMode.Rotate )
					headEmulatedRotation += new Vector3( vertical, horizontal, 0 );
			}

            wand1.UpdateController( flags, wandAnalog , wandAnalog2, wandAnalog3 );

			float headForward = 0;
			float headStrafe = 0;
			float headVertical = 0;
			
			float speed = emulatedTranslateSpeed;
			if( IJKLkeyMode == TrackerEmulationMode.Translate )
				speed = emulatedTranslateSpeed;
			else if( IJKLkeyMode == TrackerEmulationMode.Rotate )
				speed = emulatedRotationSpeed;
			
			if( Input.GetKey(KeyCode.I) )
				headForward += speed;
			else if( Input.GetKey(KeyCode.K) )
				headForward -= speed;
			if( Input.GetKey(KeyCode.J) )
				headStrafe -= speed;
			else if( Input.GetKey(KeyCode.L) )
				headStrafe += speed;
			if( Input.GetKey(KeyCode.U) )
				headVertical += speed;
			else if( Input.GetKey(KeyCode.O) )
				headVertical -= speed;

			if( IJKLkeys == TrackerEmulated.Head )
			{
				if( IJKLkeyMode == TrackerEmulationMode.Translate )
					headEmulatedPosition += new Vector3( headStrafe, headVertical, headForward );
				else if( IJKLkeyMode == TrackerEmulationMode.Rotate )
					headEmulatedRotation += new Vector3( headForward, headStrafe, headVertical );
			}
			else if( IJKLkeys == TrackerEmulated.Wand )
			{
				if( IJKLkeyMode == TrackerEmulationMode.Translate )
					wandEmulatedPosition += new Vector3( headStrafe, headVertical, headForward );
				else if( IJKLkeyMode == TrackerEmulationMode.Rotate )
					wandEmulatedRotation += new Vector3( headForward, headStrafe, headVertical );
			}


		}

		if( mocapEmulation )
		{
			//Vector3 lookAround = new Vector3( -wand1.GetAxis(Axis.RightAnalogStickUD), wand1.GetAxis(Axis.RightAnalogStickLR), 0 );
			//lookAround *= 2;
			//headEmulatedRotation += lookAround;

			// Update emulated positions/rotations
			head1.Update( headEmulatedPosition , Quaternion.Euler(headEmulatedRotation) );

			if( lockWandToHeadTransform )
				wand1.UpdateMocap( headEmulatedPosition , Quaternion.Euler(headEmulatedRotation) );
			else
				wand1.UpdateMocap( wandEmulatedPosition , Quaternion.Euler(wandEmulatedRotation) );
			
			GameObject.FindGameObjectWithTag("CameraController").transform.localPosition = headEmulatedPosition;
			GameObject.FindGameObjectWithTag("CameraController").transform.localEulerAngles = headEmulatedRotation;
		}
	}

	void OnEvent( EventData e )
	{
		//Debug.Log("CAVE2Manager: '"+name+"' received " + e.serviceType);
		if( e.serviceType == EventBase.ServiceType.ServiceTypeMocap )
		{
			// -zPos -xRot -yRot for Omicron->Unity coordinate conversion)
			Vector3 unityPos = new Vector3(e.posx, e.posy, -e.posz);
			Quaternion unityRot = new Quaternion(-e.orx, -e.ory, e.orz, e.orw);

			if( e.sourceId == head1.sourceID )
			{
				head1.Update( unityPos, unityRot );
			}
			else if( e.sourceId == head2.sourceID )
			{
				head2.Update( unityPos, unityRot );
			}
			else if( e.sourceId == wand1.mocapID )
			{
				wand1.UpdateMocap( unityPos, unityRot );
			}
			else if( e.sourceId == wand2.mocapID )
			{
				wand2.UpdateMocap( unityPos, unityRot );
			}
		}
		else if( e.serviceType == EventBase.ServiceType.ServiceTypeWand )
		{
			// -zPos -xRot -yRot for Omicron->Unity coordinate conversion)
			//Vector3 unityPos = new Vector3(e.posx, e.posy, -e.posz);
			//Quaternion unityRot = new Quaternion(-e.orx, -e.ory, e.orz, e.orw);
			
			// Flip Up/Down analog stick values
			Vector2 leftAnalogStick = new Vector2( e.getExtraDataFloat(0), -e.getExtraDataFloat(1) ) * axisSensitivity;
			Vector2 rightAnalogStick = new Vector2( e.getExtraDataFloat(2), -e.getExtraDataFloat(3) ) * axisSensitivity;
			Vector2 analogTrigger = new Vector2( e.getExtraDataFloat(4), e.getExtraDataFloat(5) );
			
			if( Mathf.Abs(leftAnalogStick.x) < axisDeadzone )
				leftAnalogStick.x = 0;
			if( Mathf.Abs(leftAnalogStick.y) < axisDeadzone )
				leftAnalogStick.y = 0;
			if( Mathf.Abs(rightAnalogStick.x) < axisDeadzone )
				rightAnalogStick.x = 0;
			if( Mathf.Abs(rightAnalogStick.y) < axisDeadzone )
				rightAnalogStick.y = 0;
			
			if( e.sourceId == wand1.sourceID )
			{
				wand1.UpdateController( e.flags, leftAnalogStick, rightAnalogStick, analogTrigger );
			}
			else if( e.sourceId == wand2.sourceID )
			{
				wand2.UpdateController( e.flags, leftAnalogStick, rightAnalogStick, analogTrigger );
			}
		}
	}
	
	public HeadTrackerState getHead(int ID)
	{
		if( ID == 2 )
			return head2;
		else if( ID == 1 )
			return head1;
		else
		{
			Debug.LogWarning("CAVE2Manager: getHead ID: " +ID+" does not exist. Returned Head1");
			return head1;
		}
	}
	
	public WandState getWand(int ID)
	{
		if( ID == 2 )
			return wand2;
		else if( ID == 1 )
			return wand1;
		else
		{
			Debug.LogWarning("CAVE2Manager: getWand ID: " +ID+" does not exist. Returned Wand1");
			return wand1;
		}
	}

	Vector2 GUIOffset;
	
	public void SetGUIOffSet( Vector2 offset )
	{
		GUIOffset = offset;
    }
	
    public void OnWindow(int windowID)
	{
		float rowHeight = 25;

		keyboardEventEmulation = GUI.Toggle (new Rect (GUIOffset.x + 20, GUIOffset.y + rowHeight * 0, 250, 20), keyboardEventEmulation, "Keyboard Event Emulation");
		wandMousePointerEmulation = GUI.Toggle (new Rect (GUIOffset.x + 20, GUIOffset.y + rowHeight * 1, 250, 20), wandMousePointerEmulation, "Wand-Mouse Pointer");
		mocapEmulation = GUI.Toggle (new Rect (GUIOffset.x + 20, GUIOffset.y + rowHeight * 2, 250, 20), mocapEmulation, "Mocap Emulation");
		lockWandToHeadTransform = GUI.Toggle (new Rect (GUIOffset.x + 20, GUIOffset.y + rowHeight * 3, 250, 20), lockWandToHeadTransform, "Lock Wand to Head Transform");

		GUI.Label(new Rect(GUIOffset.x + 20, GUIOffset.y + rowHeight * 4, 200, 20), "WASD Keys: ");
		WASDkeys = (TrackerEmulated)GUI.SelectionGrid(new Rect(GUIOffset.x + 100, GUIOffset.y + rowHeight * 4, 200, 20), (int)WASDkeys, trackerEmuStrings, 3);
		WASDkeyMode = (TrackerEmulationMode)GUI.SelectionGrid(new Rect(GUIOffset.x + 100, GUIOffset.y + rowHeight * 5, 200, 20), (int)WASDkeyMode, trackerEmuModeStrings, 3);

		GUI.Label(new Rect(GUIOffset.x + 20, GUIOffset.y + rowHeight * 6, 200, 20), "IJKL Keys: ");
		IJKLkeys = (TrackerEmulated)GUI.SelectionGrid(new Rect(GUIOffset.x + 100, GUIOffset.y + rowHeight * 6, 200, 20), (int)IJKLkeys, trackerEmuStrings, 3);
		IJKLkeyMode = (TrackerEmulationMode)GUI.SelectionGrid(new Rect(GUIOffset.x + 100, GUIOffset.y + rowHeight * 7, 200, 20), (int)IJKLkeyMode, trackerEmuModeStrings, 3);

    }
}
