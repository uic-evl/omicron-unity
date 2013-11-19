using UnityEngine;
using System.Collections;

public class TIEFighterMovementScript : MonoBehaviour {
	public float forwardSpeed = 25.0f;
	public bool outOfControl = false;
	
	public Vector3 forwardVector = new Vector3( 1, 0, 0);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( !outOfControl )
			transform.Translate( forwardVector * forwardSpeed * Time.deltaTime );
	}
	
	void SetOutOfControl(){
		outOfControl = true;
	}
}
