using UnityEngine;
using System.Collections;

public class CrosshairScript : MonoBehaviour {

	public bool targetAcquired;
	Material crosshairMaterial;
	
	// Use this for initialization
	void Start () {
		crosshairMaterial = GetComponent<MeshRenderer>().material;
	}
	
	public void TargetLocked()
	{
		targetAcquired = true;
	}
	
	public void TargetLost()
	{
		targetAcquired = false;
	}
	
	// Update is called once per frame
	void Update () {
		if( targetAcquired )
		{
			crosshairMaterial.color = Color.green;
		}
		else
		{
			crosshairMaterial.color = Color.red;
		}
	}
}
