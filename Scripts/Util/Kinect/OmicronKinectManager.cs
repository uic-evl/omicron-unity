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

public class OmicronKinectManager : OmicronEventClient {

	public GameObject kinect2bodyPrefab;

	Hashtable trackedBodies;

	// Standard getReal3D Code Block ----------------------------------------------
	getReal3D.ClusterView clusterView;
	public void Awake()
	{
		clusterView = gameObject.AddComponent<getReal3D.ClusterView>();
		clusterView.observed = this;
	}
	
	public void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
	}
	// ----------------------------------------------------------------------------

	// Use this for initialization
	new void Start () {
		trackedBodies = new Hashtable ();
		InitOmicron ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnEvent( EventData e )
	{
		if( getReal3D.Cluster.isMaster )
		{
			if (e.serviceType == EventBase.ServiceType.ServiceTypeMocap)
			{
				int sourceID = (int)e.sourceId;
				if( !trackedBodies.ContainsKey( sourceID ) )
				{
					if( Application.HasProLicense() && Application.platform == RuntimePlatform.WindowsPlayer )
						clusterView.RPC("CreateBody", sourceID);
					else
						CreateBody(sourceID);
				}
			}
			else if (e.serviceType == EventBase.ServiceType.ServiceTypeSpeech)
			{
				string speechString = e.getExtraDataString();
				float speechConfidence = e.posx;

				Debug.Log("Received Speech: '" + speechString + "' at " +speechConfidence+ " confidence" );
			}
		}
	}

	[getReal3D.RPC]
	void CreateBody( int sourceID )
	{
		GameObject body;

		body = Instantiate(kinect2bodyPrefab) as GameObject;

		body.transform.parent = transform;
		body.transform.localPosition = Vector3.zero;
		body.GetComponent<OmicronKinectEventClient>().bodyID = sourceID;
		
		trackedBodies.Add( sourceID, body );
	}

}
