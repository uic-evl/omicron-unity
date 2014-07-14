/**************************************************************************************************
* THE OMICRON PROJECT
*-------------------------------------------------------------------------------------------------
* Copyright 2010-2013             Electronic Visualization Laboratory, University of Illinois at Chicago
* Authors:                                                                                
* Arthur Nishimoto                anishimoto42@gmail.com
*-------------------------------------------------------------------------------------------------
* Copyright (c) 2010-2013, Electronic Visualization Laboratory, University of Illinois at Chicago
* All rights reserved.
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
*
* Redistributions of source code must retain the above copyright notice, this list of conditions
* and the following disclaimer. Redistributions in binary form must reproduce the above copyright
* notice, this list of conditions and the following disclaimer in the documentation and/or other
* materials provided with the distribution.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
* IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
* FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
* CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
* USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
* WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
* ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*************************************************************************************************/

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
