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

		getReal3D.ClusterView clusterView2 = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView2.observed = rigidbody;

		getReal3D.ClusterView clusterView3 = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView3.observed = transform;
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
