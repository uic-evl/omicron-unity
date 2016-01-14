using UnityEngine;
using System.Collections;

public class SimpleSpawner : MonoBehaviour {

    public bool spawnOnTimer = true;

    public float timeDelay = 5;
    float timer;

    public GameObject objectPrefab;

	// Use this for initialization
	void Start () {
        timer = timeDelay;
	}
	
	// Update is called once per frame
	void Update () {
	    if (spawnOnTimer)
        {
            timer -= Time.deltaTime;
            if( timer < 0 )
            {
                Instantiate(objectPrefab, transform.position, transform.rotation);
                timer = timeDelay;
            }
        }
	}
}
