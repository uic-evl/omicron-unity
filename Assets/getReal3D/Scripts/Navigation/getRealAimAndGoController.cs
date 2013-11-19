using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Navigation/Aim-And-Go Controller")]

public class getRealAimAndGoController
	: MonoBehaviour
{
	private Transform m_transform = null;
	private CharacterController m_controller = null;
	private Vector3 m_lastGroundedPosition = Vector3.zero;

	public float TranslationSpeed = 1.0f;
	public string forwardAxis = "Forward";
	public string strafeAxis = "Strafe";
	public string navSpeedButton = "NavSpeed";
	public string resetButton = "Reset";

    public enum NavFollows { Wand, Head, Reference };
    public NavFollows navFollows = NavFollows.Wand;
    public Transform navReference = null;

// Use this for initialization
	void Awake()
	{
		m_transform = transform;
		m_controller = GetComponent<CharacterController>();
		m_lastGroundedPosition = m_transform.position;
	}
	
	void Start()
	{
		if (rigidbody) rigidbody.freezeRotation = true;
        if (!getReal3D.Input.NavOptions.HasValue("TranslationSpeed")) {
            getReal3D.Input.NavOptions.SetValue<float>("TranslationSpeed", TranslationSpeed);
        }
	}
	
	void OnEnable()
	{
		if (m_controller)
		{
			m_controller.enabled = true;
		}
	    CharacterMotor motor = GetComponent<CharacterMotor>();
	    if (motor != null)
		{
			motor.SetControllable(false);
			motor.enabled = false;
		}
	}

    void Update()
	{
        doNavigation(Time.smoothDeltaTime);
	}

    private void doNavigation(float elapsed)
    {
        Vector3 joy = new Vector3(getReal3D.Input.GetAxis(strafeAxis), 0, getReal3D.Input.GetAxis(forwardAxis));

        if (Cluster.isMaster || !Cluster.isOn) {
            UpdateNavigation(joy, elapsed);
        }

        if (getReal3D.Input.GetButtonDown(resetButton)) {
            doReset();
        }
    }
	
	private void doReset()
	{
		if (Cluster.isMaster || !Cluster.isOn)
		{
			m_transform.position = m_lastGroundedPosition;
		}
	}

	void UpdateNavigation(Vector3 joy, float elapsed)
	{
		// Get the input vector from keyboard or analog stick
        Vector3 directionVector = joy;
        switch (navFollows)
        {
        case NavFollows.Wand:
            directionVector = getReal3D.Input.wand.rotation * directionVector;
            break;
        case NavFollows.Head:
            directionVector = getReal3D.Input.head.rotation * directionVector;
            break;
        case NavFollows.Reference:
            if (navReference != null)
                directionVector = (navReference.localToWorldMatrix * m_transform.worldToLocalMatrix).MultiplyVector (directionVector);
            break;
        }
        
		if (directionVector != Vector3.zero)
		{
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
		
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
		
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
		
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
	
		getReal3D.Input.NavOptions.GetValue<float>("TranslationSpeed", ref TranslationSpeed);
		directionVector *= TranslationSpeed;
		if (!getReal3D.Input.GetButton(navSpeedButton)) directionVector *= getReal3D.Scale.worldScale;

		// Move the controller
		if (m_controller != null && m_controller.enabled)
		{
			CollisionFlags flags = m_controller.Move(m_transform.TransformDirection(directionVector) * elapsed);
			bool grounded = (flags & CollisionFlags.CollidedBelow) != 0;
			if (grounded) m_lastGroundedPosition = m_transform.position;
		}
		else
		{
            m_transform.position += m_transform.TransformDirection(directionVector) * elapsed;
		}
	}
}