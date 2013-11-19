/*
.-------------------------------------------------------------------
|  Unity Stereoskopix 3D v027a
|-------------------------------------------------------------------
|  This all started when TheLorax began this thread:
|  http://forum.unity3d.com/threads/11775 
|-------------------------------------------------------------------
|  There were numerous contributions to the thread from 
|  aNTeNNa trEE, InfiniteAlec, Jonathan Czeck, monark and others.
|-------------------------------------------------------------------
|  checco77 of Esimple Studios wrapped the whole thing up
|  in a script & packaged it with a shader, materials, etc. 
|  http://forum.unity3d.com/threads/60961 
|  Esimple included a copyright & license:
|  Copyright (c) 2010, Esimple Studios All Rights Reserved.
|  License: Distributed under the GNU GENERAL PUBLIC LICENSE (GPL) 
| ------------------------------------------------------------------
|  I tweaked everything, added options for Side-by-Side, Over-Under,
|  Swap Left/Right, etc, along with a GUI interface: 
|  http://forum.unity3d.com/threads/63874 
|-------------------------------------------------------------------
|  Wolfram then pointed me to shaders for interlaced/checkerboard display.
|-------------------------------------------------------------------
|  In this version (v027a), modified by Arthur Nishimoto, allows
|  multiple stereo cameras in a scene. However, only one can be active at
|  a time. Also fixed a bug where the interlaced/checkboard weave would not
|  be properly displayed if the GUI is disable on startup.
|  Also added script boolean to set the weave based on the
|  current screen resolution.
|  Also added a script boolean to enable/disable stereo mode
|-------------------------------------------------------------------
|  The package consists of
|  1) this script ('stereoskopix3D.js')
|  2) a shader ('stereo3DViewMethods.shader') 
|  3) a material ('stereo3DMat')
|  4) a demo scene ('demoScene3D.scene') - WASD or arrow keys travel, 
|     L button grab objects, L button lookaround when GUI hidden.
|-------------------------------------------------------------------
|  Instructions: (NOTE: REQUIRES UNITY PRO) 
|  1. Drag this script onto your camera.
|  2. Drag 'stereoMat' into the 'Stereo Materials' field.
|  3. Hit 'Play'. 
|  4. Adjust parameters with the GUI controls, press the tab key to toggle.
|  5. To save settings from the GUI, copy them down, hit 'Stop',
|     and enter the new settings in the camera inspector.
'-------------------------------------------------------------------
|  Perry Hoberman <hoberman (at) bway.net
|-------------------------------------------------------------------
*/

@script RequireComponent (Camera)
@script AddComponentMenu ("stereoskopix/stereoskopix3D")

private var leftCamRT;   
private var rightCamRT;
private var leftCam;
private var rightCam;

public var stereoMaterial : Material;

public var enableStereo : boolean = true;
enum mode3D {Anaglyph, SideBySide, OverUnder,Interlace,Checkerboard};
private var modeStrings : String[] = ["Anaglyph", "Side By Side","Over Under","Interlace","Checkerboard"];
public var format3D = mode3D.Anaglyph;
 
enum anaType {Monochrome, HalfColor, FullColor, Optimized, Purple};
private var anaStrings : String[] = ["Gray", "Half Color", "Color", "Optimized", "Magenta"];
public var anaglyphOptions = anaType.HalfColor;

enum modeSBS {Squeezed,Unsqueezed};
private var sbsStrings : String[] = ["Squeezed","Unsqueezed"];
public var sideBySideOptions = modeSBS.Squeezed;

public var autoSetRowsAndColumns : boolean = true;
public var interlaceRows : int = 1080;
public var checkerboardColumns : int = 1920;
public var checkerboardRows : int = 1080;

public var interaxial : float = 0.25;
public var zeroParallax : float = 6.0;
private var toParallax : float = 6.0;
public var fieldOfView : float = 60.0;

public var GuiVisible : boolean = true;
public var ToggleGuiKey : KeyCode = KeyCode.Tab;
public var ToggleStereoKey : KeyCode = KeyCode.Backslash;
public var ToggleToeInKey : KeyCode = KeyCode.P;
public var LeftRightKey : KeyCode = KeyCode.Y;
public var LeftOnlyKey : KeyCode = KeyCode.U;
public var RightOnlyKey : KeyCode = KeyCode.I;
public var RightLeftKey : KeyCode = KeyCode.O;
public var trackObjKey : KeyCode = KeyCode.T;

