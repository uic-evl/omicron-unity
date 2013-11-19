using UnityEngine;
using System.Collections;

public class ExplodingDebrisScript : MonoBehaviour {
	public GameObject explosion;
	public float timer = 5;
	
	public AudioClip explosionSound;
	public float soundPitchMod = 1.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		
		if( timer <= 0 ){
			Instantiate(explosion, transform.position, transform.rotation );
			ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem>();
			if( ps != null ){
				ps.Stop();
				GameObject flameTrail = ps.gameObject;
				flameTrail.transform.parent = null;
				Destroy(flameTrail, 10 );
			}
			
			if( explosionSound )
			{
				float jitter = (soundPitchMod * (Random.Range(-10,10) / 250.0f));
						
				GameObject audioObject = new GameObject("One Shot Audio");
				audioObject.transform.position = transform.position;
				SoundInstanceScript soundScript = audioObject.AddComponent<SoundInstanceScript>();
				soundScript.clip = explosionSound;
				soundScript.pitch = 1.0f + jitter;
			}
			
			Destroy(gameObject);
		}
	}
}
