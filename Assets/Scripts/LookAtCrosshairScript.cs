using UnityEngine;
using System.Collections;

public class LookAtCrosshairScript : MonoBehaviour {
	public Transform crosshair;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(crosshair.position);
	}
}
