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
}

public class WandState
{
	public int sourceID;
	public int mocapID;
	public Vector3 position;
	public Quaternion rotation;
	uint flags;
	
	public Vector2 leftAnalogStick;
	public Vector2 rightAnalogStick;
	public Vector2 analogTrigger;
	
	public enum ButtonState { Idle, Down, Held, Up };
	
	public ButtonState button1 = ButtonState.Idle;
	public ButtonState button2 = ButtonState.Idle;
	public ButtonState button3 = ButtonState.Idle;
	public ButtonState button4 = ButtonState.Idle;
	public ButtonState button5 = ButtonState.Idle;
	public ButtonState button6 = ButtonState.Idle;
	public ButtonState button7 = ButtonState.Idle;
	public ButtonState button8 = ButtonState.Idle;
	public ButtonState button9 = ButtonState.Idle;
	public ButtonState buttonUp = ButtonState.Idle;
	public ButtonState buttonDown = ButtonState.Idle;
	public ButtonState buttonLeft = ButtonState.Idle;
	public ButtonState buttonRight = ButtonState.Idle;
	public ButtonState buttonSP1 = ButtonState.Idle;
	public ButtonState buttonSP2 = ButtonState.Idle;
	public ButtonState buttonSP3 = ButtonState.Idle;
	
	// Time new data was received
	float updateTime = 0;
	
	public WandState(int ID, int mocapID)
	{
		sourceID = ID;
		this.mocapID = mocapID;
		position = new Vector3();
		rotation = new Quaternion();
	}
	
	public float GetAxis( CAVE2Manager.Axis axis )
	{
		switch(axis)
		{
			case( CAVE2Manager.Axis.LeftAnalogStickLR ): return leftAnalogStick.x;
			case( CAVE2Manager.Axis.LeftAnalogStickUD ): return leftAnalogStick.y;
			case( CAVE2Manager.Axis.RightAnalogStickLR ): return rightAnalogStick.x;
			case( CAVE2Manager.Axis.RightAnalogStickUD ): return rightAnalogStick.y;
			case( CAVE2Manager.Axis.AnalogTriggerL ): return analogTrigger.x;
			case( CAVE2Manager.Axis.AnalogTriggerR ): return analogTrigger.y;
			default: return 0;
		}
	}
	
	public bool GetButton( CAVE2Manager.Button button )
	{
		switch(button)
		{
			case(CAVE2Manager.Button.Button1): return button1 == ButtonState.Held;
			case(CAVE2Manager.Button.Button2): return button2 == ButtonState.Held;
			case(CAVE2Manager.Button.Button3): return button3 == ButtonState.Held;
			case(CAVE2Manager.Button.Button4): return button4 == ButtonState.Held;
			case(CAVE2Manager.Button.Button5): return button5 == ButtonState.Held;
			case(CAVE2Manager.Button.Button6): return button6 == ButtonState.Held;
			case(CAVE2Manager.Button.Button7): return button7 == ButtonState.Held;
			case(CAVE2Manager.Button.Button8): return button8 == ButtonState.Held;
			case(CAVE2Manager.Button.Button9): return button9 == ButtonState.Held;
			case(CAVE2Manager.Button.SpecialButton1): return buttonSP1 == ButtonState.Held;
			case(CAVE2Manager.Button.SpecialButton2): return buttonSP2 == ButtonState.Held;
			case(CAVE2Manager.Button.SpecialButton3): return buttonSP3 == ButtonState.Held;
			case(CAVE2Manager.Button.ButtonUp): return buttonUp == ButtonState.Held;
			case(CAVE2Manager.Button.ButtonDown): return buttonDown == ButtonState.Held;
			case(CAVE2Manager.Button.ButtonLeft): return buttonLeft == ButtonState.Held;
			case(CAVE2Manager.Button.ButtonRight): return buttonRight == ButtonState.Held;
			default: return false;
		}
	}
	
