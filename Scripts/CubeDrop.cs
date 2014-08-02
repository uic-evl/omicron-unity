using UnityEngine;
using System.Collections;

public class CubeDrop : MonoBehaviour {

	// Standard getReal3D Code Block ----------------------------------------------
	getReal3D.ClusterView clusterView;
	public void Awake()
	{
		// Adds a cluster view to this script
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		// These variables are constantly synced across the cluster
		//stream.Serialize( ref force );
	}
	// ----------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnWandButtonClick()
	{
		// Send message to cluster clients
		if( Application.HasProLicense() && Application.platform == RuntimePlatform.WindowsPlayer )
			clusterView.RPC("Drop");
		else
			Drop();
	}
	
	[getReal3D.RPC]
	void Drop()
	{
		rigidbody.useGravity = true;
	}
}
