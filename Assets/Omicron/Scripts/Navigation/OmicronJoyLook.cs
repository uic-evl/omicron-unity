using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Navigation/Joystick Look")]

public class OmicronJoyLook
	: MonoBehaviour
{
	public enum RotationAxes { JoyX=0, JoyY, JoyXY, JoyZ, JoyXZ, JoyYZ }
	public RotationAxes axes = RotationAxes.JoyXY;
	public Space XAxisSpace = Space.Self;
	public Space YAxisSpace = Space.World;
	public Space ZAxisSpace = Space.Self;
	
	public CAVE2Manager.Axis firstAxis = CAVE2Manager.Axis.RightAnalogStickLR;
	public CAVE2Manager.Axis secondAxis = CAVE2Manager.Axis.RightAnalogStickUD;
	
	public CAVE2Manager.Button resetButton = CAVE2Manager.Button.Button2;
	
	public float RotationSpeed = 30.0f;
	public enum RotationAround { Wand, Head, Reference };
    public RotationAround rotationAround = RotationAround.Reference;
    public Transform rotationReference = null;
	private Transform m_transform;
	private CharacterController controller = null;
	
	CAVE2Manager cave2Manager;
	public int headID = 1;
	public int wandID = 1;
	
	// Use this for initialization
	public void Start () {
		cave2Manager = GameObject.FindGameObjectWithTag("OmicronManager").GetComponent<CAVE2Manager>();
		
		// Make the rigid body not change rotation
		if (rigidbody) rigidbody.freezeRotation = true;
        if (!getReal3D.Input.NavOptions.HasValue("RotationSpeed")) {
            getReal3D.Input.NavOptions.SetValue<float>("RotationSpeed", RotationSpeed);
        }
	}
	
	void Awake()
	{
        m_transform = transform;
		if (controller == null)
			controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void NavigationUpdate(float elapsed)
	{
		Vector2 joy = new Vector2(cave2Manager.getWand(wandID).GetAxis(firstAxis), -cave2Manager.getWand(wandID).GetAxis(secondAxis));

		if (Cluster.isMaster || !Cluster.isOn)
		{
			if (joy.sqrMagnitude > 0f)
				UpdateRotation(joy, elapsed);
		}
	}

	void Update()
    {
        if (cave2Manager.getWand(wandID).GetButtonDown(resetButton))
        {
            doReset();
        }
        NavigationUpdate(Time.smoothDeltaTime);
    }

	private void doReset()
	{
		if (Cluster.isMaster || !Cluster.isOn)
		{
			Vector3 up = -Physics.gravity;
			up = (up.sqrMagnitude == 0.0f) ? Vector3.up : up.normalized;
			up.Scale(m_transform.localEulerAngles);
			m_transform.localRotation = Quaternion.Euler(up);
		}
	}

	void UpdateRotation(Vector2 joy, float elapsed)
	{
		getReal3D.Input.NavOptions.GetValue<float>("RotationSpeed", ref RotationSpeed);
		
        joy *= RotationSpeed * elapsed; // default scale ~6 seconds to spin 360 degrees

		Vector3 up = YAxisSpace == Space.World ? m_transform.InverseTransformDirection(Vector3.up) : Vector3.up;
		Vector3 right = XAxisSpace == Space.World ? m_transform.InverseTransformDirection(Vector3.right) : Vector3.right;
		Vector3 forward = ZAxisSpace == Space.World ? m_transform.InverseTransformDirection(Vector3.forward) : Vector3.forward;
		
		Vector3 about = m_transform.position;
		switch(rotationAround)
		{
		case RotationAround.Head: about = m_transform.localToWorldMatrix.MultiplyPoint3x4(getReal3D.Input.head.position); break;
		case RotationAround.Wand: about = m_transform.localToWorldMatrix.MultiplyPoint3x4(getReal3D.Input.wand.position); break;
		case RotationAround.Reference: if (rotationReference != null) about = rotationReference.position; break;
		}
		about = m_transform.worldToLocalMatrix * (about - m_transform.position);
		if (controller == null || !controller.enabled)
			m_transform.Translate(about, Space.Self);
		switch (axes) {
			case RotationAxes.JoyX: m_transform.Rotate(up, joy.x); break;
			case RotationAxes.JoyY: m_transform.Rotate(right, joy.y); break;
			case RotationAxes.JoyXY: m_transform.localRotation *= Quaternion.AngleAxis(joy.x, up) * Quaternion.AngleAxis(joy.y, right); break;
			case RotationAxes.JoyZ: m_transform.Rotate(forward, joy.x); break;
			case RotationAxes.JoyXZ: m_transform.localRotation *= Quaternion.AngleAxis(joy.x, up) * Quaternion.AngleAxis(joy.y, forward); break;
			case RotationAxes.JoyYZ: m_transform.localRotation *= Quaternion.AngleAxis(joy.x, forward) * Quaternion.AngleAxis(joy.y, right); break;
        }
		if (controller == null || !controller.enabled)
			m_transform.Translate(-about, Space.Self);
    }
}