enum method3D {Parallel,ToedIn};
private var methodStrings : String[] = ["Parallel", "Toed In"];
public var cameraMethod = method3D.Parallel;

enum cams3D {LeftRight, LeftOnly, RightOnly, RightLeft};
private var camStrings : String[] = ["Left/Right ["+LeftRightKey+"]", "LeftOnly ["+LeftOnlyKey+"]", "RightOnly ["+RightOnlyKey+"]", "Right/Left ["+RightLeftKey+"]"];
public var cameraSelect = cams3D.LeftRight;

public var cameraAspect : float = 1.33;
public var saveCustomAspect: boolean = false;

private var windowRect : Rect = Rect(20,20,600,300);

private var mouseLookScript;	// find MouseLookButton script
private var dummy : boolean = false;	// dummy button to get focus off text fields

private var toggleTrackObj : boolean = false;
private var trackObject : GameObject;

function Start () {
	if (!stereoMaterial) {
		Debug.LogError("No Stereo Material Found. Please drag 'stereoMat' into the Stereo Material Field");
		this.enabled = false;
		return;
	}
	
	leftCam = new GameObject ("leftCam", Camera);
	rightCam = new GameObject ("rightCam", Camera);

	leftCam.camera.CopyFrom (camera);
	rightCam.camera.CopyFrom (camera);
	
	leftCam.camera.renderingPath = camera.renderingPath;
	rightCam.camera.renderingPath = camera.renderingPath;
	
	fieldOfView = camera.fieldOfView;
	if (saveCustomAspect) {
		camera.aspect = cameraAspect;
	} else {
		cameraAspect = camera.aspect;
	}
	
	leftCam.AddComponent(GUILayer);
	rightCam.AddComponent(GUILayer);
	
	leftCamRT = new RenderTexture (Screen.width, Screen.height, 24);
	rightCamRT = new RenderTexture (Screen.width, Screen.height, 24);
	
	leftCam.camera.targetTexture = leftCamRT;
	rightCam.camera.targetTexture = rightCamRT;
	  
	stereoMaterial.SetTexture ("_LeftTex", leftCamRT);
	stereoMaterial.SetTexture ("_RightTex", rightCamRT);
	
	leftCam.camera.depth = camera.depth -2;
	rightCam.camera.depth = camera.depth -1;
	
	UpdateView();
   
	leftCam.transform.parent = transform;
	rightCam.transform.parent = transform;
	
	if( enableStereo ){
		camera.cullingMask = 0;
		camera.backgroundColor = Color (0,0,0,0);
		camera.clearFlags = CameraClearFlags.Nothing;
	} else {
		camera.cullingMask = -1;
		camera.clearFlags = CameraClearFlags.Skybox;
	}
	
	mouseLookScript = camera.GetComponent("MouseLookButton"); // deactivate MouseLookButton script (if it exists) when GUI visible	
	
	// Properly sets weave if GUI is disable on startup
	UpdateWeave();
	
}

function UpdateWeave(){
	if (format3D == mode3D.Interlace) {
		SetWeave(0);
	} else if (format3D == mode3D.Checkerboard) {
		SetWeave(1);
	}
}

