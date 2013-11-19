using UnityEngine;
using System.Collections;

public class TileGeneratorScript : MonoBehaviour {
	public int gridSize = 10;
	
	public float tileLength = 3.2f;
	public float tileScale = 1.0f;
	
	public GameObject tileA;
	public GameObject tileB;
	public GameObject tileC;
	public GameObject tileD;
	public GameObject tileE;
	public GameObject tileF;
	
	// Use this for initialization
	void Start () {
	
		for( int i = -gridSize/2; i < gridSize/2; i++ )
		{
			for( int j = -gridSize/2; j < gridSize/2; j++ )
			{
				float xPos = transform.localPosition.x + tileLength * tileScale * i;
				float yPos = transform.localPosition.y + tileLength * tileScale * j;
				Vector3 tilePos = new Vector3( xPos, transform.position.y + 0.01f, yPos );
				
				int tileType = Random.Range(0,6);
				GameObject g;
				
				switch(tileType)
				{
					case(0):
						g  = Instantiate(tileA, tilePos, Quaternion.identity) as GameObject;
						g.transform.parent = transform;
						g = SetTileRotation(g);
						break;
					case(1):
						g = Instantiate(tileB, tilePos, Quaternion.identity) as GameObject;
						g.transform.parent = transform;
						g = SetTileRotation(g);
						break;
					case(2):
						g = Instantiate(tileC, tilePos, Quaternion.identity) as GameObject;
						g.transform.parent = transform;
						g = SetTileRotation(g);
						break;
					case(3):
						g = Instantiate(tileD, tilePos, Quaternion.identity) as GameObject;
						g.transform.parent = transform;
						g = SetTileRotation(g);
						break;
					case(4):
						g = Instantiate(tileE, tilePos, Quaternion.identity) as GameObject;
						g.transform.parent = transform;
						g = SetTileRotation(g);
						break;
					case(5):
						g = Instantiate(tileF, tilePos, Quaternion.identity) as GameObject;
						g.transform.parent = transform;
						g = SetTileRotation(g);
						break;
				}
				
				
			}
		}
	}
	
	GameObject SetTileRotation(GameObject g)
	{
		int rotationAngle = Random.Range(0,4);
		switch(rotationAngle)
		{
			case(0):
				g.transform.Rotate(270,270,0);
				break;
			case(1):
				g.transform.Rotate(270,270,90);
				break;
			case(2):
				g.transform.Rotate(270,270,180);
				break;
			case(3):
				g.transform.Rotate(270,270,270);
				break;
		}
		return g;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
