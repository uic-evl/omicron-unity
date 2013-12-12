using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Updater/Head Updater")]

public class getRealHeadUpdater
	: MonoBehaviour
{
	private Transform m_transform;
	
	void Awake()
	{
		m_transform = transform;
	}

	void Update()
	{
		m_transform.localPosition = getReal3D.Input.head.position;
		m_transform.localRotation = getReal3D.Input.head.rotation;
	}

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(0.1f, 0.2f, 0.1f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(0,0,0), 1);
    }
}
