using UnityEngine;
using System.Collections;

public class GravityField : MonoBehaviour {

	public float force = 1;

	getReal3D.ClusterView clusterView;

	public void Awake()
	{
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		stream.Serialize( ref force );
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( force > 250 )
			clusterView.RPC ("Explode");
		else if( force > 20 )
			force -= getReal3D.Cluster.deltaTime * 10;
	}

	void OnTriggerStay( Collider other )
	{
		other.rigidbody.AddForce( (transform.position - other.transform.position) * force );
	}

	void LaserHit()
	{
		Debug.Log("Field hit by laser!");
		force++;
	}

	[getReal3D.RPC]
	void Explode()
	{
		Destroy( gameObject );
	}
}