function Update () {
	if( enableStereo ){
		camera.cullingMask = 0;
		camera.backgroundColor = Color (0,0,0,0);
		camera.clearFlags = CameraClearFlags.Nothing;
	} else {
		camera.cullingMask = -1;
		camera.clearFlags = CameraClearFlags.Skybox;
	}
	
	// Automatically set weave based on screen size
	if( autoSetRowsAndColumns && interlaceRows != Screen.height ){
		// Update the render texture resolution to match new resolution
		leftCamRT = new RenderTexture (Screen.width, Screen.height, 24);
		rightCamRT = new RenderTexture (Screen.width, Screen.height, 24);
		
		// Set the left/right eye cams to the new texture
		leftCam.camera.targetTexture = leftCamRT;
		rightCam.camera.targetTexture = rightCamRT;
	
		// Set the new texture as the stereo texture
		stereoMaterial.SetTexture ("_LeftTex", leftCamRT);
		stereoMaterial.SetTexture ("_RightTex", rightCamRT);
	
		// Update the interlace rows
		interlaceRows = Screen.height;
		checkerboardRows = Screen.height;
		checkerboardColumns = Screen.width;
		
		// Update the weave
		UpdateWeave();
	}
	
	if (Input.GetKeyUp(ToggleGuiKey)) {
		GuiVisible = !GuiVisible;
	} else if (Input.GetKeyUp(trackObjKey)) {
		toggleTrackObj = !toggleTrackObj;
	} else if (Input.GetKeyUp(ToggleStereoKey)) {
		enableStereo = !enableStereo;
	} else if (Input.GetKeyUp(LeftRightKey)) {
		 cameraSelect = cams3D.LeftRight;
	} else if (Input.GetKeyUp(LeftOnlyKey)) {
		cameraSelect = cams3D.LeftOnly;
	} else if (Input.GetKeyUp(RightOnlyKey)) {
		cameraSelect = cams3D.RightOnly;
	} else if (Input.GetKeyUp(RightLeftKey)) {
		cameraSelect = cams3D.RightLeft;
	} else if (Input.GetKeyUp(ToggleToeInKey)) {
		if (cameraMethod == method3D.ToedIn) {
			cameraMethod = method3D.Parallel;
		} else {
			cameraMethod = method3D.ToedIn;
		}
	} else if (Input.GetKey("-")) {
		if (Input.GetKey(KeyCode.LeftShift)) {
			interaxial -= 0.01;
		} else {
			interaxial -= 0.001;
		}
		interaxial = Mathf.Max(interaxial,0);
	} else if (Input.GetKey("=")) {
		if (Input.GetKey(KeyCode.LeftShift)) {
			interaxial += 0.01;
		} else {
			interaxial += 0.001;
		}
	} else if (Input.GetKey("[")) {
		if (Input.GetKey(KeyCode.LeftShift)) {
			zeroParallax -= 0.1;
		} else {
			zeroParallax -= 0.01;
		}
		zeroParallax = Mathf.Max(zeroParallax,1);
	} else if (Input.GetKey("]")) {
		if (Input.GetKey(KeyCode.LeftShift)) {
			zeroParallax += 0.1;
		} else {
			zeroParallax += 0.01;
		}
	}
 	if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt)) {
		toParallax = convergeOnObject();
		LerpZero (zeroParallax,toParallax,1.0);
	} else if (trackObject && toggleTrackObj) {
		convergeTrackObject();
	}

 }

function LerpZero(start:float, end:float, speed: float) {
	var t = 0.0;
	var rate = 1.0/speed;
	while (t < 1.0) {
		t += Time.deltaTime * rate;
		zeroParallax = Mathf.Lerp(start,end,t);
		yield;
	}
}

function convergeOnObject() {
	var hit: RaycastHit;
	var ray : Ray = leftCam.camera.ScreenPointToRay (Input.mousePosition);	// converge to clicked point
	if (Physics.Raycast (ray, hit, 100.0)) {
		trackObject = hit.collider.gameObject;
		//zeroParallax = Vector3.Distance(transform.position,hit.collider.gameObject.transform.position); // converge to center of object
		newZero = hit.distance;
		return newZero;
	}
}

function convergeTrackObject() {
  	var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
   	//if(vec.x>0 && vec.x<1 && vec.y>0 && vec.y<1 && vec.z>0) { // alternate to bounds - just check object center
  	if (GeometryUtility.TestPlanesAABB(planes,trackObject.collider.bounds)) {
		//Debug.Log(trackObject.name+" is ON CAMERA");
		var hit: RaycastHit;
  		var vec : Vector3 = Camera.main.WorldToViewportPoint(trackObject.transform.position);
		var ray : Ray = Camera.main.ViewportPointToRay (vec);
		if (Physics.Raycast(ray,hit,100.0)) {
			if (hit.collider.gameObject == trackObject && hit.distance > Camera.main.nearClipPlane) {
				zeroParallax = hit.distance;
			} else {
				//Debug.Log(trackObject.name+" is ON BUT HIDDEN");
			}
		}
	} else {
		//Debug.Log(trackObject.name+ " is OFF CAMERA");
	}
}

