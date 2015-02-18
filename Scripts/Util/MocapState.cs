using UnityEngine;
using System.Collections;

public class MocapState : MonoBehaviour 
{
	public int mocapID;
	public Vector3 position;
	public Quaternion rotation;
	
	public MocapState(int ID)
	{
		mocapID = ID;
		position = new Vector3();
		rotation = new Quaternion();
	}


	public void UpdateMocap( Vector3 position, Quaternion orientation )
	{
		this.position = position;
		this.rotation = orientation;
	}

	public Vector3 GetPosition()
	{
		return position;
	}
	
	public Quaternion GetRotation()
	{
		return rotation;
	}
}
