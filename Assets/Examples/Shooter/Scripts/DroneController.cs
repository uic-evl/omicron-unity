using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour {

    public GameObject laserPrefab;
    public Transform barrel;

    public bool fire;

    public Transform goal;
    public GameObject explosionEffect;

	// Use this for initialization
	void Start () {
        goal = GameObject.FindGameObjectWithTag("Player").transform;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position; 
	}
	
	// Update is called once per frame
	void Update () {
        if (fire && laserPrefab)
        {
            GameObject laser = Instantiate(laserPrefab, barrel.position, barrel.rotation) as GameObject;
            laser.SendMessage("SetParent", transform.root);
            fire = false;
        }
	}

    void OnWandButtonHoldTrigger(CAVE2Manager.Button button)
    {
        if (explosionEffect)
        {
            GameObject explodie = Instantiate(explosionEffect, transform.position, transform.rotation) as GameObject;
            Destroy(explodie, 3);
        }
        Destroy(gameObject);
    }
}
