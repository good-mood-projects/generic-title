using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *  This class implements all user-input-oriented logic and damageable/colliding methods for the player.
 *      - Inputs are managed in 3 basic blocks:
 *          -- Horizontal State Machine: Uses keys W, A, S, D, Q, E to switch lanes, modify running speed and 
 *                                       select tracks if the player is in a crossroads.
 *          -- Vertical State Machine: Uses LeftShift and Space to handle jump and slide animations. 
 *          -- Shooting system: Mainly handled with primary and secondary mouse clicks.
 *      - State Machines are first defined with references to specific methods for each state in order of appearance. 
 * 
 */

[RequireComponent(typeof(MovementController))]
public class AIBehaviour : Damageable
{
    //Inherits from Damageable, with access to protected variable _hp

    //State machines (for animations and flow control)
    struct InputState {
        public bool Jump;
        public bool Slide; 
        public bool Right;
        public bool Left;
    };
    public enum HorizontalState { running, switchingLane, dead };
    public enum VerticalState { running, jumping, springing, finishSliding, sliding, dead };
    VerticalState currentVState;
    HorizontalState currentHState;
    InputState currentIState;

    // Other components of the attached GameObject
    MovementController _movement; // This controls entity's movement

    Detector[] _detectors;
    public float detectorOrientation = -1;
    public float numberOfDetectors = 5;
    int targetLane;

    public float updateDetectionRate = 0.05f;

    public float enemySafeDetectionDist = 9.0f;
    public float unpassableDetectionDist = 6.0f;
    public float passableDetectionDist = 4.5f;
    public float sideDetectionDist = 2.0f;

    bool moving = false;

    public float triggerMovementDistance;

    public int damageToDeal = 10;

    void Awake() // Initializes all components and subscribes to newCrossRoads event.
    {
        _detectors = GetComponentsInChildren<Detector>();
        //Debug.Log("Number of detectors: " + _detectors.Length);
        _hp = startingHp;
        //_gunController = GetComponent<GunController>();
        _movement = GetComponent<MovementController>();
    }
    // Use this for initialization
    void Start()
    {
        InitializeRun();
        currentVState = VerticalState.running; //Initially running
        currentHState = HorizontalState.running;
        //currentVIState = InputState.None;
        //currentHIState = InputState.None;
        targetLane = _movement.currentLane;
        StartCoroutine(queueEvents());
    }

    // Method that initializes variables after the player choses a track in a crossroads or in the Start method
    void InitializeRun()
    {
        _movement.Initialize();
        _movement.setSlowDown(0.0f);
        int diffLane = _movement.currentLane - 1;
        for (int i = 0; i < numberOfDetectors; i++)
        {
            _detectors[i].setPosition(Vector3.Normalize(transform.right) * (i-diffLane)*_movement.laneWidth + transform.position);
            _detectors[i].setOrientation(detectorOrientation);
            _detectors[i].toggleDetector();
        }

    }

