using UnityEngine;
using System.Collections;
using omicron;
using omicronConnector;

public class OmicronWandUpdater : MonoBehaviour {
	CAVE2Manager cave2Manager;
	public int wandID = 1;
	
	// Use this for initialization
	public void Start () {
		cave2Manager = GameObject.FindGameObjectWithTag("OmicronManager").GetComponent<CAVE2Manager>();
	}
	
	// Update is called once per frame
	void Update () {
		
		//transform.localPosition = cave2Manager.getHead(wandID).position;
		//transform.localRotation = cave2Manager.getHead(wandID).orientation;
	}
	
}
