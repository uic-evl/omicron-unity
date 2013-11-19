using UnityEngine;
using System.Collections;

public class LaserCannonScript : MonoBehaviour {
	public GameObject laser;
	
	public GameObject cannon0;
	public GameObject cannon1;
	public GameObject cannon2;
	public GameObject cannon3;
	
	public CrosshairScript crosshair;
	public bool convergenceEnabled = true;
	public bool autoTargetConvergence = true;
	GameObject convergencePoint;
	public float convergenceDistance = 200;
	public float verticalConvergenceAdjust = 0;
	
	int currentCannon = 0;
	int nCannons = 0;
	public bool fire = false;
	
	float lastShotTime = 0;
	public float shotDelay = 0.1f;
	
	public AudioClip cannonFiringSound;
	public float soundPitchMod = 1.0f;
	
	// Use this for initialization
	void Start () {
		convergencePoint = new GameObject("ConvergencePoint");
		convergencePoint.transform.position = transform.position;
		convergencePoint.transform.parent = transform;
		
		if( cannon0 != null )
			nCannons++;
		if( cannon1 != null )
			nCannons++;
		if( cannon2 != null )
			nCannons++;
		if( cannon3 != null )
			nCannons++;
	}
	
	// Update is called once per frame
	void Update () {
		crosshair.transform.localPosition = new Vector3( 0, verticalConvergenceAdjust, convergenceDistance );
		
		if( convergenceEnabled )
		{
			convergencePoint.transform.localPosition = new Vector3( 0, verticalConvergenceAdjust, convergenceDistance );
			
			Ray ray = new Ray(transform.position, transform.eulerAngles);
			ray.direction = transform.forward;
	        Debug.DrawRay(ray.origin, ray.direction * convergenceDistance, Color.white);
			
			RaycastHit hitInfo;
			if( Physics.Raycast(ray, out hitInfo, convergenceDistance ) )
			{
				//convergencePoint.transform.position = hitInfo.point;
				if( hitInfo.transform.gameObject.CompareTag("EnemyFighter") )
				{
					convergencePoint.transform.position = hitInfo.transform.position;
					crosshair.TargetLocked();
				}
				
			}
			else
			{
				crosshair.TargetLost();
			}
			
			if( cannon0 != null )
				cannon0.transform.LookAt(convergencePoint.transform);
			if( cannon1 != null )
				cannon1.transform.LookAt(convergencePoint.transform);
			if( cannon2 != null )
				cannon2.transform.LookAt(convergencePoint.transform);
			if( cannon3 != null )
				cannon3.transform.LookAt(convergencePoint.transform);
		}
		else
		{
			if( cannon0 != null )
				cannon0.transform.localRotation = Quaternion.identity;
			if( cannon1 != null )
				cannon1.transform.localRotation = Quaternion.identity;
			if( cannon2 != null )
				cannon2.transform.localRotation = Quaternion.identity;
			if( cannon3 != null )
				cannon3.transform.localRotation = Quaternion.identity;
		}
	
		if( !fire && lastShotTime + shotDelay < Time.time )
			fire = getReal3D.Input.GetButton("WandButton");
		
		if(fire){
			lastShotTime = Time.time;
			
			if( currentCannon == 0 && cannon0 != null )
				GameObject.Instantiate(laser, cannon0.transform.position, cannon0.transform.rotation);
			else if( currentCannon == 1 && cannon1 != null )
				GameObject.Instantiate(laser, cannon1.transform.position, cannon1.transform.rotation);
			else if( currentCannon == 2 && cannon2 != null )
				GameObject.Instantiate(laser, cannon2.transform.position, cannon2.transform.rotation);
			else if( currentCannon == 3 && cannon3 != null )
				GameObject.Instantiate(laser, cannon3.transform.position, cannon3.transform.rotation);
			
			fire = false;
			
			currentCannon++;
			if( currentCannon > nCannons - 1 )
				currentCannon = 0;
			
			if( cannonFiringSound )
			{
				float jitter = (soundPitchMod * (Random.Range(-10,10) / 250.0f));
				
				GameObject audioObject = new GameObject("One Shot Audio");
				audioObject.transform.position = transform.position;
				SoundInstanceScript soundScript = audioObject.AddComponent<SoundInstanceScript>();
				soundScript.clip = cannonFiringSound;
				soundScript.pitch = 1.0f + jitter;
			}
		}
	}
	
	public void SetParentSpeed( float value )
	{
		laser.GetComponent<ProjectileScript>().SetParentSpeed(value);
	}
}
