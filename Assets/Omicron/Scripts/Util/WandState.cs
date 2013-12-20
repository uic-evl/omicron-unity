using UnityEngine;
using System.Collections;
using omicron;
using omicronConnector;
using System;

public class WandState : MonoBehaviour
{
	public int sourceID;
	public int mocapID;
	public Vector3 position;
	public Quaternion rotation;
	public int flags;
	
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
	ButtonState buttonNull = ButtonState.Idle;
		
	public bool RPCButtonEvents = false;
	
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
	
	public void Start()
	{
	}

	public WandState( int ID, int mocapID )
	{
		sourceID = ID;
		this.mocapID = mocapID;
		
		position = new Vector3();
		rotation = new Quaternion();
		
		flags = 0;
		leftAnalogStick = new Vector2();
		rightAnalogStick = new Vector2();
		analogTrigger = new Vector2();
	}
	/*
	void OnGUI() {

		GUI.Box(new Rect(0, 0, 250 , 250 ), "Dynamic Tile Generator");
			
		GUI.Label(new Rect(25, 20 * 12, 200, 20), "B1: " + button1);
		GUI.Label(new Rect(25, 20 * 13, 200, 20), "B2: " + button2);
		GUI.Label(new Rect(25, 20 * 14, 200, 20), "B3: " + button3);
		GUI.Label(new Rect(25, 20 * 15, 200, 20), "B4: " + button4);
		GUI.Label(new Rect(25, 20 * (int)EventBase.Flags.SpecialButton2, 200, 20), "B5: " + button5);
		GUI.Label(new Rect(25, 20 * 17, 200, 20), "B6: " + button6);
		GUI.Label(new Rect(25, 20 * 18, 200, 20), "B6: " + button7);
		GUI.Label(new Rect(25, 20 * 19, 200, 20), "B7: " + button8);
    }
    */

	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		stream.Serialize(ref position);
		stream.Serialize(ref rotation);
		stream.Serialize(ref leftAnalogStick);
		stream.Serialize(ref rightAnalogStick);
		stream.Serialize(ref analogTrigger);
	}
	
	public Vector3 GetPosition()
	{
		return position;
	}
	
	public Quaternion GetRotation()
	{
		return rotation;
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
			
			case( CAVE2Manager.Axis.LeftAnalogStickLR_Inverted ): return -leftAnalogStick.x;
			case( CAVE2Manager.Axis.LeftAnalogStickUD_Inverted ): return -leftAnalogStick.y;
			case( CAVE2Manager.Axis.RightAnalogStickLR_Inverted ): return -rightAnalogStick.x;
			case( CAVE2Manager.Axis.RightAnalogStickUD_Inverted ): return -rightAnalogStick.y;
			case( CAVE2Manager.Axis.AnalogTriggerL_Inverted ): return -analogTrigger.x;
			case( CAVE2Manager.Axis.AnalogTriggerR_Inverted ): return -analogTrigger.y;
			
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
	
	public void UpdateState( int sourceID, int mocapID )
	{	
		// Just in case we want to change controllers at runtime
		this.sourceID = sourceID;
		this.mocapID = mocapID;
		
		// Set buttons held if down on the last frame
		// Set buttons as idle if up on the last frame
		
		for(int i = 0; i < (int)EventBase.Flags.SpecialButton2; i++ )
		{
			ButtonState buttonState = GetButtonState(i);
			
			if (getReal3D.Cluster.isMaster)
			{
				if (clusterView && RPCButtonEvents)
					clusterView.RPC("UpdateButton", i, (int)buttonState );
			}
			if( buttonState == ButtonState.Down )
				UpdateButton( i, (int)ButtonState.Held );
			else if( buttonState == ButtonState.Up )
				UpdateButton( i, (int)ButtonState.Idle );
		}
		
	}
	
	ButtonState GetButtonState( int buttonID )
	{
		switch(buttonID)
		{
			case((int)CAVE2Manager.Button.Button1): return button1;
			case((int)CAVE2Manager.Button.Button2): return button2;
			case((int)CAVE2Manager.Button.Button3): return button3;
			case((int)CAVE2Manager.Button.Button4): return button4;
			case((int)CAVE2Manager.Button.Button5): return button5;
			case((int)CAVE2Manager.Button.Button6): return button6;
			case((int)CAVE2Manager.Button.Button7): return button7;
			case((int)CAVE2Manager.Button.Button8): return button8;
			case((int)CAVE2Manager.Button.Button9): return button9;
			case((int)CAVE2Manager.Button.SpecialButton1): return buttonSP1;
			case((int)CAVE2Manager.Button.SpecialButton2): return buttonSP2;
			case((int)CAVE2Manager.Button.SpecialButton3): return buttonSP3;
			case((int)CAVE2Manager.Button.ButtonUp): return buttonUp;
			case((int)CAVE2Manager.Button.ButtonDown): return buttonDown;
			case((int)CAVE2Manager.Button.ButtonLeft): return buttonLeft;
			case((int)CAVE2Manager.Button.ButtonRight): return buttonRight;
			default: return buttonNull;
		}
	}
	
	public void UpdateMocap( Vector3 position, Quaternion orientation )
	{
		this.position = position;
		this.rotation = orientation;
	}
	
	public void UpdateButton( int buttonID, int buttonState )
	{		
		switch(buttonID)
		{
			case((int)CAVE2Manager.Button.Button1): button1 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button2): button2 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button3): button3 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button4): button4 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button5): button5 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button6): button6 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button7): button7 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button8): button8 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.Button9): button9 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.SpecialButton1): buttonSP1 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.SpecialButton2): buttonSP2 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.SpecialButton3): buttonSP3 = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.ButtonUp): buttonUp = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.ButtonDown): buttonDown = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.ButtonLeft): buttonLeft = (ButtonState)buttonState; break;
			case((int)CAVE2Manager.Button.ButtonRight): buttonRight = (ButtonState)buttonState; break;
		}
	}
	
	public void UpdateController( uint flags, Vector2 leftAnalogStick, Vector2 rightAnalogStick, Vector2 analogTrigger )
	{
		this.leftAnalogStick = leftAnalogStick;
		this.rightAnalogStick = rightAnalogStick;
		this.analogTrigger = analogTrigger;
		this.flags = (int)flags;

		// Update any state changes
		if( (flags & (int)CAVE2Manager.Button.Button1) == (int)CAVE2Manager.Button.Button1 && button1 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button1, (int)ButtonState.Down );
		else if( (flags & (int)CAVE2Manager.Button.Button1) == 0 && button1 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button1, (int)ButtonState.Up );
		
		if( (flags & (int)CAVE2Manager.Button.Button2) == (int)CAVE2Manager.Button.Button2 && button2 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button2, (int)ButtonState.Down );
		else if( (flags & (int)CAVE2Manager.Button.Button2) == 0 && button2 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button2, (int)ButtonState.Up );
			
		if( (flags & (int)CAVE2Manager.Button.Button3) == (int)CAVE2Manager.Button.Button3 && button3 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button3, (int)ButtonState.Down );
		else if( (flags & (int)CAVE2Manager.Button.Button3) == 0 && button3 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button3, (int)ButtonState.Up );
			
		if( (flags & (int)CAVE2Manager.Button.SpecialButton1) == (int)CAVE2Manager.Button.SpecialButton1 && buttonSP1 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.SpecialButton1, (int)ButtonState.Down );
		else if( (flags &  (int)CAVE2Manager.Button.SpecialButton1) == 0 && buttonSP1 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.SpecialButton1, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.SpecialButton2) == (int)EventBase.Flags.SpecialButton2 && buttonSP2 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.SpecialButton2, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.SpecialButton2) == 0 && buttonSP2 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.SpecialButton2, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.SpecialButton3) == (int)EventBase.Flags.SpecialButton3 && buttonSP3 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.SpecialButton3, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.SpecialButton3) == 0 && buttonSP3 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.SpecialButton3, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.Button4) == (int)EventBase.Flags.Button4 && button4 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button4, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.Button4) == 0 && button4 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button4, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.Button5) == (int)EventBase.Flags.Button5 && button5 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button5, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.Button5) == 0 && button5 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button5, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.Button6) == (int)EventBase.Flags.Button6 && button6 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button6, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.Button6) == 0 && button6 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button6, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.Button7) == (int)EventBase.Flags.Button7 && button7 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button7, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.Button7) == 0 && button7 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button7, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.ButtonUp) == (int)EventBase.Flags.ButtonUp && buttonUp != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.ButtonUp, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.ButtonUp) == 0 && buttonUp != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.ButtonUp, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.ButtonDown) == (int)EventBase.Flags.ButtonDown && buttonDown != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.ButtonDown, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.ButtonDown) == 0 && buttonDown != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.ButtonDown, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.ButtonLeft) == (int)EventBase.Flags.ButtonLeft && buttonLeft != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.ButtonLeft, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.ButtonLeft) == 0 && buttonLeft != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.ButtonLeft, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.ButtonRight) == (int)EventBase.Flags.ButtonRight && buttonRight != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.ButtonRight, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.ButtonRight) == 0 && buttonRight != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.ButtonRight, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.Button8) == (int)EventBase.Flags.Button8 && button8 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button8, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.Button8) == 0 && button8 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button8, (int)ButtonState.Up );
		
		if( (flags & (int)EventBase.Flags.Button9) == (int)EventBase.Flags.Button9 && button9 != ButtonState.Held )
			UpdateButton( (int)CAVE2Manager.Button.Button9, (int)ButtonState.Down );
		else if( (flags & (int)EventBase.Flags.Button9) == 0 && button8 != ButtonState.Idle)
			UpdateButton( (int)CAVE2Manager.Button.Button9, (int)ButtonState.Up );
			
	}
}