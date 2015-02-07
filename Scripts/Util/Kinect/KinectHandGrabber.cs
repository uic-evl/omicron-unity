using UnityEngine;
using System.Collections;

public class KinectHandGrabber : MonoBehaviour {

	public int handState;

	int lastKnownHandState;

	public bool grabbing;
	public bool holdingObject;
	
	public bool hasGrabableObject;
	public GameObject grabableObject;
	public Transform originalParent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if( handState == (int)OmicronKinectEventClient.KinectHandState.Open )
		{
			lastKnownHandState = (int)OmicronKinectEventClient.KinectHandState.Open;
			grabbing = false;

			if( holdingObject )
			{
				ReleaseObject();
			}
		}
		else if( handState == (int)OmicronKinectEventClient.KinectHandState.Closed )
		{
			lastKnownHandState = (int)OmicronKinectEventClient.KinectHandState.Closed;
			grabbing = true;
		}

	}

	void OnTriggerStay( Collider other )
	{

	}

	void SelectGrabbableObject( GameObject otherGameObject )
	{
		grabableObject = otherGameObject;
	}

	void GrabObject()
	{
		originalParent = grabableObject.transform.parent;
		grabableObject.rigidbody.isKinematic = true;
		
		grabableObject.transform.parent = transform;
		
		//grabJoint = grabableObject.AddComponent<FixedJoint>();
		//grabJoint.connectedBody = rigidbody;
		//grabJoint.breakForce = Mathf.Infinity;
		//grabJoint.breakTorque = Mathf.Infinity;
		
		holdingObject = true;
	}

	void ReleaseObject()
	{
		grabableObject.rigidbody.isKinematic = false;
		if( originalParent != transform )
			grabableObject.transform.parent = originalParent;
		else
			grabableObject.transform.parent = null;
		//grabJoint = null;
		
		holdingObject = false;
		hasGrabableObject = false;
		grabableObject = null;
	}
}
