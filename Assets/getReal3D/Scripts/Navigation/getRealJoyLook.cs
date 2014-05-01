using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Navigation/Joystick Look")]

public class getRealJoyLook
	: MonoBehaviour
{
	public enum RotationAxes { JoyX=0, JoyY, JoyXY, JoyZ, JoyXZ, JoyYZ }
	public RotationAxes axes = RotationAxes.JoyXY;
	public string yawAxis = "Yaw";
	public string pitchAxis = "Pitch";
	public string resetButton = "Reset";
	public float RotationSpeed = 30.0f;
	public enum RotationAround { Wand, Head, Reference };
    public RotationAround rotationAround = RotationAround.Head;
    public Transform rotationAroundReference = null;
	public RotationAround rotationFollows = RotationAround.Wand;
	public Transform rotationFollowsReference = null;
	private Transform m_transform;
	private CharacterController controller = null;

	void Awake()
	{
        m_transform = transform;
		if (controller == null)
			controller = GetComponent<CharacterController>();
	}
	
	void Start()
	{
		// Make the rigid body not change rotation
		if (rigidbody) rigidbody.freezeRotation = true;
        if (!getReal3D.Input.NavOptions.HasValue("RotationSpeed")) {
            getReal3D.Input.NavOptions.SetValue<float>("RotationSpeed", RotationSpeed);
        }
	}
	
	// Update is called once per frame
	void NavigationUpdate(float elapsed)
	{
		Vector2 joy = new Vector2(getReal3D.Input.GetAxis(yawAxis), getReal3D.Input.GetAxis(pitchAxis));

		if (Cluster.isMaster || !Cluster.isOn)
		{
			if (joy.sqrMagnitude > 0f)
				UpdateRotation(joy, elapsed);
		}
	}

	void Update()
    {
        if (getReal3D.Input.GetButtonDown(resetButton))
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

		Matrix4x4 frame = Matrix4x4.identity;
		switch(rotationFollows)
		{
		case RotationAround.Head: frame = Matrix4x4.TRS(Vector3.zero, m_transform.rotation * getReal3D.Input.head.rotation, Vector3.one); break;
		case RotationAround.Wand: frame = Matrix4x4.TRS(Vector3.zero, m_transform.rotation * getReal3D.Input.wand.rotation, Vector3.one); break;
		case RotationAround.Reference: frame = (rotationFollowsReference != null)
											 ? Matrix4x4.TRS(Vector3.zero, rotationFollowsReference.rotation, Vector3.one)
											 : Matrix4x4.TRS(Vector3.zero, m_transform.rotation, Vector3.one); break;
		}

		Vector3 up = m_transform.up;
		Vector3 forward = frame.GetColumn(2);
		float angle;
		Vector3 right;
		Quaternion.FromToRotation(up, forward).ToAngleAxis(out angle, out right);

		Vector3 about = m_transform.position;
		switch(rotationAround)
		{
		case RotationAround.Head: about = m_transform.localToWorldMatrix.MultiplyPoint3x4(getReal3D.Input.head.position); break;
		case RotationAround.Wand: about = m_transform.localToWorldMatrix.MultiplyPoint3x4(getReal3D.Input.wand.position); break;
		case RotationAround.Reference: if (rotationAroundReference != null) about = rotationAroundReference.position; break;
		}
		about = m_transform.worldToLocalMatrix * (about - m_transform.position);
		if (controller == null || !controller.enabled)
			m_transform.Translate(about, Space.Self);
		switch(axes) {
		case RotationAxes.JoyX:  m_transform.Rotate(  right, joy.x, Space.World); break;
		case RotationAxes.JoyY:  m_transform.Rotate(     up, joy.x, Space.World); break;
		case RotationAxes.JoyZ:  m_transform.Rotate(forward, joy.x, Space.World); break;
		case RotationAxes.JoyXY: m_transform.Rotate(     up, joy.x, Space.World); m_transform.Rotate(  right, joy.y, Space.World); break;
		case RotationAxes.JoyXZ: m_transform.Rotate(forward, joy.x, Space.World); m_transform.Rotate(  right, joy.y, Space.World); break;
		case RotationAxes.JoyYZ: m_transform.Rotate(     up, joy.x, Space.World); m_transform.Rotate(forward, joy.y, Space.World); break;
		}
		if (controller == null || !controller.enabled)
			m_transform.Translate(-about, Space.Self);
    }
}
