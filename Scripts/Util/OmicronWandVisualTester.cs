using UnityEngine;
using System.Collections;

public class OmicronWandVisualTester : OmicronWandUpdater {
	
	public Material litMaterial;
	public Material unlitMaterial;
	
	public GameObject buttonCross;
	public GameObject buttonCircle;
	public GameObject buttonUp;
	public GameObject buttonDown;
	public GameObject buttonLeft;
	public GameObject buttonRight;
	
	public bool sphereColliderEnabled = false;

	public bool useRPC = false;
	
	public bool crossPressed;
	public bool circlePressed;
	public bool upPressed;
	public bool downPressed;
	public bool leftPressed;
	public bool rightPressed;

	// Use this for initialization
	new void Start () {
		InitOmicron();
	}
	
	// Update is called once per frame
	void Update () {
		
		if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.Button3) )
			{
				crossPressed = true;
				SetButtonState((int)CAVE2Manager.Button.Button3, true);
			}
			else if( cave2Manager.getWand(wandID).GetButtonUp(CAVE2Manager.Button.Button3) )
			{
				crossPressed = false;
				SetButtonState((int)CAVE2Manager.Button.Button3, false);
			}
			
			if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.Button2) )
			{
				circlePressed = true;
				SetButtonState((int)CAVE2Manager.Button.Button2, true);
			}
			else if( cave2Manager.getWand(wandID).GetButtonUp(CAVE2Manager.Button.Button2) )
			{
				circlePressed = false;
				SetButtonState((int)CAVE2Manager.Button.Button2, false);
			}
			
			if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.ButtonUp) )
			{
				upPressed = true;
				SetButtonState((int)CAVE2Manager.Button.ButtonUp, true);
			}
			else if( cave2Manager.getWand(wandID).GetButtonUp(CAVE2Manager.Button.ButtonUp) )
			{
				upPressed = false;
				SetButtonState((int)CAVE2Manager.Button.ButtonUp, false);
			}
			if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.ButtonDown) )
			{
				downPressed = true;
				SetButtonState((int)CAVE2Manager.Button.ButtonDown, true);
			}
			else if( cave2Manager.getWand(wandID).GetButtonUp(CAVE2Manager.Button.ButtonDown) )
			{
				downPressed = false;
				SetButtonState((int)CAVE2Manager.Button.ButtonDown, false);
			}
			if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.ButtonLeft) )
			{
				leftPressed = true;
				SetButtonState((int)CAVE2Manager.Button.ButtonLeft, true);
			}
			else if( cave2Manager.getWand(wandID).GetButtonUp(CAVE2Manager.Button.ButtonLeft) )
			{
				leftPressed = false;
				SetButtonState((int)CAVE2Manager.Button.ButtonLeft, false);
			}
			if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.ButtonRight) )
			{
				rightPressed = true;
				SetButtonState((int)CAVE2Manager.Button.ButtonRight, true);
			}
			else if( cave2Manager.getWand(wandID).GetButtonUp(CAVE2Manager.Button.ButtonRight) )
			{
				rightPressed = false;
				SetButtonState((int)CAVE2Manager.Button.ButtonRight, false);
			}
	}

	void SetButtonState( int buttonID, bool lit )
	{
		switch(buttonID)
		{
			case((int)CAVE2Manager.Button.Button2):
				if( lit )
					buttonCircle.renderer.material = litMaterial;
				else
					buttonCircle.renderer.material = unlitMaterial;
				break;
			case((int)CAVE2Manager.Button.Button3):
				if( lit )
					buttonCross.renderer.material = litMaterial;
				else
					buttonCross.renderer.material = unlitMaterial;
				break;
			case((int)CAVE2Manager.Button.ButtonUp):
				if( lit )
					buttonUp.renderer.material = litMaterial;
				else
					buttonUp.renderer.material = unlitMaterial;
				break;
			case((int)CAVE2Manager.Button.ButtonDown):
				if( lit )
					buttonDown.renderer.material = litMaterial;
				else
					buttonDown.renderer.material = unlitMaterial;
				break;
			case((int)CAVE2Manager.Button.ButtonLeft):
				if( lit )
					buttonLeft.renderer.material = litMaterial;
				else
					buttonLeft.renderer.material = unlitMaterial;
				break;
			case((int)CAVE2Manager.Button.ButtonRight):
				if( lit )
					buttonRight.renderer.material = litMaterial;
				else
					buttonRight.renderer.material = unlitMaterial;
				break;
		}
	}
}