    private IEnumerator queueEvents()
    {
        while (true)
        {
            currentIState.Left = false;
            currentIState.Right = false;
            int currentLane = _movement.currentLane - 1;
            List<Obstacle> obstacles = _detectors[currentLane].getObstacleList();
            PlayerController enemy = findEnemy();          
            if (obstacles != null)
            {
                if (obstacles.Count > 0)
                {
                    float distance = Mathf.Abs(Vector3.Dot(transform.position, transform.forward) - Vector3.Dot(obstacles[0].gameObject.transform.position, transform.forward));
                    if (distance < unpassableDetectionDist) //If there's an unpassable object, chose to move preferably towards the player's current lane
                    {
                        int checkDir = 1;
                        if (enemy != null && distance < enemySafeDetectionDist) //Avoid player movement influencing safe navigation
                        {
                            if (enemy.getCurrentAILane() < currentLane) //CAMBIAR
                            {
                                checkDir = -1;
                            }
                        }
                        if (!obstacles[0].high_obstacle && !obstacles[0].low_obstacle)
                        {
                            targetLane = checkSides(currentLane,checkDir, distance);
                            if (targetLane == -1)
                            {
                                targetLane = checkSides(currentLane, checkDir * (-1), distance);
                                if (targetLane == -1)
                                {
                                    targetLane = currentLane;
                                }
                            }
                            if (targetLane > currentLane)
                            {
                                if (!Physics.Raycast(transform.position, transform.right, sideDetectionDist))
                                    currentIState.Right = true;
                            }

                            if (targetLane < currentLane && targetLane >= 0)
                            {
                                if (!Physics.Raycast(transform.position, transform.right * (-1), sideDetectionDist))
                                    currentIState.Left = true;
                            }              
                            obstacles = _detectors[targetLane].getObstacleList();
                        }
                    }
                    if (distance < passableDetectionDist)
                    {
                        if (obstacles != null)
                        {
                            if (obstacles.Count > 0)
                            {
                                if (obstacles[0].high_obstacle)
                                {
                                    currentIState.Slide = true;
                                }
                                if (obstacles[0].low_obstacle)
                                {
                                    currentIState.Jump = true;
                                }
                            }
                        }
                    }
                    else //In case there is no CLOSE obstacles, move closer to the enemy
                    {
                        currentIState.Slide = false;
                        currentIState.Jump = false;
                        if (enemy != null)
                        {                        
                            if (enemy.getCurrentAILane()-1 > currentLane)
                            {
                                if (!Physics.Raycast(transform.position, transform.right, sideDetectionDist))
                                    currentIState.Right = true;
                            }
                            if (enemy.getCurrentAILane()-1 < currentLane)
                            {
                                if (!Physics.Raycast(transform.position, transform.right * (-1), sideDetectionDist))
                                    currentIState.Left = true;
                            }
                        }
                    }
                }
                else //In case there is no obstacles at all, move closer to the enemy
                {
                    currentIState.Slide = false;
                    currentIState.Jump = false;
                    if (enemy != null)
                    {
                        if (enemy.getCurrentAILane() - 1 > currentLane)
                        {
                            if (!Physics.Raycast(transform.position, transform.right, sideDetectionDist))
                                currentIState.Right = true;
                        }
                        if (enemy.getCurrentAILane() - 1 < currentLane)
                        {
                            if (!Physics.Raycast(transform.position, transform.right * (-1), sideDetectionDist))
                                currentIState.Left = true;
                        }

                    }
                }
            }
            yield return new WaitForSeconds(updateDetectionRate);
        }
    }

    private int checkSides(int currentLane, int side, float distance)
    {
        if (currentLane + side >= 0 && currentLane + side < _detectors.Length)
        {
            List<Obstacle> obstacles = _detectors[currentLane + side].getObstacleList();
            if (obstacles != null && obstacles.Count > 0)
            {
                float newDistance = Mathf.Abs(Vector3.Dot(transform.position, transform.forward) - Vector3.Dot(obstacles[0].gameObject.transform.position, transform.forward));
                if (!obstacles[0].high_obstacle && !obstacles[0].low_obstacle && newDistance - distance <= unpassableDetectionDist)
                {
                    int tmpLane = checkSides(currentLane + side, side, distance);
                    return tmpLane;
                }
                else
                {
                    return currentLane + side;
                }
            }
            else
            {
                return currentLane + side;
            }

        }
        else
        {
            return -1;
        }
    }

    PlayerController findEnemy()
    {
        PlayerController enemy = null;
        bool enemyFound = false;
        foreach (Detector d in _detectors)
        {
            if (d.getEnemyObject() != null)
            {         
                enemy = d.getEnemyObject();
            }
            if (d.getEnemyFoundState())
            {
                enemyFound = true;
            }
        }
        if (enemyFound && enemy == null)
        {
            Destroy(gameObject);
        }
        if (enemyFound && enemy != null)
        {
            float distanceToEnemy = Mathf.Abs(Vector3.Dot(transform.position, transform.forward) - Vector3.Dot(enemy.gameObject.transform.position, transform.forward));

            if (distanceToEnemy < triggerMovementDistance)
                moving = true;
        }
        return enemy;
    }

