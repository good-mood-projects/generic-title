using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class SawObstacle : Obstacle {

    /*public int damageToDeal;
    public bool high_obstacle = false;
    public bool low_obstacle = false;
    public bool isDamageable;
    public int initialHp;
    */

    public float degreeRange;
    //public float initialDegrees;
    public float speed;
    public float orientation = 1;
    Vector3 higherPoint;

    Detector detector;
    bool playerClose = false;
    PlayerController player;

    void Awake()
    {
        
    }

    void Start()
    {
        //radRange = degreeRange * Mathf.PI / 180.0f;
        higherPoint = new Vector3(transform.position.x, GetComponent<Renderer>().bounds.max.y, transform.position.z);
        detector = GetComponentInChildren<Detector>();
        if (detector == null)
        {
            Debug.LogError("Missing child detector in SawObstacle named " + transform.name);
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
            float rotation = transform.rotation.eulerAngles.z;
            if (rotation > 180)
                rotation = -(360 - rotation);

            if (rotation > degreeRange)
                orientation = -1;

            if (rotation < -degreeRange)
                orientation = 1;

            transform.RotateAround(higherPoint, orientation * Vector3.forward, Time.fixedDeltaTime * speed);

            if (Vector3.Dot(transform.parent.forward, (player.transform.position - transform.parent.position)) < 0.0f)
            {
                playerClose = false;
            }
        }
        else
        {
            if (detector.getEnemyObject() != null)
            {
                player = detector.getEnemyObject();
            }
        }
    }
}
