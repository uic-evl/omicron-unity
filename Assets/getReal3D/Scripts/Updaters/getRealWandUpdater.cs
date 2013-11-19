using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Updater/Wand Updater")]

public class getRealWandUpdater
	: MonoBehaviour
{
	private Transform m_transform;
	
	void Awake()
	{
		m_transform = transform;
	}
	
	void Update()
	{
		m_transform.localPosition = getReal3D.Input.wand.position;
		m_transform.localRotation = getReal3D.Input.wand.rotation;
	}

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(0.1f, 0.1f, 0.15f));
    }
}
