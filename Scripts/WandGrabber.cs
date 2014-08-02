using UnityEngine;
using System.Collections;

public class WandGrabber : OmicronWandUpdater {

	public CAVE2Manager.Button grabButton;

	public float grabDistance = 0.1f;

	public Collider grabCollider;

	public bool grabbing;
	public bool holdingObject;

	public bool hasGrabableObject;
	public GameObject grabableObject;
	public Transform originalParent;

	public GameObject visualCollider;


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
	new void Start () {
		InitOmicron(); // Required OmicronWandUpdater function

	}
	
	// Update is called once per frame
	void Update () {
		grabCollider.isTrigger = true;

		if( getReal3D.Cluster.isMaster )
		{
			grabbing = cave2Manager.getWand(wandID).GetButton(grabButton);
		}


		// Shoot a ray from the wand
		Ray ray = new Ray( transform.position, transform.TransformDirection(Vector3.forward) );
		RaycastHit hit;
		
		// Get the first collider that was hit by the ray
		bool wandHit = Physics.Raycast(ray, out hit, 100);
		Debug.DrawLine(ray.origin, hit.point); // Draws a line in the editor
		
		if( wandHit && hit.collider.gameObject.GetComponent<GrabableObject>() )
		{
			if( hit.distance <= grabDistance && !holdingObject )
			{
				hasGrabableObject = true;
				if( getReal3D.Cluster.isMaster && grabbing )
				{
					clusterView.RPC ("SelectGrabbableObject", hit.collider.gameObject.GetComponent<getReal3D.ClusterView>() );
					clusterView.RPC ("GrabObject");
				}
			}
			else if( hit.distance > 0.1f )
			{
				hasGrabableObject = false;
				if( grabableObject == hit.collider.gameObject )
				{
					clusterView.RPC ("DeselectGrabbableObject");
				}
			}
		}

		if( getReal3D.Cluster.isMaster && !grabbing && hasGrabableObject && holdingObject )
		{
			clusterView.RPC ("ReleaseObject");
		}

		if( visualCollider )
		{
			if( hasGrabableObject )
				visualCollider.renderer.material.color = new Color(10/255.0f, 250/255.0f, 250/255.0f, 128/255.0f);
			else if( holdingObject )
				visualCollider.renderer.material.color = new Color(10/255.0f, 250/255.0f, 10/255.0f, 0/255.0f);
			else if( grabbing )
				visualCollider.renderer.material.color = new Color(10/255.0f, 250/255.0f, 10/255.0f, 128/255.0f);
			else
				visualCollider.renderer.material.color = new Color(10/255.0f, 250/255.0f, 10/255.0f, 64/255.0f);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if( other.GetComponent<GrabableObject>() && !hasGrabableObject )
		{
			//hasGrabableObject = true;
			//grabableObject = other.gameObject;
			//clusterView.RPC ("SelectGrabbableObject", other.GetComponent<getReal3D.ClusterView>() );
		}
	}

	void OnTriggerExit(Collider other)
	{
		if( other.gameObject == grabableObject )
		{
			//hasGrabableObject = false;
			//grabableObject = null;
			//clusterView.RPC ("ReleaseObject");
		}
	}

	[getReal3D.RPC]
	void SelectGrabbableObject( getReal3D.ClusterView otherCV )
	{
		grabableObject = otherCV.gameObject;
	}

	[getReal3D.RPC]
	void DeselectGrabbableObject()
	{
		grabableObject = null;
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
