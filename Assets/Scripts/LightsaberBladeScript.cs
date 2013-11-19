using UnityEngine;
using System.Collections;

public class LightsaberBladeScript : MonoBehaviour {
	public bool activated  = false;
	public GameObject sparkEffects;
	
	public GameObject sparkpoint;
	ParticleSystem sparks;
	// Use this for initialization
	void Start () {
		sparkpoint = Instantiate(sparkEffects) as GameObject;
		sparks = sparkpoint.GetComponent("ParticleSystem") as ParticleSystem;
		sparks.Stop();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter( Collision collision )
	{
		if( activated && collision.transform.parent != transform.parent )
		{
			ContactPoint contact = collision.contacts[0];
			sparkpoint.transform.position = contact.point;
			sparks.Play();
		}
	}
	
	void OnCollisionStay( Collision collision )
	{
		if( activated && collision.transform.parent != transform.parent )
		{
			ContactPoint contact = collision.contacts[0];
			sparkpoint.transform.position = contact.point;
		}
	}
	
	void OnCollisionExit( Collision collision )
	{
		sparks.Stop();
	}
}
