using UnityEngine;
using System.Collections;

public class DynamicTileGeneratorScript : MonoBehaviour {
	
	public Transform centerObject;
	public float worldHeight = 0;
	public Vector3 closestTilePos;
	
	public int gridLength = 1;
	public int gridWidth = 25;
	
	public float tileLength = 9.6f; // 3x3 tile set
	public float tileScale = 5.0f;
	public float tileInterval = 1.0f;
	
	public float activeTiles = 0;
	public float maxTileAheadDistance = 1200;
	public float maxTileBehindDistance = 120;
	public float initialLoadTileSpeed = 500;
	float tileDistance = 0;
	
	public GameObject tile0;
	public GameObject tileA;
	public GameObject tileB;
	public GameObject tileC;
	public GameObject tileD;
	public GameObject tileE;
	public GameObject tileF;
	
	public ArrayList tilePosList;
	
	public bool trench = false;
	public bool surfaceHardReset = false;
	
	public bool showGUI = true;
	
	public GameObject surface;
	
	// Use this for initialization
	void Start () {
		tilePosList = new ArrayList();
		
		surface = new GameObject("Death Star Surface Tiles");
	}
	
	GameObject CreateRandomTile()
	{
		int tileType = Random.Range(10,16);
		GameObject tile = tile0;
				
		switch(tileType)
		{
			case(0): tile = tileA; break;
			case(1): tile = tileB; break;
			case(2): tile = tileC; break;
			case(3): tile = tileD; break;
			case(4): tile = tileE; break;
			case(5): tile = tileF; break;
		}

		return tile;
	}
	
	void ResetSurface()
	{
		tilePosList.Clear();
		Destroy(surface);
		surface = new GameObject("Death Star Surface Tiles");
		surfaceHardReset = false;
	}
	
	GameObject SetTileRotation(GameObject g)
	{
		int rotationAngle = Random.Range(0,4);
		switch(rotationAngle)
		{
			case(0):
				g.transform.Rotate(0,0,0);
				break;
			case(1):
				g.transform.Rotate(0,90,0);
				break;
			case(2):
				g.transform.Rotate(0,180,0);
				break;
			case(3):
				g.transform.Rotate(0,270,0);
				break;
		}
		return g;
	}
	
