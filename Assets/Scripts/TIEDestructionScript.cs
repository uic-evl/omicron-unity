using UnityEngine;
using System.Collections;

public class TIEDestructionScript : MonoBehaviour {
	public Transform portPylon;
	public Transform stbdPylon;
	public Transform cockpit;
	
	public GameObject explosion;
	
	public GameObject firePrefab;
	
	Transform portWing;
	Transform stbdWing;
	
	GameObject portWingFire;
	GameObject portPylonFire;
	GameObject stbdWingFire;
	GameObject stbdPylonFire;
	
	bool outOfControl = false;
	public float outOfControlTimer = 5.0f;
	bool destroyed = false;
	
	public bool selfDestructEnabled = false;
	public float selfDestructTimer = 50.0f;
	
	public AudioClip explosionSound;
	public AudioClip explosionSoundSmall;
	public AudioClip hullHitSound;
	
	public float soundPitchMod = 1.0f;
	
	// Use this for initialization
	void Start () {
		portWing = portPylon.FindChild("PortWing");
		stbdWing = stbdPylon.FindChild("StbdWing");
	}
	
	public void DestroyAll(){
		//if( portWing != null )
		//	Instantiate(explosion, portWing.position, Quaternion.identity);
		//if( stbdWing != null )
		//	Instantiate(explosion, stbdWing.position, Quaternion.identity);
			
		// Separate the fire particle systems from base object so flames continue after gameobject is destroyed
		if( portWingFire != null ){
			portWingFire.transform.parent = null;
			portWingFire.GetComponent<ParticleSystem>().Stop();
			
			if( explosionSoundSmall )
			{
				float jitter = (soundPitchMod * (Random.Range(-10,10) / 250.0f));
						
				GameObject audioObject = new GameObject("One Shot Audio");
				audioObject.transform.position = portWingFire.transform.position;
				SoundInstanceScript soundScript = audioObject.AddComponent<SoundInstanceScript>();
				soundScript.clip = explosionSoundSmall;
				soundScript.pitch = 1.0f + jitter;
			}
			Destroy( portWingFire, 10 );
		}
		if( stbdWingFire != null ){
			if( explosionSoundSmall )
			{
				float jitter = (soundPitchMod * (Random.Range(-10,10) / 250.0f));
						
				GameObject audioObject = new GameObject("One Shot Audio");
				audioObject.transform.position = stbdWingFire.transform.position;
				SoundInstanceScript soundScript = audioObject.AddComponent<SoundInstanceScript>();
				soundScript.clip = explosionSoundSmall;
				soundScript.pitch = 1.0f + jitter;
			}
			
			stbdWingFire.transform.parent = null;
			stbdWingFire.GetComponent<ParticleSystem>().Stop();
			Destroy( stbdWingFire, 10 );
		}
		if( portPylonFire != null ){
			portPylonFire.transform.parent = null;
			portPylonFire.GetComponent<ParticleSystem>().Stop();
			Destroy( portPylonFire, 10 );
		}
		if( stbdPylonFire != null ){
			stbdPylonFire.transform.parent = null;
			stbdPylonFire.GetComponent<ParticleSystem>().Stop();
			Destroy( stbdPylonFire, 10 );
		}
		
		if( portWing != null )
			Destroy(portWing.gameObject);
		if( stbdWing != null )
			Destroy(stbdWing.gameObject);
		Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
		// Destroy if both wings gone
		if( (portWing != null && stbdWing != null) && portWing.parent == null && stbdWing.parent == null ){
			DestroyAll();
		}
		
		if( outOfControl ){
			outOfControlTimer -= Time.deltaTime;
			if( outOfControlTimer <= 0 )
				DestroyAll();
		}
		
		if( selfDestructEnabled )
			selfDestructTimer -= Time.deltaTime;
		if( selfDestructTimer <= 0 )
			DestroyAll();
	}
	
