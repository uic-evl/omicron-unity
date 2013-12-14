using UnityEngine;
using System.Collections;

public class OmicronPlayerController : OmicronSimplePlayerController {

	// Update is called once per frame
	void Update () {
		
		if( getReal3D.Cluster.isMaster )
		{
			forward = cave2Manager.getWand(wandID).GetAxis(forwardAxis);	
			forward *= movementScale;
			
			strafe = cave2Manager.getWand(wandID).GetAxis(strafeAxis);
			strafe *= movementScale;
		}
		
		CharacterController controller = GetComponentInChildren<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(strafe, 0, forward);
            moveDirection = transform.TransformDirection(moveDirection);          
        }
        controller.Move(moveDirection * getReal3D.Cluster.deltaTime);
	}
}
