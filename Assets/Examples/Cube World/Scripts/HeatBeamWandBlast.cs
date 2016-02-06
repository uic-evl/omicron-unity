using UnityEngine;
using System.Collections;

public class HeatBeamWandBlast : OmicronWandUpdater
{
	public CAVE2Manager.Button activateButton;
	public LayerMask wandLayerMask = -1;
	
	public bool wandHit;

	public GameObject blastPrefab;

	// Update is called once per frame
	void Update()
	{
		// Shoot a ray from the wand
		Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
		RaycastHit hit;
		
		// Get the first collider that was hit by the ray
		wandHit = Physics.Raycast(ray, out hit, 100, wandLayerMask);
		Debug.DrawLine(ray.origin, hit.point); // Draws a line in the editor
		
		if (wandHit) // The wand is pointed at a collider
		{
			if (CAVE2Manager.GetButtonDown(wandID,activateButton))
			{
				Instantiate(blastPrefab, hit.point, Quaternion.identity);
			}
			
			
		}

		//GetComponent<SphereCollider>().enabled = true; // Enable sphere collider after raycast
	}
}