	public bool GetButtonDown( CAVE2Manager.Button button )
	{
		switch(button)
		{
			case(CAVE2Manager.Button.Button1): return button1 == ButtonState.Down;
			case(CAVE2Manager.Button.Button2): return button2 == ButtonState.Down;
			case(CAVE2Manager.Button.Button3): return button3 == ButtonState.Down;
			case(CAVE2Manager.Button.Button4): return button4 == ButtonState.Down;
			case(CAVE2Manager.Button.Button5): return button5 == ButtonState.Down;
			case(CAVE2Manager.Button.Button6): return button6 == ButtonState.Down;
			case(CAVE2Manager.Button.Button7): return button7 == ButtonState.Down;
			case(CAVE2Manager.Button.Button8): return button8 == ButtonState.Down;
			case(CAVE2Manager.Button.Button9): return button9 == ButtonState.Down;
			case(CAVE2Manager.Button.SpecialButton1): return buttonSP1 == ButtonState.Down;
			case(CAVE2Manager.Button.SpecialButton2): return buttonSP2 == ButtonState.Down;
			case(CAVE2Manager.Button.SpecialButton3): return buttonSP3 == ButtonState.Down;
			case(CAVE2Manager.Button.ButtonUp): return buttonUp == ButtonState.Down;
			case(CAVE2Manager.Button.ButtonDown): return buttonDown == ButtonState.Down;
			case(CAVE2Manager.Button.ButtonLeft): return buttonLeft == ButtonState.Down;
			case(CAVE2Manager.Button.ButtonRight): return buttonRight == ButtonState.Down;
			default: return false;
		}
	}
	
	public bool GetButtonUp( CAVE2Manager.Button button )
	{
		switch(button)
		{
			case(CAVE2Manager.Button.Button1): return button1 == ButtonState.Up;
			case(CAVE2Manager.Button.Button2): return button2 == ButtonState.Up;
			case(CAVE2Manager.Button.Button3): return button3 == ButtonState.Up;
			case(CAVE2Manager.Button.Button4): return button4 == ButtonState.Up;
			case(CAVE2Manager.Button.Button5): return button5 == ButtonState.Up;
			case(CAVE2Manager.Button.Button6): return button6 == ButtonState.Up;
			case(CAVE2Manager.Button.Button7): return button7 == ButtonState.Up;
			case(CAVE2Manager.Button.Button8): return button8 == ButtonState.Up;
			case(CAVE2Manager.Button.Button9): return button9 == ButtonState.Up;
			case(CAVE2Manager.Button.SpecialButton1): return buttonSP1 == ButtonState.Up;
			case(CAVE2Manager.Button.SpecialButton2): return buttonSP2 == ButtonState.Up;
			case(CAVE2Manager.Button.SpecialButton3): return buttonSP3 == ButtonState.Up;
			case(CAVE2Manager.Button.ButtonUp): return buttonUp == ButtonState.Up;
			case(CAVE2Manager.Button.ButtonDown): return buttonDown == ButtonState.Up;
			case(CAVE2Manager.Button.ButtonLeft): return buttonLeft == ButtonState.Up;
			case(CAVE2Manager.Button.ButtonRight): return buttonRight == ButtonState.Up;
			default: return false;
		}
	}
	
