using UnityEngine;
using System.Collections;

public class CAVE2ScreenManager : MonoBehaviour {
	public GameObject P1_getRealCameraUpdater;
	
	public string machineName;
	
	public bool lyra01 = true;
	public bool lyra02 = true;
	public bool lyra03 = true;
	public bool lyra04 = true;
	public bool lyra05 = true;
	public bool lyra06 = true;
	public bool lyra07 = true;
	public bool lyra08 = true;
	public bool lyra09 = true;
	
	Camera[] cameras;
	
	// Use this for initialization
	void Start () {
		machineName = System.Environment.MachineName;
	}
	
	// Update is called once per frame
	void Update () {
		cameras = P1_getRealCameraUpdater.GetComponentsInChildren<Camera>();
		
		if( machineName.Contains("LYRA-01") )
		{
			P1_getRealCameraUpdater.SetActive(lyra01);
		}
		else if( machineName.Contains("LYRA-02") )
		{
			P1_getRealCameraUpdater.SetActive(lyra02);	
		}
		else if( machineName.Contains("LYRA-03") )
		{
			P1_getRealCameraUpdater.SetActive(lyra03);
		}
		else if( machineName.Contains("LYRA-04") )
		{
			P1_getRealCameraUpdater.SetActive(lyra04);
		}
		else if( machineName.Contains("LYRA-05") )
		{
			foreach( Camera c in cameras )
				c.enabled = lyra05;	
		}
		else if( machineName.Contains("LYRA-06") )
		{
			foreach( Camera c in cameras )
				c.enabled = lyra06;	
		}
		else if( machineName.Contains("LYRA-07") )
		{
			foreach( Camera c in cameras )
				c.enabled = lyra07;	
		}
		else if( machineName.Contains("LYRA-08") )
		{
			foreach( Camera c in cameras )
				c.enabled = lyra08;	
		}
		else if( machineName.Contains("LYRA-09") )
		{
			foreach( Camera c in cameras )
				c.enabled = lyra09;	
		}
	}
	
	void OnGUI()
	{
		if( getReal3D.Cluster.isMaster )
		{
			lyra01 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 1, 100, 15), lyra01, "Lyra-01");
			lyra02 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 2, 100, 15), lyra02, "Lyra-02");
			lyra03 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 3, 100, 15), lyra03, "Lyra-03");
			lyra04 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 4, 100, 15), lyra04, "Lyra-04");
			lyra05 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 5, 100, 15), lyra05, "Lyra-05");
			lyra06 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 6, 100, 15), lyra06, "Lyra-06");
			lyra07 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 7, 100, 15), lyra07, "Lyra-07");
			lyra08 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 8, 100, 15), lyra08, "Lyra-08");
			lyra09 = UnityEngine.GUI.Toggle(new Rect(10, 15 * 9, 100, 15), lyra09, "Lyra-09");
		}
	}
	void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		if( getReal3D.Cluster.isMaster )
		{
			stream.Serialize(ref lyra01);
			stream.Serialize(ref lyra02);
			stream.Serialize(ref lyra03);
			stream.Serialize(ref lyra04);
			stream.Serialize(ref lyra05);
			stream.Serialize(ref lyra06);
			stream.Serialize(ref lyra07);
			stream.Serialize(ref lyra08);
			stream.Serialize(ref lyra09);
		}
		else
		{
			stream.Serialize(ref lyra01);
			stream.Serialize(ref lyra02);
			stream.Serialize(ref lyra03);
			stream.Serialize(ref lyra04);
			stream.Serialize(ref lyra05);
			stream.Serialize(ref lyra06);
			stream.Serialize(ref lyra07);
			stream.Serialize(ref lyra08);
			stream.Serialize(ref lyra09);
		}
	}
}
