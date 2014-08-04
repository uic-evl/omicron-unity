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

	// Standard getReal3D Code Block ----------------------------------------------
	getReal3D.ClusterView clusterView;
	public void Awake()
	{
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		stream.Serialize( ref grabbing );
		stream.Serialize( ref holdingObject );
	}
	// ----------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if( handState == (int)OmicronKinectEventClient.KinectHandState.Open )
		{
			lastKnownHandState = (int)OmicronKinectEventClient.KinectHandState.Open;
			grabbing = false;

			if( getReal3D.Cluster.isMaster && holdingObject )
			{
				if( Application.HasProLicense() && Application.platform == RuntimePlatform.WindowsPlayer )
				{
					clusterView.RPC ("ReleaseObject");
				}
				else
				{
					ReleaseObject();
				}
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
		if( other.GetComponent<GrabableObject>() )
		{
			if( getReal3D.Cluster.isMaster && grabbing && !holdingObject )
			{
				if( Application.HasProLicense() && Application.platform == RuntimePlatform.WindowsPlayer )
				{
					clusterView.RPC ("SelectGrabbableObjectRPC", other.GetComponent<getReal3D.ClusterView>() );
					clusterView.RPC ("GrabObject");
				}
				else
				{
					SelectGrabbableObject( other.gameObject );
					GrabObject();
				}
			}
		}
	}

	[getReal3D.RPC]
	void SelectGrabbableObjectRPC( getReal3D.ClusterView otherCV )
	{
		grabableObject = otherCV.gameObject;
	}
	
	void SelectGrabbableObject( GameObject otherGameObject )
	{
		grabableObject = otherGameObject;
	}


	[getReal3D.RPC]
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

	[getReal3D.RPC]
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