	public void Update()
	{
		// Wait a few frames before updating states to held or idle
		if( Time.time < updateTime + Time.deltaTime * 2 )
			return;
		
		// Set buttons held if down on the last frame
		// Set buttons as idle if up on the last frame
		if( button1 == ButtonState.Down )
			button1 = ButtonState.Held;
		else if( button1 == ButtonState.Up )
			button1 = ButtonState.Idle;
		
		if( button2 == ButtonState.Down )
			button2 = ButtonState.Held;
		else if( button2 == ButtonState.Up )
			button2 = ButtonState.Idle;
		
		if( button3 == ButtonState.Down )
			button3 = ButtonState.Held;
		else if( button3 == ButtonState.Up )
			button3 = ButtonState.Idle;
		
		if( button4 == ButtonState.Down )
			button4 = ButtonState.Held;
		else if( button4 == ButtonState.Up )
			button4 = ButtonState.Idle;
		
		if( button5 == ButtonState.Down )
			button5 = ButtonState.Held;
		else if( button5 == ButtonState.Up )
			button5 = ButtonState.Idle;
		
		if( button6 == ButtonState.Down )
			button6 = ButtonState.Held;
		else if( button6 == ButtonState.Up )
			button6 = ButtonState.Idle;
		
		if( button7 == ButtonState.Down )
			button7 = ButtonState.Held;
		else if( button7 == ButtonState.Up )
			button7 = ButtonState.Idle;
		
		if( button8 == ButtonState.Down )
			button8 = ButtonState.Held;
		else if( button8 == ButtonState.Up )
			button8 = ButtonState.Idle;
		
		if( button9 == ButtonState.Down )
			button9 = ButtonState.Held;
		else if( button9 == ButtonState.Up )
			button9 = ButtonState.Idle;
		
		if( buttonDown == ButtonState.Down )
			buttonDown = ButtonState.Held;
		else if( buttonDown == ButtonState.Up )
			buttonDown = ButtonState.Idle;
		
		if( buttonUp == ButtonState.Down )
			buttonUp = ButtonState.Held;
		else if( buttonUp == ButtonState.Up )
			buttonUp = ButtonState.Idle;
		
		if( buttonLeft == ButtonState.Down )
			buttonLeft = ButtonState.Held;
		else if( buttonLeft == ButtonState.Up )
			buttonLeft = ButtonState.Idle;
		
		if( buttonRight == ButtonState.Down )
			buttonRight = ButtonState.Held;
		else if( buttonRight == ButtonState.Up )
			buttonRight = ButtonState.Idle;
		
		if( buttonSP1 == ButtonState.Down )
			buttonSP1 = ButtonState.Held;
		else if( buttonSP1 == ButtonState.Up )
			buttonSP1 = ButtonState.Idle;
		
		if( buttonSP2 == ButtonState.Down )
			buttonSP2 = ButtonState.Held;
		else if( buttonSP2 == ButtonState.Up )
			buttonSP2 = ButtonState.Idle;
		
		if( buttonSP3 == ButtonState.Down )
			buttonSP3 = ButtonState.Held;
		else if( buttonSP3 == ButtonState.Up )
			buttonSP3 = ButtonState.Idle;
	}
	
	public void UpdateMocap( Vector3 position, Quaternion orientation )
	{
		this.position = position;
		this.rotation = orientation;
	}
	
