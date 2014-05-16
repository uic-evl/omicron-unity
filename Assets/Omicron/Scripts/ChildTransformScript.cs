using UnityEngine;
using System.Collections;

public class ChildTransformScript : MonoBehaviour {

	public Transform parent;
	public bool matchPosition = true;
	public bool matchRotation = true;
	public bool matchScale = true;

	Vector3 positionOffset;
	Vector3 rotationOffset;
	Vector3 scaleOffset;

	// Use this for initialization
	void Start () {
		positionOffset = transform.position;
		rotationOffset = transform.eulerAngles;
		scaleOffset = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if( parent && matchPosition )
			transform.position = parent.position + positionOffset;
		if( parent && matchRotation )
			transform.eulerAngles = parent.eulerAngles + rotationOffset;
		if( parent && matchScale )
			transform.localScale = parent.localScale + scaleOffset;
	}
}
