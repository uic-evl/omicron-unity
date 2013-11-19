using UnityEngine;
using System.Collections;

public class ShadowUI
	: MonoBehaviour
{
	public Light[] lights;
	
	void OnGUI()
	{
		if (!getReal3D.GUI.BeginGUI()) return;
		GUILayout.BeginArea(new Rect(0,0,150,getReal3D.GUI.height));
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical( "Lights", GUI.skin.window );
		foreach( Light l in lights )
		{
			l.enabled = GUILayout.Toggle( l.enabled, l.name );
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		getReal3D.GUI.EndGUI();
	}
	
	void LateUpdate()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}
	
	void OnSerializeClusterView(getReal3D.ClusterStream stream){
		foreach( Light l in lights )
		{
			bool enabled = l.enabled;
			stream.Serialize(ref enabled);
			l.enabled = enabled;
		}
	}
}
