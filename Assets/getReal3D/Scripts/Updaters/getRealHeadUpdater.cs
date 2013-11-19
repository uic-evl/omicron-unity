using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Updater/Head Updater")]

public class getRealHeadUpdater
	: MonoBehaviour
{
	private Transform m_transform;
	public string headSensor = "Head";
	
	void Awake()
	{
		m_transform = transform;
	}

	void Update()
	{
		//Vector3 locPos = new Vector3( getReal3D.Input.GetSensor(headSensor).position.x, getReal3D.Input.GetSensor(headSensor).position.y, -getReal3D.Input.GetSensor(headSensor).position.z );
		m_transform.localPosition = getReal3D.Input.GetSensor(headSensor).position;
		m_transform.localRotation = getReal3D.Input.GetSensor(headSensor).rotation;
	}

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(0.1f, 0.2f, 0.1f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(0,0,0), 1);
    }
}