	void OnProjectileHit( Transform hitLocation ){
		
		if( hullHitSound )
		{
			float jitter = (soundPitchMod * (Random.Range(-10,10) / 250.0f));
						
			GameObject audioObject = new GameObject("One Shot Audio");
			audioObject.transform.position = transform.position;
			SoundInstanceScript soundScript = audioObject.AddComponent<SoundInstanceScript>();
			soundScript.clip = hullHitSound;
			soundScript.pitch = 1.0f + jitter;
		}
		
		// Pylon hits ( sets wing and pylon on fire while separating the two
		if( portWing != null && portWing.parent != null && (hitLocation == portPylon || hitLocation.parent == portPylon) ){
			Rigidbody rb = portWing.gameObject.AddComponent<Rigidbody>();
			rb.useGravity = false;
			
			portWingFire = Instantiate(firePrefab, portWing.position, Quaternion.identity) as GameObject;
			portPylonFire = Instantiate(firePrefab, hitLocation.position, Quaternion.identity) as GameObject;
			portWingFire.transform.parent = portWing;
			portWingFire.GetComponent<ParticleSystem>().Play();
			
			portPylonFire.transform.parent = portPylon;
			portPylonFire.GetComponent<ParticleSystem>().Play();
			
			rb.AddExplosionForce( 5, hitLocation.position, 5 );
			rb.AddRelativeTorque( new Vector3( Random.Range(75,100), Random.Range(80,100), Random.Range(90,100) ) );
			
			Instantiate(explosion, hitLocation.position, Quaternion.identity);
			portWing.parent = null;
			
			portWing.gameObject.AddComponent<ExplodingDebrisScript>().explosion = explosion;
			portWing.gameObject.GetComponent<ExplodingDebrisScript>().explosionSound = explosionSoundSmall;
			
			SetOutOfControl();			
		}
		else if( stbdWing != null && stbdWing.parent != null && (hitLocation == stbdPylon || hitLocation.parent == stbdPylon) ){
			Rigidbody rb = stbdWing.gameObject.AddComponent<Rigidbody>();
			rb.useGravity = false;
			
			stbdWingFire = Instantiate(firePrefab, stbdWing.position, Quaternion.identity) as GameObject;
			stbdPylonFire = Instantiate(firePrefab, hitLocation.position, Quaternion.identity) as GameObject;
			stbdWingFire.transform.parent = stbdWing;
			stbdWingFire.GetComponent<ParticleSystem>().Play();
			
			stbdPylonFire.transform.parent = stbdPylon;
			stbdPylonFire.GetComponent<ParticleSystem>().Play();
			
			rb.AddExplosionForce( 5, hitLocation.position, 5 );
			rb.AddRelativeTorque( new Vector3( Random.Range(75,100), Random.Range(80,100), Random.Range(90,100) ) );
			
			Instantiate(explosion, hitLocation.position, Quaternion.identity);
			stbdWing.parent = null;
			
			stbdWing.gameObject.AddComponent<ExplodingDebrisScript>().explosion = explosion;
			stbdWing.gameObject.GetComponent<ExplodingDebrisScript>().explosionSound = explosionSoundSmall;
			
			SetOutOfControl();
		}
		
		// Cockpit hit (destroys entire object - except separated parts)
		else {
			if( explosionSound )
			{
				float jitter = (soundPitchMod * (Random.Range(-10,10) / 250.0f));
							
				GameObject audioObject = new GameObject("One Shot Audio");
				audioObject.transform.position = transform.position;
				SoundInstanceScript soundScript = audioObject.AddComponent<SoundInstanceScript>();
				soundScript.clip = explosionSound;
				soundScript.pitch = 1.0f + jitter;
			}
		
			
			// Separate the fire particle systems from base object so flames continue after gameobject is destroyed
			if( portPylonFire != null ){
				portPylonFire.transform.parent = null;
				portPylonFire.GetComponent<ParticleSystem>().Stop();
				Destroy( portPylonFire, 10 );
			}
			if( stbdPylonFire != null ){
				stbdPylonFire.transform.parent = null;
				stbdPylonFire.GetComponent<ParticleSystem>().Stop();
				Destroy( stbdPylonFire, 10 );
			}
			
			if( !destroyed ){
				destroyed = true;
				Instantiate(explosion, hitLocation.position, Quaternion.identity);
				Destroy(gameObject);
			}
		}
	}
	
	void SetOutOfControl(){
		if( explosionSound )
		{
			float jitter = (soundPitchMod * (Random.Range(-10,10) / 250.0f));
						
			GameObject audioObject = new GameObject("One Shot Audio");
			audioObject.transform.position = transform.position;
			SoundInstanceScript soundScript = audioObject.AddComponent<SoundInstanceScript>();
			soundScript.clip = explosionSound;
			soundScript.pitch = 1.0f + jitter;
		}
		
		if( !outOfControl ){
			gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.AddRelativeForce( Vector3.forward * 25 );
			rigidbody.AddRelativeTorque( new Vector3( Random.Range(2,10), Random.Range(4,10), Random.Range(3,10) ) );
			outOfControl = true;
		}
	}
}
