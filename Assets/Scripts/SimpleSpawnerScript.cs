using UnityEngine;
using System.Collections;

public class SimpleSpawnerScript : MonoBehaviour {
	public GameObject prefab;
	
	public float spawnDelay = 2.0f;
	public int spawnRadius = 10;
	float spawnTimer;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		spawnTimer += Time.deltaTime;
		
		if( spawnTimer > spawnDelay ){
			Instantiate( prefab, transform.position + Random.insideUnitSphere * spawnRadius, transform.rotation );
			spawnTimer = 0;
		}
	}
}
