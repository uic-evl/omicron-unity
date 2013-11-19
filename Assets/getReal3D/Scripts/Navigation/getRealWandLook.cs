using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Navigation/Wand Look")]

public class getRealWandLook
	: MonoBehaviour
{
	public enum RotationAxes { WandX = 0, WandY, WandXY, WandZ, WandXZ, WandYZ, WandXYZ}
	public RotationAxes axes = RotationAxes.WandXY;
	public string activationButton = "WandLook";
	public string resetButton = "Reset";
	
	private Quaternion m_initialWand = new Quaternion();
	private Quaternion m_initialLocal = new Quaternion();
	private bool m_active = false;

	private Transform m_transform;
	public enum RotationAround { Wand, Head, Reference };
    public RotationAround rotationAround = RotationAround.Reference;
    public Transform rotationReference = null;
	private CharacterController controller = null;

	void Awake()
	{
        m_transform = transform;
		if (controller == null)
			controller = GetComponent<CharacterController>();
	}

	// Use this for initialization
	void Start()
    {
        // Make the rigid body not change rotation
        if (rigidbody) rigidbody.freezeRotation = true;
    }
	
	// Update is called once per frame
	void NavigationUpdate()
	{
		if (getReal3D.Input.GetButton(activationButton))
		{
			if (!m_active)
			{
				m_active = true;
				m_initialWand = getReal3D.Input.wand.rotation;
				m_initialLocal = m_transform.localRotation;
			}
			else if (Cluster.isMaster || !Cluster.isOn)
			{
				UpdateRotation(m_initialWand, getReal3D.Input.wand.rotation);
			}
		}
		else if (m_active)
			m_active = false;
	}

	void Update()
	{
		if (getReal3D.Input.GetButtonDown(resetButton))
		{
			doReset();
		}
		NavigationUpdate();
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

	void UpdateRotation (Quaternion initialWand, Quaternion currentWand)
    {
		Quaternion diffOrn = Quaternion.Inverse(initialWand) * currentWand;
		switch(axes)
		{
		case RotationAxes.WandX:   diffOrn = Quaternion.Euler(new Vector3(diffOrn.eulerAngles.x, 0, 0)); break;
		case RotationAxes.WandY:   diffOrn = Quaternion.Euler(new Vector3(0, diffOrn.eulerAngles.y, 0)); break;
		case RotationAxes.WandXY:  diffOrn = Quaternion.Euler(new Vector3(diffOrn.eulerAngles.x, diffOrn.eulerAngles.y, 0)); break;
		case RotationAxes.WandZ:   diffOrn = Quaternion.Euler(new Vector3(0, 0, diffOrn.eulerAngles.z)); break;
		case RotationAxes.WandXZ:  diffOrn = Quaternion.Euler(new Vector3(diffOrn.eulerAngles.x, 0, diffOrn.eulerAngles.z)); break;
		case RotationAxes.WandYZ:  diffOrn = Quaternion.Euler(new Vector3(0, diffOrn.eulerAngles.y, diffOrn.eulerAngles.z)); break;
		case RotationAxes.WandXYZ: break;
		}
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
		m_transform.localRotation = m_initialLocal * diffOrn;
		if (controller == null || !controller.enabled)
			m_transform.Translate(-about, Space.Self);
	}
}