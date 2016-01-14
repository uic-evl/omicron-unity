using UnityEngine;
using System.Collections;

public class WandTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnWandButtonHold( CAVE2Manager.Button button )
    {
        SendMessageUpwards("OnWandButtonHoldTrigger", button);
    }
}
