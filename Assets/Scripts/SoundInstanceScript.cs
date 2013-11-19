using UnityEngine;
using System.Collections;

public class SoundInstanceScript : MonoBehaviour {
	public AudioClip clip;
	
	AudioSource audioSource;
	public float pitch = 1.0f;
	public float minDistance = 50.0f;
	public float maxDistance = 250.0f;
	
	// Use this for initialization
	void Start () {
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = true;
		
	}
	
	// Update is called once per frame
	void Update () {
		if( !audioSource.clip )
		{
			audioSource.clip = clip;
			audioSource.pitch = pitch;
			audioSource.maxDistance = maxDistance;
			audioSource.minDistance = minDistance;
			audioSource.dopplerLevel = 0;
			audioSource.Play();
		}
		if( !audioSource.isPlaying )
			Destroy(gameObject);
	}
}
