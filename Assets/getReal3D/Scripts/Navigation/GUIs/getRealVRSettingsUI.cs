using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/** getRealVRSettingsUI script
 * 
 * Part of: getReal3D for Unity 2.1, (C) Mechdyne, 2013
 * 
 **/

public class getRealVRSettingsUI : MonoBehaviour
{
    /// <summary>
    /// A helper class for handling a text field as float data.
    /// </summary>
	private class TextFieldAsFloatData
	{
		public TextFieldAsFloatData(string l, float fv, string tf = "##0.000")
		{
			label = l;
			floatValue = fv;
			textFormat = tf;
			minLimit = float.MinValue;
			maxLimit = float.MaxValue;
			
			if (validStyle == null)
			{
				GUISkin skin = (GUISkin)Instantiate(Resources.Load("gr2DUISkin"));
				validStyle = new GUIStyle(skin.GetStyle("TextField"));
				invalidStyle = new GUIStyle(skin.GetStyle("TextField"));

				invalidStyle.normal.textColor = Color.red;
	    		invalidStyle.active.textColor = Color.red;
				invalidStyle.hover.textColor = Color.red;
				invalidStyle.focused.textColor = Color.red;
			}
			
			style = validStyle;
		}
		
		public bool OnGUIUpdate()
		{
			bool retVal = false;
	        GUILayout.BeginHorizontal();
	        GUILayout.Label(label);
		
			textValue = (style == validStyle) ? floatValue.ToString(textFormat) : textValue;
	        textValue = GUILayout.TextField(textValue, 6, style);

			double tempVal = floatValue;
	        if (GUI.changed)
	        {
	            if (System.Double.TryParse(textValue, out tempVal) && tempVal >= minLimit && tempVal <= maxLimit)
	            {
					style = validStyle;
	                retVal = (float)tempVal != floatValue;
					floatValue = (float)tempVal;
	            }
	            else
	            {
	                style = invalidStyle;
	            }
	        }
	        GUILayout.EndHorizontal();
			return retVal;
		}
		
		public string label { get; set; }
		public float floatValue { get; set; }
		public string textFormat { get; set; }
		
		public float minLimit { get; set; }
		public float maxLimit { get; set; }
		
		string textValue { get; set; }
		GUIStyle style { get; set; }
		
		static GUIStyle validStyle = null;
		static GUIStyle invalidStyle = null;
	}
	
	TextFieldAsFloatData m_worldScale = null;
	TextFieldAsFloatData m_eyeSeparation = null;
	TextFieldAsFloatData m_translationSpeed = null;
	TextFieldAsFloatData m_rotationSpeed = null;
	
	private float m_eventTime = 0.0f;
	private int m_eventFrame = 0;
	private bool m_drawGUI = true;
	private bool m_keepOpen = false;
	
	private bool switchNavEnabled = false;
	private int m_selectedNav = 0;
	
	/// <summary>
	/// Late initialization (give dependencies a chance to run their Awake).
	/// </summary>
    void Start()
    {
		m_worldScale = new TextFieldAsFloatData("World Scale", getReal3D.Scale.userScale, "##0.0000");
		m_worldScale.minLimit = 1e-3f;
		m_worldScale.maxLimit = 1e3f;
		
		m_eyeSeparation = new TextFieldAsFloatData("Eye Separation (unknown)", getReal3D.Scale.eyeSeparation);
		m_eyeSeparation.minLimit = -1e3f;
		m_eyeSeparation.maxLimit = 1e3f;

		m_translationSpeed = new TextFieldAsFloatData("Translation Speed", 1.0f);
		m_translationSpeed.minLimit = 1e-3f;
		m_translationSpeed.maxLimit = 1e3f;

		m_rotationSpeed = new TextFieldAsFloatData("Rotation Speed", 1.0f);
		m_rotationSpeed.minLimit = 1e-3f;
		m_rotationSpeed.maxLimit = 1e3f;
		
		switchNavEnabled = gameObject.GetComponent<getRealWalkthruController>() != null
			            && gameObject.GetComponent<getRealAimAndGoController>() != null;
	}
	
	void LateUpdate()
	{
		float tempVal = 1.0f;
		if (getReal3D.Input.NavOptions.GetValue<float>("TranslationSpeed", ref tempVal))
			m_translationSpeed.floatValue = tempVal;
		if (getReal3D.Input.NavOptions.GetValue<float>("RotationSpeed", ref tempVal))
			m_rotationSpeed.floatValue = tempVal;
		m_worldScale.floatValue = getReal3D.Scale.userScale;
		m_eyeSeparation.floatValue = getReal3D.Scale.eyeSeparation;
		m_eyeSeparation.label = "Eye Separation ("+getReal3D.Scale.eyeSeparationUnitString+")";
	}
	
