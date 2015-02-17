/**************************************************************************************************
* THE OMICRON PROJECT
 *-------------------------------------------------------------------------------------------------
 * Copyright 2010-2014		Electronic Visualization Laboratory, University of Illinois at Chicago
 * Authors:										
 *  Arthur Nishimoto		anishimoto42@gmail.com
 *-------------------------------------------------------------------------------------------------
 * Copyright (c) 2010-2014, Electronic Visualization Laboratory, University of Illinois at Chicago
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
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES; LOSS OF 
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *************************************************************************************************/
 
/*
 * OmicronInputScript handles all network connentions to the input server and sends events out to objects tagged as 'OmicronListener'.
 * Currently supported input servers:
 * 		- Omicron oinputserver
 *		- OmicronInputConnector
 */
using UnityEngine;
using System.Collections;

using omicronConnector;
using omicron;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class TouchPoint{

	Vector3 position;
	int ID;
	EventBase.Type gesture;
	Ray touchRay = new Ray();
	RaycastHit touchHit;
	long timeStamp;
	GameObject objectTouched;
	
	public TouchPoint(Vector2 pos, int id){
		position = pos;
		ID = id;
		touchRay = Camera.main.ScreenPointToRay(position);
		gesture = EventBase.Type.Null;
		timeStamp = (long)Time.time;
	}
	
	public Vector3 GetPosition(){
		return position;
	}
	
	public Ray GetRay(){
		return touchRay;
	}
	
	public int GetID(){
		return ID;
	}
	
	public long GetTimeStamp(){
		return timeStamp;
	}
	
	public EventBase.Type GetGesture(){
		return gesture;
	}
	
	public RaycastHit GetRaycastHit(){
		 return touchHit;
	}
	
	public GameObject GetObjectTouched(){
		 return objectTouched;
	}
	
	public void SetGesture(EventBase.Type value){
		 gesture = value;
	}
	
	public void SetRaycastHit(RaycastHit value){
		 touchHit = value;
	}
	
	public void SetObjectTouched(GameObject value){
		 objectTouched = value;
	}
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
class EventListener : IOmicronConnectorClientListener
{
	OmicronManager parent;
	
	public EventListener( OmicronManager p )
	{
		parent = p;
	}
	
	public override void onEvent(EventData e)
	{
		parent.AddEvent(e);
	}// onEvent
	
}// EventListener

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
class OmicronManager : MonoBehaviour
{
	EventListener omicronListener;
	OmicronConnectorClient omicronManager;
	public bool connectToServer = false;
	public string serverIP = "localhost";
	public int serverMsgPort = 28000;
	public int dataPort = 7013;
	
	public bool debug = false;
	
		
	// Use mouse clicks to emulate touches
	public bool mouseTouchEmulation = false;
	
	// List storing events since we have multiple threads
	private ArrayList eventList;
	
	private ArrayList omicronClients;

	
	int connectStatus = 0;

	// Initializations
	public void Start()
	{
		omicronListener = new EventListener(this);
		omicronManager = new OmicronConnectorClient(omicronListener);
		
		eventList = new ArrayList();
		
		gameObject.tag = "OmicronManager";
		
		if( connectToServer )
		{
			ConnectToServer();

			CAVE2Manager cave2Manager = GameObject.FindGameObjectWithTag("OmicronManager").GetComponent<CAVE2Manager>();
			cave2Manager.keyboardEventEmulation = false;
			cave2Manager.wandMousePointerEmulation = false;
		}
		else if( !connectToServer )
		{
			
			getRealCameraUpdater getRealCam = Camera.main.GetComponent<getRealCameraUpdater>();
			if( getRealCam )
			{
				getRealCam.applyHeadPosition = false;
				getRealCam.applyHeadRotation = false;
				getRealCam.applyCameraProjection = false;
			}
			
		}
	}// start

	public bool ConnectToServer()
	{
		connectToServer = omicronManager.Connect( serverIP, serverMsgPort, dataPort );

		if( connectToServer )
			connectStatus = 1;
		else
			connectStatus = -1;

		return connectToServer;
	}

	public void DisconnectServer()
	{
		omicronManager.Dispose ();
		connectStatus = 0;
		connectToServer = false;
		Debug.Log("InputService: Disconnected");
	}

	public void AddClient( OmicronEventClient c )
	{
		if( omicronClients != null )
			omicronClients.Add(c);
		else
		{
			// First run case since client may attempt to connect before
			// OmicronManager Start() is called
			omicronClients = new ArrayList();
			omicronClients.Add(c);
		}
	}
	
	public void AddEvent( EventData e )
	{
		lock(eventList.SyncRoot)
		{
			eventList.Add(e);
			if( debug )
			{
				Debug.Log("OmicronInputScript: New event ID: " + e.sourceId);
				Debug.Log("   type" + e.serviceType);
			}
		}
	}
	
	public void Update()
	{
		if( mouseTouchEmulation )
			{
				Vector2 position = new Vector3( Input.mousePosition.x, Input.mousePosition.y );
						
				// Ray extending from main camera into screen from touch point
				Ray touchRay = Camera.main.ScreenPointToRay(position);
				Debug.DrawRay(touchRay.origin, touchRay.direction * 10, Color.white);
						
				TouchPoint touch = new TouchPoint(position, -1);
				
				if( Input.GetMouseButtonDown(0) )
					touch.SetGesture( EventBase.Type.Down );
				else if( Input.GetMouseButtonUp(0) )
					touch.SetGesture( EventBase.Type.Up );
				else if( Input.GetMouseButton(0) )
					touch.SetGesture( EventBase.Type.Move );
				
				//GameObject[] touchObjects = GameObject.FindGameObjectsWithTag("OmicronListener");
				//foreach (GameObject touchObj in touchObjects) {
				//	touchObj.BroadcastMessage("OnTouch",touch,SendMessageOptions.DontRequireReceiver);
				//}
			}
			
			lock(eventList.SyncRoot)
			{
				foreach( EventData e in eventList )
				{
					ArrayList activeClients = new ArrayList();
					foreach( OmicronEventClient c in omicronClients )
					{
						if( !c.IsFlaggedForRemoval() )
						{
							c.BroadcastMessage("OnEvent",e,SendMessageOptions.DontRequireReceiver);
							activeClients.Add(c);
						}
					}
					omicronClients = activeClients;
					/*
					if( (EventBase.ServiceType)e.serviceType == EventBase.ServiceType.ServiceTypePointer )
					{
						// 2D position of the touch, flipping y-coordinates
						Vector2 position = new Vector3( e.posx * Screen.width, Screen.height - e.posy * Screen.height );
						
						// Ray extending from main camera into screen from touch point
						Ray touchRay = Camera.main.ScreenPointToRay(position);
						Debug.DrawRay(touchRay.origin, touchRay.direction * 10, Color.white);
						
						TouchPoint touch = new TouchPoint(position, (int)e.sourceId);
						touch.SetGesture( (EventBase.Type)e.type ); 
						
						//GameObject[] touchObjects = GameObject.FindGameObjectsWithTag("OmicronListener");
						//foreach (GameObject touchObj in touchObjects) {
						//	touchObj.BroadcastMessage("OnTouch",touch,SendMessageOptions.DontRequireReceiver);
						//}
					}
					
					else
					{
						//GameObject[] omicronObjects = GameObject.FindGameObjectsWithTag("OmicronListener");
						//foreach (GameObject obj in omicronObjects) {
						//	obj.BroadcastMessage("OnEvent",e,SendMessageOptions.DontRequireReceiver);
						//}
					}
					*/
				}
				
				// Clear the list (TODO: probably should set the Processed flag instead and cleanup elsewhere)
				eventList.Clear();
			}
	}
	
	void OnApplicationQuit()
    {
		if( connectToServer ){
			DisconnectServer();
		}
    }

	// GUI
	Rect windowRect = new Rect(0, 0, 250 , 300);
	string[] navStrings = new string[] {"Walk", "Drive", "Freefly"};
	string[] horzStrings = new string[] {"Strafe", "Turn"};
	string[] forwardRefStrings = new string[] {"CAVE", "Head", "Wand"};

	GUIStyle idleStatus = new GUIStyle();
	GUIStyle activeStatus = new GUIStyle();
	GUIStyle errorStatus = new GUIStyle();
	GUIStyle currentStatus;
	Vector2 GUIOffset;
	
	public void SetGUIOffSet( Vector2 offset )
	{
		GUIOffset = offset;
    }

	void OnGUI()
	{
		//windowRect = GUI.Window(-1, windowRect, OnWindow, "OmicronManager");			
	}

	public void OnWindow(int windowID)
	{
		float rowHeight = 25;

		idleStatus.normal.textColor = Color.white;
		activeStatus.normal.textColor = Color.green;
		errorStatus.normal.textColor = Color.red;

        currentStatus = idleStatus;
        
        string statusText = "UNKNOWN";
        switch (connectStatus)
        {
            case(0): currentStatus = idleStatus; statusText = "Not Connected"; break;
            case(1): currentStatus = activeStatus; statusText = "Connected"; break;
            case(-1): currentStatus = errorStatus; statusText = "Failed to Connect"; break;
        }

		if( GUI.Toggle (new Rect (GUIOffset.x + 20, GUIOffset.y + rowHeight * 0, 250, 20), connectToServer, "Connect to Server:") )
		{
			if( currentStatus != activeStatus )
				ConnectToServer();
		}
		else
		{
			if( currentStatus == activeStatus )
				DisconnectServer();
        }

        GUI.Label (new Rect (GUIOffset.x + 150, GUIOffset.y + rowHeight * 0 + 3, 250, 200), statusText, currentStatus);

		GUI.Label(new Rect(GUIOffset.x + 25, GUIOffset.y + rowHeight * 1, 120, 20), "Omicron Server IP:");
		serverIP = GUI.TextField(new Rect(GUIOffset.x + 150, GUIOffset.y + rowHeight * 1, 200, 20), serverIP, 25);

		GUI.Label(new Rect(GUIOffset.x + 25, GUIOffset.y + rowHeight * 2, 120, 20), "Server Message Port:");
		serverMsgPort = int.Parse(GUI.TextField(new Rect(GUIOffset.x + 150, GUIOffset.y + rowHeight * 2, 200, 20), serverMsgPort.ToString(), 25));

		GUI.Label(new Rect(GUIOffset.x + 25, GUIOffset.y + rowHeight * 3, 120, 20), "Data Port:");
		dataPort = int.Parse(GUI.TextField(new Rect(GUIOffset.x + 150, GUIOffset.y + rowHeight * 3, 200, 20), dataPort.ToString(), 25));

	}
}// class