	public void UpdateController( uint flags, Vector2 leftAnalogStick, Vector2 rightAnalogStick, Vector2 analogTrigger )
	{
		updateTime = Time.time;

		this.leftAnalogStick = leftAnalogStick;
		this.rightAnalogStick = rightAnalogStick;
		this.analogTrigger = analogTrigger;
		
		// Update any state changes
		if( (flags & 1) == 1 )
			button1 = ButtonState.Down;
		else if( (flags & 1) == 0 )
			button1 = ButtonState.Up;
		
		if( (flags & 2) == 2 )
			button2 = ButtonState.Down;
		else if( (flags & 2) == 0 )
			button2 = ButtonState.Up;
			
		if( (flags & 4) == 4 )
			button3 = ButtonState.Down;
		else if( (flags & 4) == 0 )
			button3 = ButtonState.Up;
			
		if( (flags & 8) == 8 )
			buttonSP1 = ButtonState.Down;
		else if( (flags & 8) == 0 )
			buttonSP1 = ButtonState.Up;
		
		if( (flags & 16) == 16 )
			buttonSP2 = ButtonState.Down;
		else if( (flags & 16) == 0 )
			buttonSP2 = ButtonState.Up;
		
		if( (flags & 32) == 32 )
			buttonSP3 = ButtonState.Down;
		else if( (flags & 32) == 0 )
			buttonSP3 = ButtonState.Up;
		
		if( (flags & 64) == 64 )
			button4 = ButtonState.Down;
		else if( (flags & 64) == 0 )
			button4 = ButtonState.Up;
		
		if( (flags & 128) == 128 )
			button5 = ButtonState.Down;
		else if( (flags & 128) == 0 )
			button5 = ButtonState.Up;
		
		if( (flags & 256) == 256 )
			button6 = ButtonState.Down;
		else if( (flags & 256) == 0 )
			button6 = ButtonState.Up;
		
		if( (flags & 512) == 512 )
			button7 = ButtonState.Down;
		else if( (flags & 512) == 0 )
			button7 = ButtonState.Up;
		
		if( (flags & 1024) == 1024 )
			buttonUp = ButtonState.Down;
		else if( (flags & 1024) == 0 )
			buttonUp = ButtonState.Up;
		
		if( (flags & 2048) == 2048 )
			buttonDown = ButtonState.Down;
		else if( (flags & 2048) == 0 )
			buttonDown = ButtonState.Up;
		
		if( (flags & 4096) == 4096 )
			buttonLeft = ButtonState.Down;
		else if( (flags & 4096) == 0 )
			buttonLeft = ButtonState.Up;
		
		if( (flags & 8192) == 8192 )
			buttonRight = ButtonState.Down;
		else if( (flags & 8192) == 0 )
			buttonRight = ButtonState.Up;
		
		if( (flags & (int)EventBase.Flags.Button8) == (int)EventBase.Flags.Button8 )
			button8 = ButtonState.Down;
		else if( (flags & (int)EventBase.Flags.Button8) == 0 )
			button8 = ButtonState.Up;
		
		if( (flags & (int)EventBase.Flags.Button9) == (int)EventBase.Flags.Button9 )
			button9 = ButtonState.Down;
		else if( (flags & (int)EventBase.Flags.Button9) == 0 )
			button9 = ButtonState.Up;
	}
}

public class CAVE2Manager : OmicronEventClient {
	
	HeadTrackerState head1;
	HeadTrackerState head2;
	
	public WandState wand1;
	public WandState wand2;
	
	public enum Axis { None, LeftAnalogStickLR, LeftAnalogStickUD, RightAnalogStickLR, RightAnalogStickUD, AnalogTriggerL, AnalogTriggerR };
	public enum Button { None, Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8, Button9, SpecialButton1, SpecialButton2, SpecialButton3, ButtonUp, ButtonDown, ButtonLeft, ButtonRight };
	
	// Note these represent Omicron sourceIDs
	public int Head1 = 0; 
	public int Wand1 = 1; // Controller ID
	public int Wand1Mocap = 3; // 3 = Xbox
	
	public int Head2 = 4; // 4 = Head_Tracker2
	public int Wand2 = 2;
	public int Wand2Mocap = 5;
	
	// Use this for initialization
	new void Start () {
		base.Start();
		
		head1 = new HeadTrackerState(Head1);
		head2 = new HeadTrackerState(Head2);
		
		wand1 = new WandState(Wand1, Wand1Mocap);
		wand2 = new WandState(Wand2, Wand2Mocap);
	}
	
	// Update is called once per frame
	void Update () {
		wand1.Update();
		wand2.Update();
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
			Vector3 unityPos = new Vector3(e.posx, e.posy, -e.posz);
			Quaternion unityRot = new Quaternion(-e.orx, -e.ory, e.orz, e.orw);
			
			// Flip Up/Down analog stick values
			Vector2 leftAnalogStick = new Vector2( e.getExtraDataFloat(0), -e.getExtraDataFloat(1) );
			Vector2 rightAnalogStick = new Vector2( e.getExtraDataFloat(2), -e.getExtraDataFloat(3) );
			Vector2 analogTrigger = new Vector2( e.getExtraDataFloat(4), e.getExtraDataFloat(5) );
			
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
}
