using UnityEngine;
using System.Collections;

public class StaticBackgroundObjectScript : MonoBehaviour {
	Vector3 initialOffset;
	
	// Use this for initialization
	void Start () {
		initialOffset = transform.position - Camera.mainCamera.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = initialOffset + Camera.mainCamera.transform.position;
	}
}