    // Avoid weird movements in the event of fps loss
    void FixedUpdate()
    {
        // VERTICAL STATE MACHINE
        if (currentVState != VerticalState.dead)
        {
            switch (currentVState)
            {
                case VerticalState.running:
                    if (currentIState.Jump)
                    {
                        currentVState = VerticalState.jumping;
                        _movement.Jump();
                        break;
                    }
                    if (currentIState.Slide)
                    {
                        currentVState = VerticalState.sliding;
                        _movement.Slide();
                        break;
                    }
                    break;

                case VerticalState.jumping:
                    bool jumpFinished = _movement.Jumping();
                    if (jumpFinished)
                    {
                        currentVState = VerticalState.running;
                    }
                    break;

                case VerticalState.sliding:
                    if (currentIState.Jump)
                    {
                        currentVState = VerticalState.springing;
                        _movement.SpringMovement();
                    }
                    else // Maybe erase this else if things start to go bad
                    {
                        bool slideFinishing = _movement.Sliding(currentIState.Slide);
                        if (slideFinishing)
                        {
                            currentVState = VerticalState.finishSliding;
                        }
                    }
                    break;

                case VerticalState.springing:
                    bool springFinished = _movement.FinishSliding();
                    if (springFinished)
                    {
                        currentVState = VerticalState.jumping;
                    }
                    break;

                case VerticalState.finishSliding:
                    if (currentIState.Jump)
                    {
                        currentVState = VerticalState.springing;
                        _movement.SpringMovement();
                    }
                    else
                    {
                        bool isFinished = _movement.FinishSliding();
                        if (isFinished)
                        {
                            currentVState = VerticalState.running;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        // HORIZONTAL STATE MACHINE
        if (currentHState != HorizontalState.dead)
        {
            switch (currentHState)
            {
                case HorizontalState.running:
                    _movement.CorrectPosition();
                    checkSwitchLanes();
                    break;

                case HorizontalState.switchingLane:
                    SwitchingLanes();
                    break;

                default:
                    break;

            }
            //Move player
            if (moving) _movement.Move();
        }
    }

    // ---------------------------------
    // HORIZONTAL STATE MACHINE METHODS
    // ---------------------------------


    void checkSwitchLanes()
    {
        /*if (Input.GetKey(KeyCode.Q))
        {
            _movement.FastSwitchLeft();
            currentHState = HorizontalState.switchingLane;
            //movement = new Vector3(switchDir*switchingVelocity, 0.0f, forwardVelocity);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            _movement.FastSwitchRight();
            currentHState = HorizontalState.switchingLane;
            //movement = new Vector3(switchDir*switchingVelocity, 0.0f, forwardVelocity);
        }
        else
        {*/
            int switchDir = 0;
            if (currentIState.Right)
            {
                switchDir = 1;
            }
            if (currentIState.Left)
            {
                switchDir = -1;
            }
            bool switching = _movement.SwitchLane(switchDir);
            if (switching)
            {
                currentHState = HorizontalState.switchingLane;
            }
        //}
    }

    void SwitchingLanes()
    {
        _movement.SetSwitchingMovement();
        // If the player has arrived to the target lane
        if (_movement.CheckIfMovementFinishing())
        {
            checkSwitchLanes(); //Input buffering
            if (_movement.CheckIfMovementFinishing())
            {
                currentHState = HorizontalState.running;
                _movement.SetForwardMovement();
            }
        }
    }

    // END OF STATE MACHINE METHODS

    // Manages collision with other rigidbodys. 
    // Player manages its own collisions with objects so enemies may collide with them on its own way.
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Obstacle>() != null) //Check if the collision was triggered by an obstacle
        {
            Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
            ObstacleCollide(obstacle);
        }

        if (other.gameObject.GetComponent<PlayerController>() != null) //Check if the collision was triggered by an obstacle
        {
            Destroy(gameObject);
        }
    }

    //Manages obstacle collisions
    void ObstacleCollide(Obstacle obstacle)
    {
        if ((obstacle.high_obstacle && (currentVState == VerticalState.sliding || currentVState == VerticalState.finishSliding)) || (obstacle.low_obstacle && currentVState == VerticalState.jumping))
        {
            obstacle.Avoided();
        }
        else
        {
            obstacle.Hitting(this);
        }
    }

    public void Hitting(Damageable enemy)
    {
        enemy.DamageTaken(damageToDeal);
    }
    public override void Death()
    {
        currentHState = HorizontalState.dead;
        currentVState = VerticalState.dead;
        Destroy(gameObject);
    }
}
