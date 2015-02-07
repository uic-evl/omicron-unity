using UnityEngine;
using System.Collections;

public class CubeDrop : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnWandButtonClick()
	{
		Drop();
	}

	void Drop()
	{
		rigidbody.useGravity = true;
	}
}
