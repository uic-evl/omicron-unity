using UnityEngine;
using System.Collections;

public class NodeInfoGUIText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "Node: "+System.Environment.MachineName;
		guiText.text += "\nMain Camera Count: "+GameObject.FindGameObjectsWithTag("MainCamera").Length;
	}
}
