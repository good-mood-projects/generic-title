using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script must be attached to a detector GameObject and is used by the AI to map
 * obstacles and enemies ahead of itself.
 * 
 * It is initialized at AI position and then move forward until it reaches a maximum distance.
 * 
 * Stores information about Player's and obstacles gameobject in a list. 
 */ 
public class Detector : MonoBehaviour {

    public float orientation = -1;
    public float distance = 0.0f;
    public float maxDistance = 100.0f;
    Vector3 currentPos;
    public float speed = 20.0f;
    bool detecting = false;

    List<Obstacle> obstaclesDetected;
    PlayerController enemy;
    bool enemyFound;

	// Use this for initialization
	void Start () {
        obstaclesDetected = new List<Obstacle>();
        enemyFound = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (detecting)
        {
            if (distance < maxDistance)
            {
                distance += Time.deltaTime * speed;              
            }
            transform.position = new Vector3(currentPos.x, transform.parent.position.y, transform.parent.position.z + orientation * distance);
        }
        List<Obstacle> tmpList = new List<Obstacle>();
        for (int i = 0; i<obstaclesDetected.Count; i++)
        {
            if (obstaclesDetected[i] != null)
            {
                if (Vector3.Dot(transform.parent.forward, (obstaclesDetected[i].transform.position - transform.parent.position)) > 0.0f)
                {
                    tmpList.Add(obstaclesDetected[i]);
                }
            }
        }
        if (obstaclesDetected.Count != tmpList.Count)
        {
            obstaclesDetected = tmpList;
            //Debug.Log(obstaclesDetected.Count);
        }
        if (enemy != null)
        {
            if (Vector3.Dot(transform.parent.forward, (enemy.transform.position - transform.parent.position)) < 0.0f)
            {
                enemy = null;
            }
        }
	}

    public void setMaxDistance(float newDistance)
    {
        maxDistance = newDistance;
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void setOrientation(float newOr)
    {
        orientation = newOr;
    }

    public void setPosition(Vector3 newPos)
    {
        currentPos = newPos;
        distance = 0.0f;
    }

    public void toggleDetector()
    {
        detecting = !detecting;
    }

    public List<Obstacle> getObstacleList()
    {
        return obstaclesDetected;
    }

    public bool getEnemyFoundState()
    {
        return enemyFound;
    }

    public PlayerController getEnemyObject()
    {
        return enemy;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Obstacle>() != null) //Check if the collision was triggered by an obstacle
        {
            Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
            obstaclesDetected.Add(obstacle);
        }
        if (other.gameObject.GetComponent<PlayerController>() != null) //Check if the collision was triggered by an enemy
        {
            enemy = other.gameObject.GetComponent<PlayerController>();
            enemyFound = true;
            //float distance = Mathf.Abs(Vector3.Dot(Vector3.Normalize(transform.parent.forward),transform.parent.position) - Vector3.Dot(Vector3.Normalize(transform.parent.forward),transform.position));

            //Debug.Log("Player has been detected at a distance of "+distance+" units.");
        }
    }
}
