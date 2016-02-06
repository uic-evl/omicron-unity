using UnityEngine;
using System.Collections;

public class FollowWaypoint : MonoBehaviour {

	public Transform[] waypoints;

	public bool moving = true;
	public bool loop = true;
	public int nextPosition = 0;

	public float speed = 10;
	public float distanceToWaypoint;
	public float advanceWaypointDistance = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( moving )
		{
			transform.Translate(0, 0, speed * Time.deltaTime);
		}

		transform.LookAt(waypoints[nextPosition]);

		if( Vector3.Distance(transform.position, waypoints[nextPosition].position) <= advanceWaypointDistance )
		{
			if( nextPosition < waypoints.Length - 1 )
			{
				nextPosition++;
			}
			else if( loop )
			{
				nextPosition = 0;
			}
			else
			{
				moving = false;
			}
		}
	}
}