function LateUpdate() {
	if( enableStereo )
		UpdateView();
}

function UpdateView() {
	// Ensures this camera render texture is used if multiple stereo camera are used
	// This assumes that all other cameras are set inactive if not in use
	stereoMaterial.SetTexture ("_LeftTex", leftCamRT);
	stereoMaterial.SetTexture ("_RightTex", rightCamRT);
	
	switch (cameraSelect) {
		case cams3D.LeftRight:
			leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(interaxial/2, 0, 0);
			break;
		case cams3D.LeftOnly:
			leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2, 0, 0);
			break;
		case cams3D.RightOnly:
			leftCam.transform.position = transform.position + transform.TransformDirection(interaxial/2, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(interaxial/2, 0, 0);
			break;
		case cams3D.RightLeft:
			leftCam.transform.position = transform.position + transform.TransformDirection(interaxial/2, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2, 0, 0);
			break;
	}
	if (cameraMethod == method3D.ToedIn) {
		leftCam.camera.projectionMatrix = camera.projectionMatrix;
		rightCam.camera.projectionMatrix = camera.projectionMatrix;
		leftCam.transform.LookAt (transform.position + (transform.TransformDirection (Vector3.forward) * zeroParallax));
		rightCam.transform.LookAt (transform.position + (transform.TransformDirection (Vector3.forward) * zeroParallax));
	} else {
		leftCam.transform.rotation = transform.rotation; 
	    rightCam.transform.rotation = transform.rotation;
	    switch (cameraSelect) {
			case cams3D.LeftRight:
				leftCam.camera.projectionMatrix = projectionMatrix(true);
				rightCam.camera.projectionMatrix = projectionMatrix(false);
				break;
			case cams3D.LeftOnly:
				leftCam.camera.projectionMatrix = projectionMatrix(true);
				rightCam.camera.projectionMatrix = projectionMatrix(true);
				break;
			case cams3D.RightOnly:
				leftCam.camera.projectionMatrix = projectionMatrix(false);
				rightCam.camera.projectionMatrix = projectionMatrix(false);
				break;
			case cams3D.RightLeft:
				leftCam.camera.projectionMatrix = projectionMatrix(false);
				rightCam.camera.projectionMatrix = projectionMatrix(true);
				break;
		}
	}
}

function OnRenderImage (source:RenderTexture, destination:RenderTexture) {
	
	if( !enableStereo )
		return;
   RenderTexture.active = destination;
   GL.PushMatrix();
   GL.LoadOrtho();
   switch (format3D) {
   	case mode3D.Anaglyph:
   	    stereoMaterial.SetPass(0);
      	DrawQuad(0);
   	break;
   	case mode3D.SideBySide:
   	case mode3D.OverUnder:
		for(var i:int = 1; i <= 2; i++) {
			stereoMaterial.SetPass(i);
			DrawQuad(i);
		}
	break;
	case mode3D.Interlace:
	case mode3D.Checkerboard:
		stereoMaterial.SetPass(3);
		DrawQuad(3);
   	break;
   	default:
   	break;
   }
   GL.PopMatrix();
}

function OnGUI () {
   	if (GuiVisible) {
    	windowRect = GUILayout.Window (0, windowRect, DoWindow, "Stereoskopix 3D Controls");
    	if (mouseLookScript) mouseLookScript.suppress = true;
	} else {
		if (mouseLookScript) mouseLookScript.suppress = false;
	}
}

