using UnityEngine;
using System.Collections;

public class FlockingSeparationScript : MonoBehaviour
{
	ArrayList separationObjects;
	
	Vector3 separationVector;
	
	public int separationObjectCount;
	
	// Use this for initialization
	void Start()
	{
		separationObjects = new ArrayList();
	}
	
	// Update is called once per frame
	void Update()
	{
		Vector3 centerPos = transform.position;
		separationObjectCount = separationObjects.Count;
		
        foreach( GameObject other in separationObjects )
        {
			// Other boid
			Vector3 otherBoidPos = other.transform.position;
			
			// Distance to other
			//float distanceToOther = Vector3.Distance( centerPos, otherBoidPos );
			
			// Separation
			separationVector += centerPos - otherBoidPos;
        }// for
		
				
		if( separationObjectCount == 0 )
		{
			separationVector = Vector3.zero;
		}
	}
	
	public Vector3 GetSeparationVector()
	{
		return separationVector;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if( separationObjects != null  && other.gameObject.name.Equals("SeparationCollider") )
		{
			separationObjects.Add(other.gameObject);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if( separationObjects != null && other.gameObject.name.Equals("SeparationCollider") )
		{
			separationObjects.Remove(other.gameObject);
		}
	}
}
