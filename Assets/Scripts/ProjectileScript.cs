using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	public float speed = 2.0f;
	public float parentSpeed = 0.0f;
	
	public float lifetime = 3.0f;
	
	public GameObject hullHitEffect;
	
	// Use this for initialization
	void Start () {
		Destroy(gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate( 0, 0, speed + parentSpeed );			
	}
	
	void OnCollisionEnter(Collision collision) {
		if( !collision.transform.gameObject.CompareTag("Projectile") && !collision.transform.gameObject.CompareTag("Player") ){
			
			//foreach (ContactPoint contact in collision.contacts) {
            //	Debug.DrawRay(contact.point, contact.normal, Color.white);
        	//}
			if( hullHitEffect )
				Instantiate( hullHitEffect, transform.position, transform.rotation );
			
			Debug.Log(this.name + " collided with " + collision.transform.name);
			collision.gameObject.SendMessageUpwards("OnProjectileHit", collision.transform, SendMessageOptions.DontRequireReceiver );
			Destroy(gameObject);
		}
    }
	
	public void SetParentSpeed( float value )
	{
		parentSpeed = value;
	}
}
