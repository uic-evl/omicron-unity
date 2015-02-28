using UnityEngine;
using System.Collections;

public class CAVE2SkyboxController : MonoBehaviour {

    public LayerMask skyboxCullingMask;
    LayerMask lastCullingMask;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if( lastCullingMask != skyboxCullingMask )
        {
            Camera[] cameras = gameObject.GetComponentsInChildren<Camera>();
            foreach( Camera c in cameras )
            {
                c.cullingMask = skyboxCullingMask;
            }
            lastCullingMask = skyboxCullingMask;
        }
	}
}
