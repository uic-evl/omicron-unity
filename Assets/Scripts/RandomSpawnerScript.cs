using UnityEngine;
using System.Collections;

public class RandomSpawnerScript : MonoBehaviour {
	public GameObject spawnedObject;
	
	public float nObjects = 0;
	
	// Use this for initialization
	void Start () {
		for( int i = 0; i < nObjects; i++ ){
			Vector3 objPosition = new Vector3( Random.Range(-500,500), 0, Random.Range(-500,500) );
			GameObject newObj = (GameObject)Instantiate(spawnedObject, objPosition, Quaternion.identity);
			newObj.transform.Rotate( 0, Random.Range(0,360), 0 );
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
