using UnityEngine;
using System.Collections;

public class LightsaberScript : MonoBehaviour {
	public bool activated = false;
	
	public float bladeLength = 1.0f;
	public float bladeWidth = 0.05f;
	public float bladeOffset = 0.13f;
	public float bladeColliderOffset = 0.03f;
	float curBladeLength;
	
	public Material bladeMaterial;
	public Color bladeColor;
	
	public AudioClip saberOnClip;
	public AudioClip saberOffClip;
	public AudioClip saberIdleClip;
	
	LineRenderer bladeRenderer;
	Light bladeLight;
	public float lightRange = 2;
	public float lightIntensity = 1;
	
	float bladeProgress = 0;
	public float activationSpeed = 1.0f;
	
	// Adds noise to hum sound and blade material
	public float jitterModifier = 1.0f;
	
	AudioSource bladeHumAudio;
	AudioSource bladeActivationAudio;
	
	const int OFF = 0;
	enum SaberState { Off, Activating, On, Deactivating };
	SaberState saberState = SaberState.Off;
	
	CapsuleCollider bladeCollider;
	Rigidbody bladeRigidbody;
	Rigidbody hiltRigidbody;
	
	GameObject blade;
	
	public float hiltRadius = 0.04f;
	public float hiltLength = 0.27f;
	public PhysicMaterial hiltPhysicMaterial;
	
	public bool useGravity = true;
	public bool usePhysics = false;
	
	LightsaberBladeScript bladeScript;
	public GameObject sparkEffects;
	
	// Use this for initialization
	void Start () {
		// Light
		blade = new GameObject(gameObject.name + " blade");
		blade.transform.parent = transform;
		bladeLight = blade.AddComponent("Light") as Light;
		bladeLight.color = bladeColor;
		bladeLight.range = lightRange;
		bladeScript = blade.AddComponent("LightsaberBladeScript") as LightsaberBladeScript;
		bladeScript.sparkEffects = sparkEffects;
		
		// Line Renderer
		bladeRenderer = gameObject.AddComponent("LineRenderer") as LineRenderer;
		bladeRenderer.SetPosition(0, new Vector3( bladeOffset, 0, 0 ) );
		bladeRenderer.SetPosition(1, new Vector3( bladeOffset, 0, 0 ) );
		bladeRenderer.SetWidth( bladeWidth, bladeWidth );
		bladeRenderer.material = bladeMaterial;
		bladeRenderer.useWorldSpace = false;
		
		// Collider
		bladeCollider = blade.AddComponent("CapsuleCollider") as CapsuleCollider;
		bladeCollider.direction = 0;
		bladeCollider.radius = bladeWidth / 2.0f;
		bladeCollider.height = 0;
		bladeCollider.material = hiltPhysicMaterial;
		
		bladeRigidbody = blade.AddComponent("Rigidbody") as Rigidbody;
		bladeRigidbody.constraints = RigidbodyConstraints.FreezeAll;
		
		CapsuleCollider hiltCollider = gameObject.AddComponent("CapsuleCollider") as CapsuleCollider;
		hiltCollider.direction = 0;
		hiltCollider.radius = hiltRadius;
		hiltCollider.height = hiltLength;
		hiltCollider.material = hiltPhysicMaterial;
		hiltCollider.center = Vector3.zero;
		
		hiltRigidbody = gameObject.AddComponent("Rigidbody") as Rigidbody;
		hiltRigidbody.useGravity = useGravity;

		// Sounds
		bladeHumAudio = gameObject.AddComponent("AudioSource") as AudioSource;
		bladeHumAudio.clip = saberIdleClip;
		bladeHumAudio.loop = true;
		bladeHumAudio.playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update () {
		if( getReal3D.Input.GetButtonDown("Activate") )
			activated = !activated;
		
		if( usePhysics )
		{
			rigidbody.constraints = RigidbodyConstraints.None;
		}
		else
		{
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		
		// Activation trigger
		if( activated && saberState != SaberState.On && saberState != SaberState.Activating )
		{
			AudioSource.PlayClipAtPoint(saberOnClip, transform.position);
			saberState = SaberState.Activating;
			bladeHumAudio.Play();
			bladeCollider.isTrigger = false;
		}
		if( !activated && saberState != SaberState.Off && saberState != SaberState.Deactivating )
		{
			AudioSource.PlayClipAtPoint(saberOffClip, transform.position);
			saberState = SaberState.Deactivating;
		}
		
		// Activation states
		if( saberState == SaberState.Activating )
		{
			bladeProgress += Time.deltaTime * activationSpeed;
			if( bladeProgress >= 1.0f )
				saberState = SaberState.On;
		}
		else if( saberState == SaberState.Deactivating )
		{
			bladeProgress -= Time.deltaTime * activationSpeed;
			
			if( bladeProgress <= 0.0f )
			{
				saberState = SaberState.Off;
				bladeHumAudio.Stop();
				bladeCollider.isTrigger = true;
			}
		}
		
		// State independent updates
		bladeScript.activated = activated;
		hiltRigidbody.useGravity = useGravity;
		bladeHumAudio.volume = bladeProgress;
		curBladeLength = Mathf.Lerp( bladeOffset, bladeLength, bladeProgress );
		float curBladeLight = Mathf.Lerp( 0, lightIntensity, bladeProgress );
		
		// Update blade dimensions
		blade.transform.localPosition = new Vector3( bladeOffset, 0, 0 );
		blade.transform.localRotation = Quaternion.identity;

		bladeRenderer.SetPosition(1, new Vector3( curBladeLength, 0, 0 ) );
		bladeRenderer.SetWidth( bladeWidth, bladeWidth );
		bladeCollider.radius = bladeWidth / 2.0f;
		
		// Add some noise to the blade color, sound, and light
		float jitter = jitterModifier * (Random.Range(-10,10) / 250.0f);
		bladeHumAudio.pitch = 1.0f + jitter / 10.0f;
		bladeMaterial.SetColor("_TintColor", new Color( 0.5f, 0.5f, 0.5f, 0.5f + jitter ));
		
		if( saberState != SaberState.Off )
			bladeLight.intensity = curBladeLight + jitter;
		else
			bladeLight.intensity = curBladeLight;
	}
	
	void FixedUpdate()
	{
		float curBladeColliderCenter = Mathf.Lerp( bladeColliderOffset, bladeColliderOffset/2.0f + bladeLength/2.0f- bladeOffset/2.0f, bladeProgress );
		
		bladeCollider.height = curBladeLength - bladeOffset;
		bladeCollider.center = new Vector3( curBladeColliderCenter, 0, 0 );
	}
}
