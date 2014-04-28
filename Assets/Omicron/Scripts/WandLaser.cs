using UnityEngine;
using System.Collections;

public class WandLaser : OmicronWandUpdater {
	
	public bool laserActivated;
	float laserDistance;
	
	LineRenderer laser;
	public Material laserMaterial;
	public Color laserColor = Color.red;
	
	getReal3D.ClusterView clusterView;

	public void Awake()
	{
		clusterView = gameObject.GetComponent<getReal3D.ClusterView>();
		if( clusterView == null )
		{
			gameObject.AddComponent<getReal3D.ClusterView>();
			clusterView = gameObject.GetComponent<getReal3D.ClusterView>();
			clusterView.observed = this;
		}
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		stream.Serialize( ref laserActivated );
		stream.Serialize( ref laserActivated );
	}
	
	// Use this for initialization
	new void Start () {
		InitOmicron();
		
		laser = gameObject.AddComponent<LineRenderer>();
		laser.SetWidth( 0.02f, 0.02f );
		laser.useWorldSpace = false;
		laser.material = laserMaterial;
		laser.SetColors( laserColor, laserColor );
	}
	
	// Update is called once per frame
	void Update () {
		if( getReal3D.Cluster.isMaster )
		{
			
			
			laserActivated = cave2Manager.getWand(wandID).GetButton(CAVE2Manager.Button.Button3);
			laser.enabled = laserActivated;
			
			if( laserActivated )
			{
				Ray ray = new Ray( transform.position, transform.TransformDirection(Vector3.forward) );
				RaycastHit hit;
				laserDistance = 1000;
		        if (Physics.Raycast(ray, out hit, 100))
				{
		            Debug.DrawLine(ray.origin, hit.point);
					laserDistance = hit.distance;
					
				}
				laser.SetPosition( 1, new Vector3( 0, 0, laserDistance ) );
			}
		}
	}
}
