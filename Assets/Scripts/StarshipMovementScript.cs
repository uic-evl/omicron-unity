using UnityEngine;
using System.Collections;

public class StarshipMovementScript : MonoBehaviour {
	public float distanceScale = 0.05f;
	public float cameraVelocity;
	
	public Vector3 currentVelocity;
	public float rotationModifier = 12.0f;
	
	public float currentHeading = 0;
	public float currentSpeed;
	
	public bool setCourse = false;
	public float newHeading = 0;
	public float targetSpeed = 0;
	
	public bool thrusters = false;
	public bool thrustersAtStationKeeping = false;
	public float thrusterPower = 0.15f;
	public float thrusterAcceleration = 0.025f;
	public float thrusterDeceleration = 0.05f;
	
	public bool impulse = false;
	public float impulsePower = 0.0f;
	public float impulseAcceleration = 5;
	public float impulseDeceleration = 5;
	
	public bool warp = false;
	public float warpFactor = 1;
	public float warpAcceleration = 750;
	public float warpDeceleration = 500;
	public float warpVelocity;
	
	Vector3 lastPosition;
	
	float speedOfLight = 300000000; // 3.0 x 10^8 m/s
	float impulseSpeed = 320;
	public AudioClip warpPowerupSound;
	public AudioClip warpSpeedSound;
	bool playedWarpPowerUp = false;
	bool playedWarpOut = false;
	
	
	public float yawForce;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {		
		UpdateSpeed();
		transform.Translate(0,0,currentSpeed * distanceScale);
		transform.Rotate(Input.GetAxis("Vertical") * Time.deltaTime * rotationModifier,  0, 0);
		transform.Rotate(0,  Input.GetAxis("Horizontal") * Time.deltaTime * rotationModifier, 0, Space.World);
		
		if( Input.GetAxis("Horizontal") != 0 && yawForce < 30 && yawForce > -30 && currentSpeed > 100 )
			yawForce -= Input.GetAxis("Horizontal") * Time.deltaTime * rotationModifier;
		else if( Input.GetAxis("Horizontal") == 0 && yawForce > 0.1f )
			yawForce -= 1.0f * Time.deltaTime * rotationModifier;
		else if( Input.GetAxis("Horizontal") == 0 && yawForce < -0.1f )
			yawForce += 1.0f * Time.deltaTime * rotationModifier;
			
		
		transform.localEulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.y, yawForce );			
		lastPosition = transform.position;
	}
	
	void UpdateSpeed(){
		cameraVelocity = Camera.mainCamera.velocity.magnitude;
		currentVelocity = (transform.position - lastPosition ) / Time.deltaTime;
	}
	
	// Inertial dampers
	void OnTriggerEnter (Collider other) {
		//other.transform.parent = gameObject.transform;
	}
	
	AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume, float minDistance, float maxDistance) {
        GameObject go = new GameObject("Warp Speed Sound");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
		source.minDistance = minDistance;
		source.maxDistance = maxDistance;
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
}