	void ChangeWorldScale(float val)
	{
		getReal3D.Scale.userScale = val;
	}

	void ChangeEyeSeparation(float val)
	{
		getReal3D.Scale.eyeSeparation = val;
	}

	void ChangeNavigation(int val)
	{
		if (val == 0)
		{
			gameObject.GetComponent<getRealAimAndGoController>().enabled = false;
			gameObject.GetComponent<getRealWalkthruController>().enabled = true;
		}
		else
		{
			gameObject.GetComponent<getRealWalkthruController>().enabled = false;
			gameObject.GetComponent<getRealAimAndGoController>().enabled = true;
		}
	}
	
	void ChangeTranslationSpeed(float val)
	{
		getReal3D.Input.NavOptions.SetValue<float>("TranslationSpeed", val);
	}
	
	void ChangeRotationSpeed(float val)
	{
		getReal3D.Input.NavOptions.SetValue<float>("RotationSpeed", val);
	}

    void OnGUI()
    {
		if (Event.current.isKey || Event.current.isMouse)
		{
			m_eventTime = Time.time;
		}
		if (Event.current.type == EventType.Layout && Time.frameCount != m_eventFrame)
		{
			m_eventFrame = Time.frameCount;
			m_drawGUI = Time.time < m_eventTime + 5.0f || m_keepOpen;
		}
		if (!m_drawGUI) return;
		if (!getReal3D.GUI.BeginGUI()) return;

		GUILayout.BeginArea(new Rect(0,0,220,getReal3D.GUI.height));
		GUILayout.BeginVertical("VR Settings", GUI.skin.window);

		if (m_worldScale.OnGUIUpdate())
		{
			ChangeWorldScale(m_worldScale.floatValue);
		}
		
		if (m_eyeSeparation.OnGUIUpdate())
		{
			ChangeEyeSeparation(m_eyeSeparation.floatValue);
		}
		
		if (switchNavEnabled)
		{
			m_selectedNav = GUILayout.SelectionGrid(m_selectedNav, new string[] {"Walk Through", "Aim-N-Go"}, 2);
			if (GUI.changed)
			{
				ChangeNavigation(m_selectedNav);
			}
		}
		
		if (m_translationSpeed.OnGUIUpdate())
		{
			ChangeTranslationSpeed(m_translationSpeed.floatValue);
		}

		if (m_rotationSpeed.OnGUIUpdate())
		{
			ChangeRotationSpeed(m_rotationSpeed.floatValue);
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("FPS: "+ (1.0f/Time.smoothDeltaTime).ToString("##0.00"));
		GUILayout.FlexibleSpace();
		GUILayout.Label("SPF: "+ Time.smoothDeltaTime.ToString("##0.000"));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Frame: " + Time.frameCount);
		if (getReal3D.Plugin.isDistrib())
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label("Cluster frame: " + getReal3D.Cluster.frameCount);
		}
		GUILayout.EndHorizontal();

		m_keepOpen = GUILayout.Toggle(m_keepOpen, "Keep open");

		GUILayout.EndVertical();

		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
		getReal3D.GUI.EndGUI();
    }

	void OnSerializeClusterView(getReal3D.ClusterStream stream)
	{
		stream.Serialize(ref m_keepOpen);
		if (stream.isWriting)
		{
			float temp = 0;
			temp = m_eyeSeparation.floatValue; stream.Serialize(ref temp);
			temp = m_worldScale.floatValue; stream.Serialize(ref temp);
			temp = m_translationSpeed.floatValue; stream.Serialize(ref temp);
			temp = m_rotationSpeed.floatValue; stream.Serialize(ref temp);
			stream.Serialize(ref m_selectedNav);
		}
		else
		{
			{
				float temp = 0;
				stream.Serialize(ref temp);
				if (m_eyeSeparation.floatValue != temp)
					ChangeEyeSeparation(m_eyeSeparation.floatValue = temp);
				stream.Serialize(ref temp);
				if (m_worldScale.floatValue != temp)
					ChangeWorldScale(m_worldScale.floatValue = temp);
				stream.Serialize(ref temp);
				if (m_translationSpeed.floatValue != temp)
					ChangeTranslationSpeed(m_translationSpeed.floatValue = temp);
				stream.Serialize(ref temp);
				if ( m_rotationSpeed.floatValue != temp)
					ChangeRotationSpeed(m_rotationSpeed.floatValue = temp);
			}
			{
				int temp = 0;
				stream.Serialize(ref temp);
				if (m_selectedNav != temp)
					ChangeNavigation(m_selectedNav = temp);
			}
		}
	}
}