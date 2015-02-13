using UnityEngine;
using System.Collections;

public class DebugGUIManager : MonoBehaviour {

	Rect mainWindow = new Rect(20, 20, 400, 350);

	public enum DebugWindow { Main, CAVE2, PlayerController };
	string[] windowStrings = new string[] {"OmicronManager", "CAVE2 Simulator", "PlayerController"};

	public DebugWindow currentWindow = DebugWindow.Main;

	OmicronManager omgManager;
	CAVE2Manager cave2manager;
	OmicronPlayerController playerController;

	Rect omicronWindow;
	Rect playerWindow;

	public bool showGUI = false;

	void Start()
	{
		omgManager = GameObject.FindGameObjectWithTag ("OmicronManager").GetComponent<OmicronManager> ();
		cave2manager = GameObject.FindGameObjectWithTag ("OmicronManager").GetComponent<CAVE2Manager> ();
		playerController = GameObject.FindGameObjectWithTag ("PlayerController").GetComponent<OmicronPlayerController> ();
	}

	void Update()
	{
		if ( (Input.GetKey(KeyCode.LeftAlt)||Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F11))
			showGUI = !showGUI;
	}

	void OnGUI() {
		if( showGUI )
		{
			mainWindow = GUI.Window(0, mainWindow, OnMainWindow, "Omicron Debug Manager");
		}
	}
	void OnMainWindow(int windowID) {
		GUI.DragWindow (new Rect (0, 0, 10000, 20));

		currentWindow = (DebugWindow)GUI.SelectionGrid(new Rect(10, 20, 350, 24), (int)currentWindow, windowStrings, 3);

		if (currentWindow == DebugWindow.Main )
		{
			if( omgManager != null )
			{
				omgManager.SetGUIOffSet(new Vector2(0,50));
                omgManager.OnWindow(windowID);
	        }
	        else
				GUI.Label(new Rect(0,50,256,24), "This Feature is Not Currently Available");
		}
		else if (currentWindow == DebugWindow.CAVE2 )
		{
			if( cave2manager != null )
			{
				cave2manager.SetGUIOffSet(new Vector2(0,50));
				cave2manager.OnWindow(windowID);
			}
			else
				GUI.Label(new Rect(20,50,256,24), "This Feature is Not Currently Available");
		}
		else if (currentWindow == DebugWindow.PlayerController )
		{
			if( playerController != null )
			{
				playerController.SetGUIOffSet(new Vector2(0,25));
				playerController.OnWindow(windowID);
			}
			else
				GUI.Label(new Rect(20,50,256,24), "This Feature is Not Currently Available");
        }
    }
}