function DoWindow (windowID : int) {
	GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label ("Mode");
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			format3D = GUILayout.SelectionGrid (format3D, modeStrings,1,GUILayout.MaxWidth(100));
			if (GUI.changed) {
				if (format3D == mode3D.Interlace) {
					SetWeave(0);
				} else if (format3D == mode3D.Checkerboard) {
					SetWeave(1);
				}
			}
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label ("Options");
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				anaglyphOptions = GUILayout.Toolbar (anaglyphOptions, anaStrings,GUILayout.MaxWidth(500));
				if (GUI.changed) {
					SetAnaglyphType();
				}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				sideBySideOptions = GUILayout.Toolbar (sideBySideOptions, sbsStrings,GUILayout.MaxWidth(200));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label ("[Alt-Click on Object to Converge]");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				GUILayout.Label ("Rows",GUILayout.MinWidth(60));
				interlaceString = System.Convert.ToString(interlaceRows);
				interlaceString = GUILayout.TextField (interlaceString,4,GUILayout.MaxWidth(50));
				interlaceRows = System.Convert.ToDouble(interlaceString);
				if (GUI.changed) {
					if (format3D == mode3D.Interlace) {
						SetWeave(0);
					}
				}
				if (GUILayout.Button("-")) {
					interlaceRows -= 1;
					SetWeave(0);
				}
				if (GUILayout.Button("+")) {
					interlaceRows +=1;
					SetWeave(0);
				}	  	
				GUILayout.FlexibleSpace();
				toggleTrackObj = GUILayout.Toggle(toggleTrackObj, "Track Object [T]");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				GUILayout.Label ("Columns",GUILayout.MinWidth(60));
				checkerString = System.Convert.ToString(checkerboardColumns);
				checkerString = GUILayout.TextField (checkerString,4,GUILayout.MaxWidth(50));
				checkerboardColumns = System.Convert.ToDouble(checkerString);
				GUILayout.Label ("Rows");
				checkerString = System.Convert.ToString(checkerboardRows);
				checkerString = GUILayout.TextField (checkerString,4,GUILayout.MaxWidth(50));
				checkerboardRows = System.Convert.ToDouble(checkerString);
				if (GUI.changed) {
					if (format3D == mode3D.Checkerboard) {
						SetWeave(1);
					}
				}
				if (GUILayout.Button ("enter")) {
        			GUI.FocusControl ("focus");
				}
				GUILayout.FlexibleSpace();
				GUILayout.Label("  ["+ToggleGuiKey+" toggles controls]");
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	GUILayout.EndHorizontal();
	GUILayout.Space(15);
	GUILayout.BeginHorizontal();
			GUILayout.Label ("Camera Select",GUILayout.MinWidth(120));
			GUILayout.Space(15);
			cameraSelect = GUILayout.Toolbar (cameraSelect, camStrings,GUILayout.MaxWidth(400));
			GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	GUILayout.BeginHorizontal();
			GUILayout.Label ("Camera Method [P]",GUILayout.MinWidth(120));
			GUILayout.Space(15);
			cameraMethod = GUILayout.Toolbar (cameraMethod, methodStrings,GUILayout.MaxWidth(200));
			GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	GUILayout.BeginHorizontal();
		GUILayout.Label ("Interaxial  - +",GUILayout.MinWidth(120));
		interaxial = GUILayout.HorizontalSlider (interaxial, 0.0, 5.0,GUILayout.MaxWidth(300));
		GUILayout.Label (" "+interaxial);
		GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	GUILayout.BeginHorizontal();
		GUILayout.Label ("Zero Parallax  [ ] ",GUILayout.MinWidth(120));
		zeroParallax = GUILayout.HorizontalSlider (zeroParallax, 1.0, 100.0,GUILayout.MaxWidth(300));
		GUILayout.Label (" "+zeroParallax);
		GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	GUILayout.BeginHorizontal();
		GUILayout.Label ("Field of View",GUILayout.MinWidth(120));
		fieldOfView = GUILayout.HorizontalSlider (fieldOfView, 1.0, 180.0,GUILayout.MaxWidth(300));
		camera.fieldOfView = fieldOfView;
		GUILayout.Label (" "+fieldOfView);
		GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	GUILayout.BeginHorizontal();
		GUILayout.Label ("Aspect Ratio",GUILayout.MinWidth(120));
		if (GUILayout.Button("Reset")) {
			camera.ResetAspect();
			cameraAspect = camera.aspect; 
		}
		cameraAspect = GUILayout.HorizontalSlider (cameraAspect, 0.1, 4.0,GUILayout.MaxWidth(250));
		camera.aspect = cameraAspect;
		GUILayout.Label (" "+cameraAspect);
		GUILayout.FlexibleSpace();
		GUI.SetNextControlName ("focus");
		dummy = GUILayout.Toggle(dummy, "");

	GUILayout.EndHorizontal();
	GUI.DragWindow();
}

private function SetWeave(xy) {
	if (xy) {
		stereoMaterial.SetFloat("_Weave_X", checkerboardColumns);
		stereoMaterial.SetFloat("_Weave_Y", checkerboardRows);
	} else {
		stereoMaterial.SetFloat("_Weave_X", 1);
		stereoMaterial.SetFloat("_Weave_Y", interlaceRows);
	}
}

