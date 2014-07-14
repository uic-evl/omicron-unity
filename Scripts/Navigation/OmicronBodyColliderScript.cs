using UnityEngine;
using System.Collections;

public class OmicronBodyColliderScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter( Collision c )
	{
		Debug.Log(name + " collided with " + c.gameObject.name );
	}
}
