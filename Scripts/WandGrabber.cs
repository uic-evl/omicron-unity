using UnityEngine;
using System.Collections;

public class WandGrabber : OmicronWandUpdater {

	public CAVE2Manager.Button grabButton;

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
		if( getReal3D.Cluster.isMaster )
		{
			grabbing = cave2Manager.getWand(wandID).GetButton(grabButton);

			grabCollider.isTrigger = (grabbing || holdingObject);

			if( !grabbing && hasGrabableObject && !holdingObject )
			{
				originalParent = grabableObject.transform.parent;
				grabableObject.rigidbody.isKinematic = true;
				grabableObject.transform.parent = transform;
				holdingObject = true;
			}
			else if( grabbing && hasGrabableObject && holdingObject )
			{
				grabableObject.rigidbody.isKinematic = false;
				grabableObject.transform.parent = originalParent;
				holdingObject = false;
				hasGrabableObject = false;
			}
		}

		if( visualCollider )
		{
			if( grabbing && !holdingObject && hasGrabableObject )
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
		if( other.gameObject.rigidbody && other.GetComponent<getReal3D.ClusterView>() && !hasGrabableObject )
		{
			clusterView.RPC ("GrabObject", other.GetComponent<getReal3D.ClusterView>() );
			
		}
	}

	void OnTriggerExit(Collider other)
	{
		if( other.gameObject.rigidbody && other.gameObject == grabableObject )
		{
			clusterView.RPC ("ReleaseObject");
		}
	}

	[getReal3D.RPC]
	void GrabObject( getReal3D.ClusterView otherCV )
	{
		hasGrabableObject = true;
		grabableObject = otherCV.gameObject;
	}

	[getReal3D.RPC]
	void ReleaseObject()
	{
		hasGrabableObject = false;
	}
}
