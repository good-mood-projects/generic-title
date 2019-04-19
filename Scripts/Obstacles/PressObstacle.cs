using UnityEngine;
using System.Collections;


public class PressObstacle : Obstacle
{
    // NOW MOVES IN A LINEAR FASHION. DELAY COULD BE IMPLEMENTED
    public float height;
    public float upSpeed;
    public float downSpeed;
    Vector3 initialPos;
    int direction = -1; //1 UP, -1 DOWN

    Detector detector;
    bool playerClose = false;
    PlayerController player;
    
    void Start()
    {
        initialPos = transform.position;   
        detector = GetComponentInChildren<Detector>();
        if (detector == null)
        {
            Debug.LogError("Missing child detector in PressObstacle named " + transform.name);
        }
        else
        {
            detector.setPosition(transform.position);
            detector.setOrientation(-1);
            detector.toggleDetector();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerClose)
        {
            //Implement movement
            if (direction == -1) // GOING DOWN
            {
                moveObstacle(new Vector3(transform.position.x, transform.position.y - downSpeed * Time.fixedDeltaTime, transform.position.z));
                if (initialPos.y - transform.position.y >= height)
                {
                    direction = 1;
                }
            }
            else
            {
                moveObstacle(new Vector3(transform.position.x, transform.position.y + upSpeed * Time.fixedDeltaTime, transform.position.z));
                if (initialPos.y - transform.position.y <= 0.0f)
                {
                    direction = -1;
                }
            }


            if (player == null)
            {
                playerClose = false;
            }
        }
        else
        {
            if (detector.getEnemyObject() != null)
            {
                player = detector.getEnemyObject();
                playerClose = true;
            }
        }
    }
}
