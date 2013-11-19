using UnityEngine;
using System.Collections;

public class ThirdPersonCameraScript : MonoBehaviour {

	public float cameraYaw = Mathf.PI/2.0f;
	public float cameraPitch = 0;
	
	public float cameraPitchLimit = Mathf.PI / 2.5f;
		
	public float cameraDistance = -2;
	float userCameraDistance;
	
	public float cameraDistanceMinLimit = -0.7f;
	
	public float verticalOffset = 0.5f; // camera height
	
	public float sensitivityX = -0.1f;
	public float sensitivityY = 0.1f;
	public float zoomSensitivity = 1.0f;
	
	public bool aimMode = false;
	public bool enableRotation = true;
	public bool followObjectRotation = true;
	public float followRotationDelay = 0.5f;
	public float followPitchOffset;
	float followPitchVelocity;
	float followRollVelocity;
	public float focusObjectPitch;
	public float focusObjectYaw;
	
	public GameObject focusObject;
	public float focusInterpolationTime = 1.0f;
	public float focusInterpolationTimer = 0.0f;
	public Transform newFocusObject;
	public Transform lastFocusObject;
	
	
	public bool allowUpRotation = true;
	public bool allowDownRotation = true;
	public bool allowLeftRotation = true;
	public bool allowRightRotation = true;
	
	// Use this for initialization
	void Start () {
		userCameraDistance = cameraDistance;
		
		Camera.main.transform.rotation = Quaternion.identity;
		focusObject = new GameObject("Camera Focus");
		focusObject.transform.parent = transform;
		
		focusObject.transform.position = transform.position;
		lastFocusObject = transform;
		if( newFocusObject == null )
			newFocusObject = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if( newFocusObject != lastFocusObject && focusInterpolationTimer < 1 ){
			focusObject.transform.position = Vector3.Lerp( lastFocusObject.position, newFocusObject.position, focusInterpolationTimer );
			focusInterpolationTimer += Time.deltaTime * focusInterpolationTime;
		} else if( newFocusObject != lastFocusObject && focusInterpolationTimer >= 1 ){
			lastFocusObject = newFocusObject;
			focusInterpolationTimer = 0;
		} else {
			focusObject.transform.position = newFocusObject.position;
		}
		
		if( Input.GetKeyDown( KeyCode.LeftAlt ) )
			enableRotation = !enableRotation;

		// Rotate the camera based on input
		if( enableRotation ){
			float newYaw = Input.GetAxis("Mouse X") * sensitivityX;
			if( (newYaw > 0 && allowLeftRotation) || (newYaw < 0 && allowRightRotation) )
				cameraYaw += newYaw;
			
			float newPitch = Input.GetAxis("Mouse Y") * sensitivityY;
			if( (newPitch > 0 && allowUpRotation) || (newPitch < 0 && allowDownRotation) )
				cameraPitch += newPitch;
		}
		
		

		if( cameraDistance <= cameraDistanceMinLimit )
		{
			cameraDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
			userCameraDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
		}
		else
			cameraDistance = cameraDistanceMinLimit;
		
		// Clamp rotation
		if( cameraPitch > 2 * Mathf.PI )
			cameraPitch -= 2 * Mathf.PI;
		if( cameraPitch < 0 )
			cameraPitch += 2 * Mathf.PI;
		
		if( cameraYaw > 2 * Mathf.PI )
			cameraYaw -= 2 * Mathf.PI;
		if( cameraYaw < 0 )
			cameraYaw += 2 * Mathf.PI;
		
		focusObjectPitch = -(focusObject.transform.eulerAngles.x + followPitchOffset) * Mathf.Deg2Rad;
		focusObjectYaw = (-focusObject.transform.eulerAngles.y * Mathf.Deg2Rad) + Mathf.PI/2.0f;
		
		// Clamp focus rotation
		if( focusObjectPitch > 2 * Mathf.PI )
			focusObjectPitch -= 2 * Mathf.PI;
		if( focusObjectPitch < 0 )
			focusObjectPitch += 2 * Mathf.PI;
		
		if( focusObjectYaw > 2 * Mathf.PI )
			focusObjectYaw -= 2 * Mathf.PI;
		if( focusObjectYaw < 0 )
			focusObjectYaw += 2 * Mathf.PI;
		
		// Smoothly follow focus object
		if( followObjectRotation )
		{
			// Smooth dampen as degrees to smoothly transition 360-0 degree border
			cameraYaw = Mathf.SmoothDampAngle( cameraYaw * Mathf.Rad2Deg, focusObjectYaw * Mathf.Rad2Deg, ref followRollVelocity, followRotationDelay ) * Mathf.Deg2Rad;
			
			cameraPitch = Mathf.SmoothDampAngle( cameraPitch * Mathf.Rad2Deg, focusObjectPitch * Mathf.Rad2Deg, ref followPitchVelocity, followRotationDelay ) * Mathf.Deg2Rad;
		}
		else{
			if( cameraPitch > cameraPitchLimit )
				cameraPitch = cameraPitchLimit;	
			if( cameraPitch < -cameraPitchLimit )
				cameraPitch = -cameraPitchLimit;
		}
		
		float horzRadius = cameraDistance * Mathf.Cos(cameraPitch);
		float y = cameraDistance * Mathf.Sin(cameraPitch);
		float x = horzRadius * Mathf.Cos(cameraYaw);
		float z = horzRadius * Mathf.Sin(cameraYaw);
		Camera.main.transform.position = focusObject.transform.position + new Vector3( x, y + verticalOffset, z );
				
		// Look at the character
		Camera.main.transform.LookAt( focusObject.transform.position + new Vector3( 0, verticalOffset, 0 ) );
	}
	
	void FixedUpdate()
	{
		// Check for camera collision
		if( Camera.main.GetComponent("CameraCollisionScript") )
		{
			CameraCollisionScript collisionScript = Camera.main.GetComponent("CameraCollisionScript") as CameraCollisionScript;
			if( collisionScript.collisionNormal.x > 0 )
				allowRightRotation = false;
			else if( collisionScript.collisionNormal.x < 0 )
				allowLeftRotation = false;
			else
			{
				allowRightRotation = true;
				allowLeftRotation = true;
			}
			
			if( collisionScript.collisionNormal.y > 0 )
				allowUpRotation = false;
			else if( collisionScript.collisionNormal.y < 0 )
				allowDownRotation = false;
			else
			{
				allowUpRotation = true;
				allowDownRotation = true;
			}
			
			if( collisionScript.colliding )
				cameraDistance += Time.deltaTime;
		}
	}
	void LateUpdate(){		
		// Spherical to cartesian coordinate conversion
		float x = cameraDistance * Mathf.Cos(cameraYaw) * Mathf.Cos(cameraPitch);
		float z = cameraDistance * Mathf.Sin(cameraYaw) * Mathf.Cos(cameraPitch);
		float y = cameraDistance * Mathf.Sin(cameraPitch);
		
		Camera.main.transform.position = focusObject.transform.position + new Vector3( x, y + verticalOffset, z );
				
		// Look at the character
		Camera.main.transform.LookAt( focusObject.transform.position + new Vector3( 0, verticalOffset, 0 ) );
	}
	
	public void SetFocusObject( Transform newObject ){
		if( focusInterpolationTimer == 0 ){
			newFocusObject = newObject;
		}
	}
}
