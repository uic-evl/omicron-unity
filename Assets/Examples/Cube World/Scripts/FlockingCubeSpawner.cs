using UnityEngine;
using System.Collections;

public class FlockingCubeSpawner : MonoBehaviour {

	public GameObject flockingCubePrefab;

	public bool spawnCube;

	public CAVE2Manager.Button cubeButton = CAVE2Manager.Button.Button6;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(spawnCube)
		{
			SpawnCube();
			spawnCube = false;
		}

		if (CAVE2Manager.GetButtonUp(1,cubeButton))
		{
			SpawnCube();
		}
	}

	void SpawnCube()
	{
		GameObject cube = Instantiate( flockingCubePrefab, transform.position, transform.rotation ) as GameObject;
		int randomColor = Random.Range(0,7);
		Color trailColor = Color.white;

		switch(randomColor)
		{
			case(0): trailColor = Color.blue; break;
			case(1): trailColor = Color.cyan; break;
			case(2): trailColor = Color.grey; break;
			case(3): trailColor = Color.green; break;
			case(4): trailColor = Color.magenta; break;
			case(5): trailColor = Color.red; break;
			case(6): trailColor = Color.yellow; break;
			case(7): trailColor = Color.white; break;
		}
		cube.GetComponent<TrailRenderer>().material.SetColor("_TintColor",trailColor);
	}
}
