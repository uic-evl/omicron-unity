using UnityEngine;
using System.Collections;

public class WandLaser : OmicronWandUpdater {
	
	public bool laserActivated;
	float laserDistance;
	bool wandHit;
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
		// Adds a cluster view to this script
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		// These variables are constantly synced across the cluster
		// and are based on input events handled in Update()
		stream.Serialize( ref laserActivated );
		stream.Serialize( ref wandHit );
		stream.Serialize( ref laserPosition );
		stream.Serialize( ref laserDistance );
	}
	// ----------------------------------------------------------------------------

	// Use this for initialization
	new void Start () {
		InitOmicron();

		// Laser line
		laser = gameObject.AddComponent<LineRenderer>();
		laser.SetWidth( 0.02f, 0.02f );
		laser.useWorldSpace = false;
		laser.material = laserMaterial;
		laser.SetColors( laserColor, laserColor );
		laser.castShadows = false;
		laser.receiveShadows = false;

		// Laser impact glow
		laserParticle = Instantiate(laserParticlePrefab) as ParticleSystem;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SphereCollider>().enabled = false; // Disable sphere collider for raycast

		// Only occurs on the master node
		if( getReal3D.Cluster.isMaster )
		{
			// Checking inputs should only be done on master node
			laserActivated = cave2Manager.getWand(wandID).GetButton(CAVE2Manager.Button.Button3);
			laser.enabled = laserActivated;

			// Shoot a ray from the wand
			Ray ray = new Ray( transform.position, transform.TransformDirection(Vector3.forward) );
			RaycastHit hit;

			// Get the first collider that was hit by the ray
			wandHit = Physics.Raycast(ray, out hit, 100);
			Debug.DrawLine(ray.origin, hit.point); // Draws a line in the editor

			if( wandHit ) // The wand is pointed at a collider
			{
				// Send a message to the hit object telling it that the wand is hovering over it
				hit.collider.gameObject.SendMessage("OnWandOver", SendMessageOptions.DontRequireReceiver );

				// If the laser button has just been pressed, tell the hit object
				if( cave2Manager.getWand(wandID).GetButtonDown(CAVE2Manager.Button.Button3) )
				{
					hit.collider.gameObject.SendMessage("OnWandButtonClick", SendMessageOptions.DontRequireReceiver );
				}

				// Laser button is held down
				if( laserActivated )
				{
					// Tell hit object laser button is held down
					hit.collider.gameObject.SendMessage("OnWandButtonHold", SendMessageOptions.DontRequireReceiver );

					Debug.DrawLine(ray.origin, hit.point);
					// Set the laser distance at the collision point
					laserDistance = hit.distance;
					laserPosition = hit.point;
				}
			}
			else if( laserActivated )
			{
				// The laser button is pressed, but not pointed at valid target
				// Set laser distance far away
				laserDistance = 1000;
			}
		}

		// Do this on all nodes
		laser.enabled = laserActivated;
		if( laserActivated )
		{
			if (wandHit)
			{
				laserParticle.transform.position = laserPosition;
				laserParticle.Emit(1);
			}
			laser.SetPosition( 1, new Vector3( 0, 0, laserDistance ) );
		}

		//GetComponent<SphereCollider>().enabled = true; // Enable sphere collider after raycast
	}
}
