/**************************************************************************************************
* THE OMICRON PROJECT
*-------------------------------------------------------------------------------------------------
* Copyright 2010-2014             Electronic Visualization Laboratory, University of Illinois at Chicago
* Authors:                                                                                
* Arthur Nishimoto                anishimoto42@gmail.com
*-------------------------------------------------------------------------------------------------
* Copyright (c) 2010-2014, Electronic Visualization Laboratory, University of Illinois at Chicago
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
using omicron;
using omicronConnector;

public class OmicronKinectEventClient : OmicronEventClient {

	public GameObject jointPrefab;

	bool jointsInitialized = false;
	GameObject[] joints;

	// Use this for initialization
	new void Start () {
		InitOmicron ();
	}

	void InitializeJoints( int jointCount )
	{
		joints = new GameObject[jointCount];
		for (int i = 0; i < jointCount; i++)
		{
			joints[i] = Instantiate(jointPrefab) as GameObject;
			joints[i].transform.parent = transform;
			joints[i].name = "Joint "+i;
		}
		jointsInitialized = true;
	}

	// Update is called once per frame
	void Update () {
	}
	
	void OnEvent( EventData e )
	{
		if (e.serviceType == EventBase.ServiceType.ServiceTypeMocap)
		{
			// 27 = OpenNI or Kinect v1.x skeleton; 29 = Kinect v2.0
			// See https://github.com/uic-evl/omicron/wiki/MSKinectService
			// for joint ID names
			int jointCount = (int)e.extraDataItems;

			if( !jointsInitialized )
				InitializeJoints(jointCount);

			for( int i = 0; i < jointCount; i++ )
			{
				float[] posArray = new float[] { 0, 0, 0 };
				e.getExtraDataVector3(i, posArray );
				joints[i].transform.localPosition = new Vector3( posArray[0], posArray[1], posArray[2] );
			}

		}
	}
}
