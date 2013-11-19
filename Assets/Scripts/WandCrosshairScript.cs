using UnityEngine;
using System.Collections;

public class WandCrosshairScript : MonoBehaviour {
	// Cursor posision
	public float maxHorizontalPosition = 45;
	public float maxVerticalPosition = 28;
	public float zDistance = 25;
	public bool centerCursor = false;
	
	GameObject targetDirection;
	public bool targetAcquired = false;
	public GameObject target;
	public float maxTargetDistance = 250;
	
	Material crosshairMaterial;
	public bool useMousePosition = false;
	
	// Use this for initialization
	void Start () {
		targetDirection = new GameObject("targetDirection");
		targetDirection.transform.parent = Camera.mainCamera.transform;
		targetDirection.transform.localPosition = Vector3.zero;
		
		crosshairMaterial = GetComponent<MeshRenderer>().material;
	}
	
	void LateUpdate() {
		if( useMousePosition )
			transform.position = Camera.mainCamera.ViewportToWorldPoint(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, zDistance));
	}
	
	// Update is called once per frame
	void Update () {
		// Crosshair movement
		float newXPos = Mathf.Clamp( transform.localPosition.x + Input.GetAxis("Mouse X"), -maxHorizontalPosition, maxHorizontalPosition );
		float newYPos = Mathf.Clamp( transform.localPosition.y + Input.GetAxis("Mouse Y"), -maxVerticalPosition, maxVerticalPosition );
		
		if( useMousePosition ){
			newXPos = Input.mousePosition.x / Screen.width;
			newYPos = Input.mousePosition.y / Screen.height;
			
			newXPos *= maxHorizontalPosition * 2;
			newYPos *= maxVerticalPosition * 2;
			newXPos -= maxHorizontalPosition;
			newYPos -= maxVerticalPosition;
		}
		else
		{
			transform.localPosition = new Vector3( newXPos, newYPos, zDistance );
		}	
		
		// Targeting raycast
		targetDirection.transform.LookAt(transform.position);
		Ray ray = new Ray(targetDirection.transform.parent.position, targetDirection.transform.eulerAngles);
		ray.direction = targetDirection.transform.forward;
        //Debug.DrawRay(ray.origin, ray.direction * maxTargetDistance, Color.white);
		
		RaycastHit hitInfo;
		if( !targetAcquired && Physics.Raycast(ray, out hitInfo, maxTargetDistance ) )
		{
			//Debug.Log("Targeted: " + hitInfo.transform.name );
			targetAcquired = true;
			crosshairMaterial.color = Color.yellow;
			target = hitInfo.transform.gameObject;
		}
		else if( !Physics.Raycast(ray, out hitInfo, maxTargetDistance ) )
		{
			targetAcquired = false;
			crosshairMaterial.color = Color.red;
			target = null;
		}
		
				
		//Vector3 fwd = transform.TransformDirection(Vector3.forward);
	    //if (Physics.Raycast(transform.position, fwd, 10)){
			
		//}
		
		
		// Center cursor
		if( centerCursor ){
			transform.localPosition = new Vector3( 0, 0, zDistance );
			centerCursor = false;
		}
	}
}
