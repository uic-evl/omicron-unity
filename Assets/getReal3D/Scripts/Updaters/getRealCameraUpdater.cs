using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using getReal3D;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("getReal3D/Updater/Camera Updater")]

public class getRealCameraUpdater
	: MonoBehaviour
{
	private Transform m_transform;
	private Camera m_camera;

	public int cameraIndex = 0;
	public bool applyHeadTracking = true;

    private float m_mainNearClip;
    private float m_mainFarClip;

    private bool m_pluginInitialized = false;
	
	public GameObject CameraPrefab = null;
	
	void Awake()
	{
		m_transform = transform;
		m_camera = camera;
		m_pluginInitialized = getReal3D.Input.Init();
		Debug.Log ("getReal3D.Input.cameras.Count = " + getReal3D.Input.cameras.Count.ToString());
	}

	void Start()
	{
		m_mainNearClip = m_camera.nearClipPlane;
		m_mainFarClip = m_camera.farClipPlane;

	    if (!m_pluginInitialized)
			Debug.LogError("Failed to initialize GetReal3DPlugin");
		else if (cameraIndex >= getReal3D.Input.cameras.Count)
		{
			cameraIndex = getReal3D.Input.cameras.Count-1;
			Debug.LogWarning("Invalid camera index. Setting to "+ cameraIndex);
		}
		else if (cameraIndex == 0 && getReal3D.Input.cameras.Count > 1)
		{
			CreateCameras();
		}
		
		Rect viewport = new Rect(0f, 0f, 1f, 1f);
		getReal3D.Plugin.getCameraViewport((uint)cameraIndex, ref viewport);
		camera.rect = viewport;
		
		camera.renderingPath = getReal3D.Config.renderingPath;
	}
	
	void CreateCameras()
	{
		List<int> needCameras = new List<int>();
		for (int i = 1; i < getReal3D.Input.cameras.Count; ++i) needCameras.Add(i);

		// find cameras, see which we can remove from needCameras
		foreach(Camera cam in Camera.allCameras)
		{
			if (cam.GetComponent<getRealCameraUpdater>() != null)
			{
				int idx = cam.GetComponent<getRealCameraUpdater>().cameraIndex;
				if (idx > 0) needCameras.Remove(idx);
			}
		}
		
		// make missing cameras
		foreach(int idx in needCameras)
		{
			GameObject newCamObject = null;
			if (CameraPrefab == null)
			{
				newCamObject = Instantiate(gameObject) as GameObject;
			}
			else
			{
				newCamObject = Instantiate(CameraPrefab) as GameObject;
			}
			
			foreach(AudioListener listener in newCamObject.GetComponents<AudioListener>())
			{
				Destroy(listener);
			}
			
			newCamObject.transform.parent = transform.parent;
			newCamObject.tag = gameObject.tag;
			newCamObject.layer = gameObject.layer;
			
			getRealCameraUpdater camUpdater = newCamObject.GetComponent<getRealCameraUpdater>();
			if (camUpdater == null) camUpdater = newCamObject.AddComponent<getRealCameraUpdater>();
			camUpdater.cameraIndex = idx;

			newCamObject.camera.CopyFrom(camera);
		}
	}
	
	void Update()
	{
		if (m_pluginInitialized)
		{
			if (applyHeadTracking)
			{
	        	m_transform.localPosition = getReal3D.Input.GetCameraSensor((uint)cameraIndex).position;
	        	m_transform.localRotation = getReal3D.Input.GetCameraSensor((uint)cameraIndex).rotation;
			}
	        m_camera.projectionMatrix = getReal3D.Input.GetCameraProjection((uint)cameraIndex, m_mainFarClip, m_mainNearClip);
		}
	}
}