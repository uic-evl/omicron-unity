using UnityEngine;
using System.Collections;
using omicron;
using omicronConnector;

public class OmicronEventClient : MonoBehaviour {
	OmicronManager omicronManager;
	
	// Use this for initialization
	public void Start () {
		omicronManager = GameObject.FindGameObjectWithTag("OmicronManager").GetComponent<OmicronManager>();
		omicronManager.AddClient(this);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnEvent( EventData e )
	{
		//Debug.Log("OmicronEventClient: '"+name+"' received " + e.serviceType);
	}
}
