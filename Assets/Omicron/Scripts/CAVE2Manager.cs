using UnityEngine;
using System.Collections;
using omicron;
using omicronConnector;

public class HeadTrackerState
{
	public int sourceID;
	public Vector3 position;
	public Quaternion orientation;
	
	public HeadTrackerState(int ID)
	{
		sourceID = ID;
		position = new Vector3();
		orientation = new Quaternion();
	}
	
	public void Update( Vector3 position, Quaternion orientation )
	{
		this.position = position;
		this.orientation = orientation;
	}
}

class WandState
{
	public int sourceID;
	public Vector3 position;
	public Quaternion orientation;
	uint flags;
	
	public WandState(int ID)
	{
		sourceID = ID;
		position = new Vector3();
		orientation = new Quaternion();
	}
	
	public void Update( Vector3 position, Quaternion orientation, Vector2 leftAnalog, Vector2 rightAnalog, Vector2 analogTrigger, uint flags )
	{
		this.position = position;
		this.orientation = orientation;
	}
}

public class CAVE2Manager : OmicronEventClient {
	
	HeadTrackerState head1;
	HeadTrackerState head2;
	
	WandState wand1;
	WandState wand2;
	
	// Use this for initialization
	new void Start () {
		base.Start();
		
		head1 = new HeadTrackerState(0);
		head2 = new HeadTrackerState(4);
		
		wand1 = new WandState(1);
	}
	
	// Update is called once per frame
	void Update () {
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
		}
		else if( e.serviceType == EventBase.ServiceType.ServiceTypeWand )
		{
			// -zPos -xRot -yRot for Omicron->Unity coordinate conversion)
			Vector3 unityPos = new Vector3(e.posx, e.posy, -e.posz);
			Quaternion unityRot = new Quaternion(-e.orx, -e.ory, e.orz, e.orw);
			
			if( e.sourceId == wand1.sourceID )
			{
				//wand1.Update( unityPos, unityRot, e.flags );
			}
		}
		
	}
	
	public HeadTrackerState getHead(int ID)
	{
		if( ID == 2 )
			return head2;
		else
			return head1;
	}
}