	void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		if (getReal3D.Cluster.isMaster)
		{
	      //foreach( Light l in lights )
	      //{
	      //  bool enabled = l.enabled;
	      //  stream.Serialize(ref enabled);
	      //}
			stream.Serialize(ref surfaceHardReset);
			stream.Serialize(ref gridLength);
			stream.Serialize(ref gridWidth);
			stream.Serialize(ref maxTileAheadDistance);
			stream.Serialize(ref maxTileBehindDistance);
			stream.Serialize(ref trench);
	   } else {
	      //foreach( Light l in lights )
	      //{
	      //  bool enabled;
	      //  stream.Serialize(ref enabled);
	      //  l.enabled = enabled;
	      //}
			stream.Serialize(ref surfaceHardReset);
			stream.Serialize(ref gridLength);
			stream.Serialize(ref gridWidth);
			stream.Serialize(ref maxTileAheadDistance);
			stream.Serialize(ref maxTileBehindDistance);
			stream.Serialize(ref trench);
	   }
	}

	
	// Update is called once per frame
	void Update () {
		if( surfaceHardReset )
		{
			ResetSurface();
			surfaceHardReset = false;
			tileDistance = 0;
		}
		
		tileInterval = tileLength * tileScale;
		activeTiles = tilePosList.Count;
		
		if( tileDistance < maxTileAheadDistance )
			tileDistance += Time.deltaTime * initialLoadTileSpeed;
		else
			tileDistance = maxTileAheadDistance;
				
		float zPos = tileInterval * (int)(( (centerObject.position.z + tileDistance) / tileInterval) * gridLength);

		for( int i = -gridWidth/2; i < gridWidth/2; i++ ){
			
			float xPos = tileInterval * (int)((centerObject.position.x / tileInterval) );
			if( trench )
				xPos = tileInterval * (int)((0 / tileInterval) );
			
			bool trenchTile = false;
			if( trench && ( i < 1 && i > -1 ) )
			{
				trenchTile = true;
				closestTilePos = new Vector3( xPos + i * tileInterval, worldHeight - tileInterval * 1, zPos );
			}
			else
			{
				closestTilePos = new Vector3( xPos + i * tileInterval, worldHeight, zPos );
			}
						
			if( !tilePosList.Contains( closestTilePos ) )
			{
				tilePosList.Add(closestTilePos);
						
				GameObject g = Instantiate( CreateRandomTile(), closestTilePos, Quaternion.identity) as GameObject;
				g = SetTileRotation(g);
				g.transform.localScale = new Vector3(tileScale,tileScale,tileScale);
				TileDestructScript script = g.GetComponent<TileDestructScript>();
				script.player = centerObject.gameObject;
				script.tileGenerator = gameObject;
				
				g.transform.parent = surface.transform;
				
				if( trenchTile )
				{
					// Left
					closestTilePos = new Vector3( xPos + i * tileInterval - tileInterval / 2, worldHeight - tileInterval * 0.5f, zPos );
					
					GameObject leftWall = Instantiate( CreateRandomTile(), closestTilePos, Quaternion.identity) as GameObject;
					leftWall.transform.Rotate(0,0,-90);
					leftWall = SetTileRotation(leftWall);
					leftWall.transform.localScale = new Vector3(tileScale,tileScale,tileScale);
					leftWall.transform.parent = surface.transform;
					
					TileDestructScript script_left = leftWall.GetComponent<TileDestructScript>();
					script_left.player = centerObject.gameObject;
					script_left.maxDistanceBehindPlayer = maxTileBehindDistance;
					script_left.tileGenerator = gameObject;
	
					// Right
					closestTilePos = new Vector3( xPos + i * tileInterval + tileInterval / 2, worldHeight - tileInterval * 0.5f, zPos );
					
					GameObject rightWall = Instantiate( CreateRandomTile(), closestTilePos, Quaternion.identity) as GameObject;
					rightWall.transform.Rotate(0,0,90);
					rightWall = SetTileRotation(rightWall);
					rightWall.transform.localScale = new Vector3(tileScale,tileScale,tileScale);
					rightWall.transform.parent = surface.transform;
					
					TileDestructScript script_right = rightWall.GetComponent<TileDestructScript>();
					script_right.player = centerObject.gameObject;
					script_right.maxDistanceBehindPlayer = maxTileBehindDistance;
					script_right.tileGenerator = gameObject;					
				}
				
			}
			
		}

	}
	
	void OnGUI() {
		if( showGUI && getReal3D.Cluster.isMaster )
		{	
			GUI.Box(new Rect(0, 0, 250 , 250 ), "Dynamic Tile Generator");
			
			GUI.Label(new Rect(25, 20, 200, 20), "Active Tiles: " + activeTiles);
			
			GUI.Label(new Rect(25, 40, 200, 20), "Grid Length: " + gridLength);
	        gridLength = (int)GUI.HorizontalSlider(new Rect(25, 60, 100, 30), gridLength, 0.0F, 32.0F);
			
			GUI.Label(new Rect(25, 80, 200, 20), "Grid Width: " + gridWidth);
	        gridWidth = (int)GUI.HorizontalSlider(new Rect(25, 100, 100, 30), gridWidth, 0.0F, 32.0F);
			
			GUI.Label(new Rect(25, 120, 200, 20), "Max Tiles Ahead Distance: " + maxTileAheadDistance);
	        maxTileAheadDistance = (int)GUI.HorizontalSlider(new Rect(25, 140, 100, 30), maxTileAheadDistance, 0.0F, 2400.0F);
			
			GUI.Label(new Rect(25, 160, 200, 20), "Max Tiles Behind Distance: " + maxTileBehindDistance);
	        maxTileBehindDistance = (int)GUI.HorizontalSlider(new Rect(25, 180, 100, 30), maxTileBehindDistance, 0.0F, 1200.0F);
			
			trench = GUI.Toggle(new Rect(25, 200, 100, 30), trench, "Trench");
			surfaceHardReset = GUI.Toggle(new Rect(25, 220, 100, 30), surfaceHardReset, "Reset");
		}
    }
}