private function SetAnaglyphType() {
   switch (anaglyphOptions) {
   		case anaType.Monochrome:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0.299,0.587,0.114,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0.299,0.587,0.114,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0.299,0.587,0.114,0));
	   	break;
   		case anaType.HalfColor:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0.299,0.587,0.114,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,1,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0,0,1,0));
	   	break;
   		case anaType.FullColor:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(1,0,0,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,1,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0,0,1,0));
	   	break;
   		case anaType.Optimized:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0,0.7,0.3,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,1,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0,0,1,0));
	   	break;
   		case anaType.Purple:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0.299,0.587,0.114,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0.299,0.587,0.114,0));
	   	break;
   }
}
 
private function DrawQuad(cam) {
	if (format3D == mode3D.Anaglyph) {
	   		GL.Begin (GL.QUADS);      
	      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.1 );
	      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1, 0.0, 0.1 );
	      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1, 1.0, 0.1 );
	      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 1.0, 0.1 );
	   		GL.End();
	} else {
		if (format3D==mode3D.SideBySide) {
			if (cam==1) {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 0.5, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 0.5, 1.0, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 1.0, 0.1 );
		   		GL.End();
			} else {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.5, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1.0, 1.0, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.5, 1.0, 0.1 );
		   		GL.End();
			}
		} else if (format3D == mode3D.OverUnder) {
			if (cam==1) {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.5, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1.0, 0.5, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1.0, 1.0, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 1.0, 0.1 );
		   		GL.End();
			} else {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1.0, 0.5, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 0.5, 0.1 );
		   		GL.End();
			} 
		} else if (format3D == mode3D.Interlace || format3D == mode3D.Checkerboard) {
	   		GL.Begin (GL.QUADS);      
	      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.1 );
	      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1, 0.0, 0.1 );
	      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1, 1.0, 0.1 );
	      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 1.0, 0.1 );
	   		GL.End();
		}
	}
} 

function PerspectiveOffCenter(
    left : float, right : float,
    bottom : float, top : float,
    near : float, far : float ) : Matrix4x4 {       
    var x =  (2.0 * near) / (right - left);
    var y =  (2.0 * near) / (top - bottom);
    var a =  (right + left) / (right - left);
    var b =  (top + bottom) / (top - bottom);
    var c = -(far + near) / (far - near);
    var d = -(2.0 * far * near) / (far - near);
    var e = -1.0;

    var m : Matrix4x4;
    m[0,0] = x;  m[0,1] = 0;  m[0,2] = a;  m[0,3] = 0;
    m[1,0] = 0;  m[1,1] = y;  m[1,2] = b;  m[1,3] = 0;
    m[2,0] = 0;  m[2,1] = 0;  m[2,2] = c;  m[2,3] = d;
    m[3,0] = 0;  m[3,1] = 0;  m[3,2] = e;  m[3,3] = 0;
    return m;
}

function projectionMatrix(isLeftCam : boolean) : Matrix4x4 {
   var left : float;
   var right : float;
   var a : float;
   var b : float;
   var FOVrad : float;
   var aspect: float = camera.aspect;
   var tempAspect: float;
   if (sideBySideOptions == modeSBS.Unsqueezed && format3D == mode3D.SideBySide) {
   		FOVrad = camera.fieldOfView / 90.0 * Mathf.PI;
   		tempAspect = aspect/2;
   } else {
   		FOVrad = camera.fieldOfView / 180.0 * Mathf.PI;
   		tempAspect = aspect;
   }
 
   a = camera.nearClipPlane * Mathf.Tan(FOVrad * 0.5);
   b = camera.nearClipPlane / (zeroParallax + camera.nearClipPlane);
   
   if (isLeftCam) {
      left  = - tempAspect * a + (interaxial/2) * b;
      right =   tempAspect * a + (interaxial/2) * b;
   }
   else {
      left  = - tempAspect * a - (interaxial/2) * b;
      right =   tempAspect * a - (interaxial/2) * b;
   }

   return PerspectiveOffCenter(left, right, -a, a, camera.nearClipPlane, camera.farClipPlane);
    
} 