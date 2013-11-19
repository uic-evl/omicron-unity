using UnityEngine;
using System.Collections;

public class TileDestructScript : MonoBehaviour {
	public GameObject player;
	public GameObject tileGenerator;
	public float maxDistanceBehindPlayer = 125;
	public float distanceFromPlayer;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		distanceFromPlayer = Vector3.Distance( transform.position, player.transform.position );
			
		if( player.transform.position.z > transform.position.z && distanceFromPlayer > maxDistanceBehindPlayer )
		{
			DynamicTileGeneratorScript script = tileGenerator.GetComponent<DynamicTileGeneratorScript>();
			script.tilePosList.Remove(transform.position);
			Destroy(gameObject);
		}
	}
	
}
