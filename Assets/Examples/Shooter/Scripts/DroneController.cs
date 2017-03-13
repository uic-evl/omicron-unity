using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour {

    public GameObject laserPrefab;
    public Transform barrel;

    public bool fire;

    public Transform goal;
    public GameObject explosionEffect;

	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        goal = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        
	}
	
	// Update is called once per frame
	void Update () {
		agent.destination = goal.position; 
        if (fire && laserPrefab)
        {
            GameObject laser = Instantiate(laserPrefab, barrel.position, barrel.rotation) as GameObject;
            laser.SendMessage("SetParent", transform.root);
            fire = false;
        }
	}

    void OnWandButtonHoldTrigger(CAVE2.Button button)
    {
        if (explosionEffect)
        {
            GameObject explodie = Instantiate(explosionEffect, transform.position, transform.rotation) as GameObject;
            Destroy(explodie, 3);
        }
        Destroy(gameObject);
    }
}
