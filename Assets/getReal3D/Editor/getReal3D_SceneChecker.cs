using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace getReal3D
{
	public class SceneChecker
		: EditorWindow
	{
		static bool needsUpdate = true;
		static bool foundCameraUpdater = false;
		static bool foundNavigationScript = false;
		static bool foundClusterViews = false;
		static List<GameObject> gameObjectsWithClusterViews = new List<GameObject>();
		static Vector2 scrollPosition = Vector2.zero;
	
		static bool ArrayEmpty(object[] array)
		{
			return array == null || array.Length == 0;
		}
	
		[MenuItem("getReal3D/Scene Checker", false, 50)]
		static void CreateChecker()
		{
			UpdateSceneStatus();
			EditorWindow.GetWindow(typeof(SceneChecker), false, "Scene Checker");
		}
		
		static void UpdateSceneStatus()
		{
			getRealCameraUpdater[] cu = Resources.FindObjectsOfTypeAll(typeof(getRealCameraUpdater)) as getRealCameraUpdater[];
			getRealAimAndGoController[] ag = Resources.FindObjectsOfTypeAll(typeof(getRealAimAndGoController)) as getRealAimAndGoController[];
			getRealWalkthruController[] wt = Resources.FindObjectsOfTypeAll(typeof(getRealWalkthruController)) as getRealWalkthruController[];
			getRealWandLook[] wl = Resources.FindObjectsOfTypeAll(typeof(getRealWandLook)) as getRealWandLook[];
			getRealJoyLook[] jl = Resources.FindObjectsOfTypeAll(typeof(getRealJoyLook)) as getRealJoyLook[];
			getReal3D.ClusterView[] cv = Resources.FindObjectsOfTypeAll(typeof(getReal3D.ClusterView)) as getReal3D.ClusterView[];
			
			foundCameraUpdater = !ArrayEmpty(cu);
			foundNavigationScript = !(ArrayEmpty(ag) && ArrayEmpty(wt) && ArrayEmpty(wl) && ArrayEmpty(jl));
			foundClusterViews = !ArrayEmpty(cv);
			
			gameObjectsWithClusterViews.Clear();
			foreach(getReal3D.ClusterView c in cv)
			{
				if (!gameObjectsWithClusterViews.Contains(c.gameObject))
					gameObjectsWithClusterViews.Add(c.gameObject);
			}
			
			needsUpdate = false;
		}
		
		void OnGUI()
		{
			if (needsUpdate) UpdateSceneStatus();
			if (!foundCameraUpdater)
				EditorGUILayout.HelpBox("No getRealCameraUpdater script found. You probably want a getRealCameraUpdater attached to a GameObject with a Camera.", MessageType.Warning, true);
			else
				EditorGUILayout.HelpBox("Found getRealCameraUpdater.", MessageType.Info, true);
	
			if (!foundNavigationScript)
				EditorGUILayout.HelpBox("No getReal3D navigation scripts found. You probably want a navigation script (getRealAimAndGoController, getRealWalkthruController, getRealWandLook, getRealJoyLook) attached to a GameObject.", MessageType.None, true);
			else
				EditorGUILayout.HelpBox("Found getReal3D navigation scripts.", MessageType.Info, true);
	
			if (!foundClusterViews)
				EditorGUILayout.HelpBox("No getReal3D.ClusterView scripts found. You probably want to attach getReal3D.ClusterView components to GameObjects you want to sync.", MessageType.Warning, true);
			else
				EditorGUILayout.HelpBox("Found getReal3D.ClusterView scripts.", MessageType.Info, true);
	
			EditorGUILayout.HelpBox("Prefer getReal3D.Input over UnityEngine.Input.", MessageType.Info, true);
			EditorGUILayout.HelpBox("Prefer getReal3D.Cluster.time and getReal3D.Cluster.deltaTime over UnityEngine.Time.time and UnityEngine.Time.deltaTime.", MessageType.Info, true);
			EditorGUILayout.HelpBox("Prefer testing for events (Input, Physics, etc.) only if getReal3D.Cluster.isMaster, then use getReal3D.RPC calls to sync the client (render) nodes.", MessageType.Info, true);
			EditorGUILayout.HelpBox("Prefer getReal3D.Cluster.Instantiate and getReal3D.Cluster.Destroy for dynamic instantiation of prefabs.", MessageType.Info, true);
	
			if (gameObjectsWithClusterViews != null && gameObjectsWithClusterViews.Count > 0)
			{
				GUILayout.Label("GameObjects with getReal3D.ClusterViews");
				EditorGUI.indentLevel++;
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
				EditorGUI.BeginDisabledGroup(true);
				foreach(GameObject go in gameObjectsWithClusterViews)
				{
					EditorGUILayout.ObjectField(go, typeof(GameObject), true);
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndScrollView();
				EditorGUI.indentLevel--;
			}
			
			if (GUILayout.Button("Update"))
			{
				UpdateSceneStatus();
			}
		}
	}
}