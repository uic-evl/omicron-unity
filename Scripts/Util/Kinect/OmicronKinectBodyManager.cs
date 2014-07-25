using UnityEngine;
using System.Collections;
using omicron;
using omicronConnector;

public class OmicronKinectBodyManager : OmicronEventClient {

	public GameObject kinect2bodyPrefab;

	Hashtable trackedBodies;
	// Use this for initialization
	new void Start () {
		trackedBodies = new Hashtable ();
		InitOmicron ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnEvent( EventData e )
	{
		if (e.serviceType == EventBase.ServiceType.ServiceTypeMocap)
		{
			int sourceID = (int)e.sourceId;
			if( !trackedBodies.ContainsKey( sourceID ) )
			{
				GameObject body = Instantiate(kinect2bodyPrefab) as GameObject;
				body.transform.parent = transform;
				body.GetComponent<OmicronKinectEventClient>().bodyID = sourceID;

				trackedBodies.Add( sourceID, body );
			}
		}
	}
}
