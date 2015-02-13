using UnityEngine;
using System.Collections;

public class CAVE2FarCameraManager : MonoBehaviour {

	public bool enableCAVE2FarCamera = true;

	public GameObject MainCamera;
	public GameObject NearMainCamera;
	public GameObject FarCamera;
	public GameObject CAVE2SkyBox;

	// Use this for initialization
	void Start () {
		if( enableCAVE2FarCamera )
		{
			CAVE2SkyBox.SetActive(true);
			MainCamera.SetActive(true);
		}
		else
		{
			CAVE2SkyBox.SetActive(false);
			NearMainCamera.SetActive(true);
			FarCamera.SetActive(true);
			MainCamera.SetActive(false);
		}
	}
}
