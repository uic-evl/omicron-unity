using UnityEngine;
using System.Collections;

public class CrosshairMovementScript : MonoBehaviour {
	public bool invertY = true;
	
	public float speed = 1.5f;
	
	public Transform center;
	public float distanceFromCenter;
	public float snapToDistance = 5.0f;
	public float snapSpeedModifier = 0.5f;
	
	float verticalInput;
	float horizontalInput;
	public float snapSpeed;
	
	
	public float maxHorizontalDistance = 40.0f;
	public float maxVerticalDistance = 30.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		verticalInput = Input.GetAxis("Vertical") * speed;
		horizontalInput = Input.GetAxis("Horizontal") * speed;

		if( invertY )
			verticalInput *= -1;
		
		transform.Translate( horizontalInput, 0, -verticalInput );
		
		distanceFromCenter = Vector3.Distance( transform.position, center.position );
		
		// Ideally a slow snap (cursor barely moved) should yield a snap speed of ~0.1
		snapSpeed = snapSpeedModifier * (distanceFromCenter / (snapToDistance * 2.0f) );
		
		
		// If crosshair is pointing alway from center and no user input, gently snap back toward center
		if( (verticalInput+horizontalInput) == 0 && distanceFromCenter > snapToDistance ){
			transform.position = Vector3.MoveTowards( transform.position, center.position, snapSpeed );
			
		}
	}
}
