using UnityEngine;
using System.Collections;

public class CubeDrop : MonoBehaviour {

	getReal3D.ClusterView clusterView;
	
	public void Awake()
	{
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		//stream.Serialize( ref force );
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LaserHit()
	{
		clusterView.RPC ("Drop");
	}
	
	[getReal3D.RPC]
	void Drop()
	{
		rigidbody.useGravity = true;
	}
}
