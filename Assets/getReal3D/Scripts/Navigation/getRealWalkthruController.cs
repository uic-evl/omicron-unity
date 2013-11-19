using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Navigation/Walkthru Controller")]
public class getRealWalkthruController
	: MonoBehaviour
{
	private CharacterMotor m_motor;
	private CharacterController m_controller;
	private Transform m_transform;
	private Vector3 m_lastGroundedPosition = Vector3.zero;
	
	public float TranslationSpeed = 1.0f;
	public string forwardAxis = "Forward";
	public string strafeAxis = "Strafe";
	public string navSpeedButton = "NavSpeed";
	public string resetButton = "Reset";
	public string jumpButton = "Jump";

    public enum NavFollows { Wand, Head, Reference };
    public NavFollows navFollows = NavFollows.Wand;
    public Transform navReference = null;


// Use this for initialization
	void Awake()
	{
	    m_transform = transform;
		m_motor = GetComponent<CharacterMotor>();
		m_controller = GetComponent<CharacterController>();
		m_lastGroundedPosition = m_transform.position;
        if (!getReal3D.Input.NavOptions.HasValue("TranslationSpeed"))
		{
            getReal3D.Input.NavOptions.SetValue<float>("TranslationSpeed", TranslationSpeed);
        }
	}
		
	void OnEnable()
	{
		if (m_controller)
		{
			m_controller.enabled = true;
		}
		if (m_motor != null)
		{
			m_motor.SetControllable(true);
			m_motor.enabled = true;
		}
	}

    void Update()
    {
        doNavigation(Time.smoothDeltaTime);
    }

    void doNavigation(float elapsed)
	{
		if (m_motor != null && m_motor.grounded)
			m_lastGroundedPosition = m_transform.position;

		Vector3 joy = new Vector3(getReal3D.Input.GetAxis(strafeAxis), 0, getReal3D.Input.GetAxis(forwardAxis));

		if (Cluster.isMaster || !Cluster.isOn)
		{
            UpdateNavigation(joy, elapsed);
		}

 		if (getReal3D.Input.GetButtonDown(resetButton))
 		{
 			doReset();
 		}
 	}

	private void doReset()
	{
		if (Cluster.isMaster || !Cluster.isOn)
		{
			m_transform.position = m_lastGroundedPosition;
			if (m_motor != null)
			{
				m_motor.inputMoveDirection = Vector3.zero;
				m_motor.inputJump = false;
			}
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
                directionVector = (navReference.localToWorldMatrix * m_transform.worldToLocalMatrix).MultiplyVector(directionVector);
            break;
        }
		directionVector -= Vector3.Dot(directionVector, Physics.gravity.normalized) * Physics.gravity.normalized;

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
        if (!getReal3D.Input.GetButton(navSpeedButton))
            directionVector *= getReal3D.Scale.worldScale;

        // Apply the direction to the CharacterMotor, CharacterController, or Transform, as available
        if (m_motor != null && m_motor.enabled && m_motor.canControl)
        {
            m_motor.inputMoveDirection = m_transform.rotation * directionVector;
            m_motor.inputJump = getReal3D.Input.GetButtonDown(jumpButton);
        }
        else if (m_controller != null && m_controller.enabled)
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