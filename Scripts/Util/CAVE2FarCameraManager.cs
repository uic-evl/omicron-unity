using UnityEngine;
using System.Collections;

public class CAVE2FarCameraManager : MonoBehaviour {

	public bool enableCAVE2FarCamera = true;

	public GameObject MainCamera;
	public GameObject NearMainCamera;
	public GameObject FarCamera;
	public GameObject CAVE2SkyBox;
    public GameObject skyBox;

    LayerMask skyboxDefaultLayer;

    bool enable = false;

    void Start()
    {
        enableCAVE2FarCamera = !enable;
        if (!Application.HasProLicense())
        {
            enableCAVE2FarCamera = false;
            enable = true;
        }
        skyboxDefaultLayer = skyBox.layer;
    }

	// Use this for initialization
	void Update () {
        if( enableCAVE2FarCamera && !enable)
		{
			CAVE2SkyBox.SetActive(true);
			MainCamera.SetActive(true);
            skyBox.SetActive(true);
            //SetLayerInChildren(skyBox, skyboxDefaultLayer);
            enable = true;
		}
        else if( !enableCAVE2FarCamera && enable)
		{
			CAVE2SkyBox.SetActive(false);
			NearMainCamera.SetActive(true);
			FarCamera.SetActive(true);
			MainCamera.SetActive(false);
            //skyBox.transform.localScale = new Vector3(10000000000000000,10000000000000000,10000000000000000);
            //SetLayerInChildren(skyBox, 0);
            enable = false;
		}
	}

    void SetLayerInChildren( GameObject parent, LayerMask layer )
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();

        foreach (Transform t in children)
        {
            t.gameObject.layer = layer;
        }
    }
}
