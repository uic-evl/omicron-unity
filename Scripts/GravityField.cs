using UnityEngine;
using System.Collections;

public class GravityField : MonoBehaviour {

	public float force = 1;

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
		stream.Serialize( ref force );
	}
	// ----------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( force > 250 )
		{
			if( Application.HasProLicense() && Application.platform == RuntimePlatform.WindowsPlayer )
				clusterView.RPC("Explode");
			else
				Explode();
		}
		else if( force > 20 )
			force -= getReal3D.Cluster.deltaTime * 10;
	}

	void OnTriggerStay( Collider other )
	{
		other.rigidbody.AddForce( (transform.position - other.transform.position) * force );
	}

	void OnWandButtonHold()
	{
		force++;
	}

	[getReal3D.RPC]
	void Explode()
	{
		Destroy( gameObject );
	}
}
