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
* 
* This is a modification of the getReal3D's getRealWalkthruController
*************************************************************************************************/

using UnityEngine;
using System.Collections;
using getReal3D;

//[AddComponentMenu("getReal3D/Navigation/Walkthru Controller")]
public class OmicronGetRealWalkthruController
	: MonoBehaviour
{
	private CharacterMotor m_motor;
	private CharacterController m_controller;
	private Transform m_transform;
	private Vector3 m_lastGroundedPosition = Vector3.zero;
	
	public float TranslationSpeed = 1.0f;
	//public string forwardAxis = "Forward";
	//public string strafeAxis = "Strafe";
	//public string navSpeedButton = "NavSpeed";
	//public string resetButton = "Reset";
	//public string jumpButton = "Jump";
	
	
	public CAVE2Manager.Axis forwardAxis = CAVE2Manager.Axis.LeftAnalogStickUD;
	public CAVE2Manager.Axis strafeAxis = CAVE2Manager.Axis.LeftAnalogStickLR;
	
	public CAVE2Manager.Button navSpeedButton = CAVE2Manager.Button.Button3;
	public CAVE2Manager.Button resetButton = CAVE2Manager.Button.Button2;
	public CAVE2Manager.Button jumpButton = CAVE2Manager.Button.Button1;
	
    public enum NavFollows { Wand, Head, Reference };
    public NavFollows navFollows = NavFollows.Wand;
    public Transform navReference = null;
	
	CAVE2Manager cave2Manager;
	public int headID = 1;
	public int wandID = 1;
	
	// Use this for initialization
	public void Start () {
		cave2Manager = GameObject.FindGameObjectWithTag("OmicronManager").GetComponent<CAVE2Manager>();
	}
	

// Use this for initialization
	void Awake()
	{
	    m_transform = transform;
		m_motor = GetComponent<CharacterMotor>();
		m_controller = GetComponent<CharacterController>();
		m_lastGroundedPosition = m_transform.position;
        //if (!getReal3D.Input.NavOptions.HasValue("TranslationSpeed"))
		//{
        //    getReal3D.Input.NavOptions.SetValue<float>("TranslationSpeed", TranslationSpeed);
        //}
	}
		
	void OnEnable()
	{
		if (m_controller)
		{
			m_controller.enabled = true;
		}
		if (m_motor != null)
		{
			m_motor.SetControllable(true);
			m_motor.enabled = true;
		}
	}

    void Update()
    {
        doNavigation(Time.smoothDeltaTime);		
    }

    void doNavigation(float elapsed)
	{
		if (m_motor != null && m_motor.grounded)
			m_lastGroundedPosition = m_transform.position;

		Vector3 joy = new Vector3( cave2Manager.getWand(wandID).GetAxis(strafeAxis), 0, cave2Manager.getWand(wandID).GetAxis(forwardAxis));

		if (Cluster.isMaster || !Cluster.isOn)
		{
            UpdateNavigation(joy, elapsed);
		}

 		if (cave2Manager.getWand(wandID).GetButtonDown(resetButton))
 		{
 			doReset();
 		}
 	}

	private void doReset()
	{
		if (Cluster.isMaster || !Cluster.isOn)
		{
			m_transform.position = m_lastGroundedPosition;
			if (m_motor != null)
			{
				m_motor.inputMoveDirection = Vector3.zero;
				m_motor.inputJump = false;
			}
		}
	}
	
	void UpdateNavigation(Vector3 joy, float elapsed)
    {
        // Get the input vector from keyboard or analog stick
        Vector3 directionVector = joy;
        switch (navFollows)
        {
        case NavFollows.Wand:
            directionVector = cave2Manager.getWand(wandID).rotation * directionVector;
            break;
        case NavFollows.Head:
            directionVector = cave2Manager.getHead(headID).rotation * directionVector;
            break;
        case NavFollows.Reference:
            if (navReference != null)
                directionVector = (navReference.localToWorldMatrix * m_transform.worldToLocalMatrix).MultiplyVector(directionVector);
            break;
        }
		directionVector -= Vector3.Dot(directionVector, Physics.gravity.normalized) * Physics.gravity.normalized;

        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;
		
            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1, directionLength);
		
            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;
		
            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }

        //getReal3D.Input.NavOptions.GetValue<float>("TranslationSpeed", ref TranslationSpeed);
        directionVector *= TranslationSpeed;
        //if (!cave2Manager.getWand(wandID).GetButton(navSpeedButton))
        //    directionVector *= getReal3D.Scale.worldScale;

        // Apply the direction to the CharacterMotor, CharacterController, or Transform, as available
        if (m_motor != null && m_motor.enabled && m_motor.canControl)
        {
            m_motor.inputMoveDirection = m_transform.rotation * directionVector;
            m_motor.inputJump = cave2Manager.getWand(wandID).GetButtonDown(jumpButton);
        }
        else if (m_controller != null && m_controller.enabled)
        {
            CollisionFlags flags = m_controller.Move(m_transform.TransformDirection(directionVector) * elapsed);
            bool grounded = (flags & CollisionFlags.CollidedBelow) != 0;
            if (grounded) m_lastGroundedPosition = m_transform.position;
        }
        else
        {
            m_transform.position += m_transform.TransformDirection(directionVector) * elapsed;
        }
    }
}