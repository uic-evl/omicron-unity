using UnityEngine;

public class getRealSkybox
	: MonoBehaviour
{
	public Transform followPosition = null;
	public bool autoScale = false;

	float maxFarClip = float.MaxValue;
	
	void OnRenderObject()
	{
		if (autoScale)
		{
			if (Camera.current.GetComponent<getRealCameraUpdater>() && Camera.current.farClipPlane < maxFarClip)
				maxFarClip = Camera.current.farClipPlane;
		}
	}
	
	[ContextMenu("Update Skybox View")]
	void LateUpdate()
	{
		if (followPosition != null)
			transform.position = followPosition.position;
		else if (transform.parent)
			transform.localRotation = Quaternion.Inverse(transform.parent.localRotation);
		if (autoScale) // this will be a frame behind
		{
			float scale = maxFarClip*maxFarClip/3f;
			if (!float.IsInfinity(scale) && !float.IsNaN(scale) && scale > 0f)
				transform.localScale = Vector3.one * Mathf.Sqrt(scale);
			maxFarClip = float.MaxValue;
		}
	}
}
