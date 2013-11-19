using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class getReal3D_Menu {
	
	[UnityEditor.Callbacks.PostProcessScene(100)]
	static public void FixPlayerSettings()
	{
		Debug.Log ("Adjusting PlayerSettings for build ...");
		PlayerSettings.captureSingleScreen = false;
		PlayerSettings.defaultIsFullScreen = false;
		PlayerSettings.defaultIsNativeResolution = false;
		PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
		PlayerSettings.forceSingleInstance = false;
		PlayerSettings.resizableWindow = true;
		PlayerSettings.runInBackground = true;
		PlayerSettings.usePlayerLog = true;
	}
	
    [MenuItem("getReal3D/Select ClusterViews", false, 12)]
    static public void SelectClusterView()
	{
		getReal3D.ClusterView[] cvArray = Resources.FindObjectsOfTypeAll(typeof(getReal3D.ClusterView)) as getReal3D.ClusterView[];
		List<GameObject> gos = new List<GameObject>();
        foreach (getReal3D.ClusterView cv in cvArray)
        {
            gos.Add(cv.gameObject);
        }
		Selection.objects = gos.ToArray();
	}

	[MenuItem("getReal3D/Remove All ClusterViews", false, 17)]
    static public void RemoveAllClusterView()
    {
        if (EditorUtility.DisplayDialog("Delete ALL ClusterView Components?", "Are you sure you want to delete all ClusterView Components in the scene, including manually added ClusterViews?", "Delete ALL ClusterViews", "Cancel"))
        {
            GameObject[] objects = getReal3D.Editor.Utils.GetAllObjectsInScene(false).ToArray();
            foreach (GameObject obj in objects)
            {
                getReal3D.ClusterView.RemoveClusterViewFromObject(obj, false);
            }
        }
    }

    [MenuItem("getReal3D/Remove Auto ClusterViews", false, 15)]
    static public void RemoveClusterView()
    {
        GameObject[] objects = getReal3D.Editor.Utils.GetAllObjectsInScene(false).ToArray();
        foreach (GameObject obj in objects)
        {
            getReal3D.ClusterView.RemoveClusterViewFromObject(obj, true);
        }
    }
	
	[MenuItem("getReal3D/Remove ClusterViews from Selection", false, 16)]
	static public void RemoveClusterViewSelected()
	{
        GameObject[] objects = getReal3D.Editor.Utils.GetAllObjectsInSelection(true).ToArray();
        foreach (GameObject obj in objects)
        {
			Debug.Log (obj);
            getReal3D.ClusterView.RemoveClusterViewFromObject(obj, true);
        }
	}

	//[MenuItem("getReal3D/Find \"Missing\" scripts", false, 2)]
	static public void RemoveMissingScripts()
	{
        GameObject[] objects = getReal3D.Editor.Utils.GetAllObjectsInScene(false).ToArray();
		foreach(GameObject obj in objects)
		{
			MonoBehaviour[] mbs = obj.GetComponents<MonoBehaviour>();
			foreach(MonoBehaviour mb in mbs)
			{
				if (mb == null)
				{
					Debug.Log ("Found a \"Missing MonoBehaviour\" on " + obj.name);
					break;
				}
			}
		}
	}

    [MenuItem("getReal3D/Undo ClusterViews", true)]
	[MenuItem("getReal3D/Add ClusterViews", true)]
	[MenuItem("getReal3D/Select ClusterViews", true)]
	static public bool CheckAutoClusterViews()
	{
		return EditorPrefs.GetBool("getReal3D.detectAndAddClusterViews", true);
	}

    [MenuItem("getReal3D/Add ClusterViews", false, 10)]
    static public void AddClusterView()
    {
		RemoveClusterView();
        getReal3D.ClusterView.ReloadProperties();
        GameObject[] objects = getReal3D.Editor.Utils.GetAllObjectsInScene(false).ToArray();
		getReal3D.ClusterView.AddClusterViewToObjects(objects, false);
    }
	
	[MenuItem("getReal3D/Add ClusterViews to Selection", true)]
	[MenuItem("getReal3D/Remove ClusterViews from Selection", true)]
    static public bool CheckSelectedClusterViews()
    {
		return CheckAutoClusterViews() && Selection.activeGameObject != null;
    }
	
	[MenuItem("getReal3D/Add ClusterViews to Selection", false, 11)]
	static public void AddClusterViewSelected()
	{
        RemoveClusterViewSelected();
        getReal3D.ClusterView.ReloadProperties();
        GameObject[] objects = getReal3D.Editor.Utils.GetAllObjectsInSelection(true).ToArray();
		getReal3D.ClusterView.AddClusterViewToObjects(objects, false);
	}

    public static void BuildPlayerImpl(string[] levels, bool clusterMode, string output, bool arch64 = false)
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);

        bool oldClusterMode = getReal3D.Editor.Utils.ChangeClusterMode(clusterMode);

        UnityEditor.BuildOptions options = BuildOptions.None;
        if (clusterMode) {
            string[] names = System.Enum.GetNames(typeof(UnityEditor.BuildOptions));
            var values = System.Enum.GetValues(typeof(UnityEditor.BuildOptions));
            string clusterOptionName = "Cluster";
            for (int i = 0; i < names.Length; ++i) {
                if (names[i] == clusterOptionName) {
                    options = (UnityEditor.BuildOptions)values.GetValue(i);
                }
            }
            if (options == BuildOptions.None) {
                throw new System.Exception("Unable to find UnityEditor.BuildOptions." + clusterOptionName);
            }
        }
        BuildPipeline.BuildPlayer(levels, output, arch64 ? BuildTarget.StandaloneWindows64 : BuildTarget.StandaloneWindows, options);
        EditorUserBuildSettings.SwitchActiveBuildTarget(target);

        getReal3D.Editor.Utils.ChangeClusterMode(oldClusterMode);
    }
}
