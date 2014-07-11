using UnityEngine;
using System.Collections;

public class WandLaser : OmicronWandUpdater {
	
	public bool laserActivated;
	float laserDistance;
	bool laserHit;
	Vector3 laserPosition;

	LineRenderer laser;
	public Material laserMaterial;
	public Color laserColor = Color.red;
	


	public ParticleSystem laserParticlePrefab;
	ParticleSystem laserParticle;

	// Standard getReal3D Code Block ----------------------------------------------
	getReal3D.ClusterView clusterView;
	public void Awake()
	{
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		stream.Serialize( ref laserActivated );
		stream.Serialize( ref laserHit );
		stream.Serialize( ref laserPosition );
		stream.Serialize( ref laserDistance );
	}
	// ----------------------------------------------------------------------------

	// Use this for initialization
	new void Start () {
		InitOmicron();
		
		laser = gameObject.AddComponent<LineRenderer>();
		laser.SetWidth( 0.02f, 0.02f );
		laser.useWorldSpace = false;
		laser.material = laserMaterial;
		laser.SetColors( laserColor, laserColor );
		laser.castShadows = false;
		laser.receiveShadows = false;

		laserParticle = Instantiate(laserParticlePrefab) as ParticleSystem;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SphereCollider>().enabled = false;

		if( getReal3D.Cluster.isMaster )
		{
			laserActivated = cave2Manager.getWand(wandID).GetButton(CAVE2Manager.Button.Button3);
			laser.enabled = laserActivated;


			if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.Button3) )
			{

			}

			Ray ray = new Ray( transform.position, transform.TransformDirection(Vector3.forward) );
			RaycastHit hit;
			
			laserHit = Physics.Raycast(ray, out hit, 100);
			Debug.DrawLine(ray.origin, hit.point);

			if( laserActivated )
			{
				if (laserHit)
				{
		            Debug.DrawLine(ray.origin, hit.point);
					laserDistance = hit.distance;
					laserPosition = hit.point;
				}
				else
				{
					laserDistance = 1000;
				}
			}
			else if( laserHit ) // Pointing at an object but not 'shooting' it
			{
				hit.collider.gameObject.BroadcastMessage("OnWandOver", SendMessageOptions.DontRequireReceiver );
			}
			if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.Button3) )
			{
				if( hit.collider )
					hit.collider.gameObject.BroadcastMessage("WandClick", SendMessageOptions.DontRequireReceiver );
				laserDistance = hit.distance;
				laserPosition = hit.point;
			}

		}
		laser.enabled = laserActivated;
		if( laserActivated )
		{
			if (laserHit)
			{
				laserParticle.transform.position = laserPosition;
				laserParticle.Emit(1);
			}
			laser.SetPosition( 1, new Vector3( 0, 0, laserDistance ) );
		}

		GetComponent<SphereCollider>().enabled = true;
	}
}
