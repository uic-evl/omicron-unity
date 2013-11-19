using UnityEngine;
using System.Collections;

public class LookAtTargetScript : MonoBehaviour {
	public Transform target;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.LookAt(target.position);
	}
}
