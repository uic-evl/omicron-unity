using UnityEngine;
using System.Collections;

public class GrabableObject : MonoBehaviour {

	/// <summary>
	/// Helper script for creating cluster synced transforms and rigidbodies
	/// </summary>
	/// 
	/// 
	// Standard getReal3D Code Block ----------------------------------------------
	public getReal3D.ClusterView clusterView;
	public void Awake()
	{
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;

		// Check existing ClusterViews to prevent duplicates
		getReal3D.ClusterView[] existingClusterViews = gameObject.GetComponents<getReal3D.ClusterView>();
		bool observingRigidbody = false;
		bool observingTransform = false;

		foreach( getReal3D.ClusterView cv in existingClusterViews )
		{
			if( cv.observed == rigidbody )
				observingRigidbody = true;
			if( cv.observed == transform )
				observingTransform = true;
		}

		if( !observingRigidbody )
		{
			getReal3D.ClusterView clusterView2 = gameObject.AddComponent<getReal3D.ClusterView>();
			clusterView2.observed = rigidbody;
		}
		if( !observingTransform )
		{
			getReal3D.ClusterView clusterView3 = gameObject.AddComponent<getReal3D.ClusterView>();
			clusterView3.observed = transform;
		}
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		//stream.Serialize( ref grabbing );
	}
	// ----------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
