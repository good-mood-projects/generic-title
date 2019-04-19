using UnityEngine;
using System.Collections;

/*
 *  This class implements all user-input-oriented logic and damageable/colliding methods for the player.
 *      - Inputs are managed in 3 basic blocks:
 *          -- Horizontal State Machine: Uses keys W, A, S, D, Q, E to switch lanes, modify running speed and 
 *                                       select tracks if the player is in a crossroads.
 *          -- Vertical State Machine: Uses LeftShift and Space to handle jump and slide animations. 
 *          -- Shooting system: Mainly handled with primary and secondary mouse clicks.
 *      - State Machines are first defined with references to specific methods used in the MovementController script. 
 * 
 */ 

[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : Damageable {
    //Inherits from Damageable, with access to protected variable _hp

    //State machines (for animations and flow control)
    public enum HorizontalState {running, switchingLane, slowingDown, movingToCenter, changingDirection, changingOrientation, dead};
    public enum VerticalState {running, jumping, springing, finishSliding, sliding, dead};
    VerticalState currentVState;
    HorizontalState currentHState;

    // Event broker and variable.
    public GameObject eventHandler;
    EventHandler _eventBroker;
    //Vector3 nextCrossRoadsPoint;

    // Other components of the attached GameObject
    PlayerMovement _movement; // This controls entity's movement
    GunController _gunController;   //This element gives access to guns
    public InputManager inputManager;

    void Awake() // Initializes all components and subscribes to newCrossRoads event.
	{
        _eventBroker = eventHandler.GetComponent<EventHandler>();
        _eventBroker.OnCrossRoadChanged += newCrossRoadsPoint;
        _hp = startingHp;
        _godMode = godModeSelector;
        _gunController = GetComponent<GunController>();
        _movement = GetComponent<PlayerMovement>();
	}

	// Use this for initialization
	void Start () 
	{
        InitializeRun();
        currentVState = VerticalState.running; //Initially running
        currentHState = HorizontalState.running;
    }

    // Method that initializes variables after the player choses a track in a crossroads or in the Start method
    void InitializeRun()
    {
        _movement.Initialize();
    }

    // Update is called once per frame
    void Update() 
	{
        // Shooting system interface for the player
        if (inputManager.playerInputs.Shoot && currentHState!=HorizontalState.dead)
        {
            _gunController.PrimaryFire(); //Shoots if not dead
        }
        if (inputManager.playerInputs.SecondaryShoot && currentHState != HorizontalState.dead)
        {
            _gunController.SecondFire(); //Shoots if not dead
        }
    }

    // Avoid weird movements in the event of fps loss
	void FixedUpdate () {
        // VERTICAL STATE MACHINE
        if (currentVState != VerticalState.dead &&
            currentHState != HorizontalState.changingDirection && 
            currentHState != HorizontalState.changingOrientation &&
            currentHState != HorizontalState.slowingDown)
        {
            switch (currentVState)
            {
                case VerticalState.running:
                    if (inputManager.playerInputs.Jump)
                    {
                        currentVState = VerticalState.jumping;
                        _movement.Jump();
                        break;
                    }
                    if (inputManager.playerInputs.Slide)
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
                    if (inputManager.playerInputs.Jump)
                    {
                        currentVState = VerticalState.springing;
                        _movement.SpringMovement();
                    }
                    else // Maybe erase this else if things start to go bad
                    {
                        bool slideFinishing = _movement.Sliding(inputManager.playerInputs.Slide);
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
                    if (inputManager.playerInputs.Jump)
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
                    if (_movement.CheckIfCrossRoads())
                    {
                        currentHState = HorizontalState.slowingDown;
                    }
                    else if (_movement.MoveToCenter())
                    {
                        currentHState = HorizontalState.movingToCenter;
                    }
                    checkSwitchLanes();
                    _movement.SetForwardSpeed(inputManager.playerInputs.ChangeSpeed);
                    _movement.SetForwardMovement();
                    break;
                case HorizontalState.movingToCenter:
                    bool finishedMovingToCenter = _movement.MovingToCenter();
                    if (finishedMovingToCenter)
                    {
                        currentHState = HorizontalState.running;
                    }
                    break;
                case HorizontalState.slowingDown:
                    bool finishedSlowDown = _movement.SlowDown();
                    if (finishedSlowDown)
                    {
                        _eventBroker.emitChoosingLane();
                        currentHState = HorizontalState.changingDirection;
                    }
                    break;

                case HorizontalState.changingDirection:
                    changeDirection();
                    break;

                case HorizontalState.changingOrientation:
                    bool finishedChange = _movement.changeOrientation();
                    if (finishedChange)
                    {
                        currentHState = HorizontalState.changingDirection;
                    }                  
                    break;

                case HorizontalState.switchingLane:
                    if (_movement.CheckIfCrossRoads())
                    {
                        currentHState = HorizontalState.slowingDown;
                    }
                    SwitchingLanes();                  
                    break;

                default:
                    break;

            }
            //Move player
            _movement.Move();        
        }
    }

    // ---------------------------------
    // HORIZONTAL STATE MACHINE METHODS
    // ---------------------------------


    void checkSwitchLanes()
    {
        if (inputManager.playerInputs.FastMoveLeft)
        {
            _movement.FastSwitchLeft();
            currentHState = HorizontalState.switchingLane;
            //movement = new Vector3(switchDir*switchingVelocity, 0.0f, forwardVelocity);
        }
        else if (inputManager.playerInputs.FastMoveRight)
        {
            _movement.FastSwitchRight();
            currentHState = HorizontalState.switchingLane;
            //movement = new Vector3(switchDir*switchingVelocity, 0.0f, forwardVelocity);
        }
        else
        {
            bool switching = _movement.SwitchLane(inputManager.playerInputs.MoveSide);
            if (switching)
            {
                currentHState = HorizontalState.switchingLane;
            }
        }
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


    void changeDirection()
    {
        if (inputManager.playerInputs.StartRun)
        {
            _eventBroker.emitDirectionChanged(_movement.getChosenOption() - 1);
            InitializeRun();
            currentHState = HorizontalState.running;
        }
        if (inputManager.playerInputs.MoveSide < 0.0f)
        {
            if (_movement.getChosenOption() - 1 >= 0)
            {
                _movement.setChosenOption(_movement.getChosenOption() - 1);
                currentHState = HorizontalState.changingOrientation;     
            }
        }

        if (inputManager.playerInputs.MoveSide > 0.0f)
        {
            if (_movement.getChosenOption() + 1 < _movement.getOptionsLength())
            {
                _movement.setChosenOption(_movement.getChosenOption() + 1);
                currentHState = HorizontalState.changingOrientation;
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
        if (other.gameObject.GetComponent<AIBehaviour>() != null) //Check if the collision was triggered by an obstacle
        {

            AIBehaviour enemy = other.gameObject.GetComponent<AIBehaviour>();
            enemy.Hitting(this);
        }
        if (other.gameObject.GetComponent<Pickup>() != null) //Check if the collision was triggered by an obstacle
        {
            Pickup objectToPick = other.gameObject.GetComponent<Pickup>();
            objectToPick.Disable();
            if (other.gameObject.GetComponent<Gun>() != null) //Check if the collision was triggered by an obstacle
            {
                int currentNumGuns = _gunController.getGunNum(); 
                Gun[] newGuns = new Gun[currentNumGuns+1];
                if (currentNumGuns > 0)
                {
                    Gun[] currentGuns = _gunController.getGuns();
                    for (int i = 0; i < currentGuns.Length; i++)
                    {
                        newGuns[i] = currentGuns[i];
                    }
                }
                newGuns[newGuns.Length - 1] = other.gameObject.GetComponent<Gun>();
                _gunController.changeGuns(newGuns);
            }
            else if (other.gameObject.GetComponent<HealthPickup>() != null) //Check if the collision was triggered by a healthpack
            {
                HealthPickup healthPack = other.gameObject.GetComponent<HealthPickup>();
                if (_hp + healthPack.getHeal() > startingHp)
                    _hp = startingHp;
                else
                    _hp += healthPack.getHeal();
                Debug.Log("Player healed for " + healthPack.getHeal());
                Destroy(other.gameObject);
            }
            else 
            {
                Destroy(other.gameObject);
            }
        }
    }

	//Manages obstacle collisions
	void ObstacleCollide(Obstacle obstacle)
	{
		if((obstacle.high_obstacle && (currentVState==VerticalState.sliding || currentVState == VerticalState.finishSliding)) || (obstacle.low_obstacle && currentVState==VerticalState.jumping))
		{
            obstacle.Avoided();
		}
		else
        {
            if (!_godMode)
            {
                obstacle.Hitting(this);
            }
            else
            {
                obstacle.Avoided();
            }
		}
	}

    public override void Death()
    {
        currentHState = HorizontalState.dead;
        currentVState = VerticalState.dead;
    }

    public void newCrossRoadsPoint(Vector3 point)
    {
        Debug.Log("CrossRoads Point received: "+point);
        _movement.setNextCrossRoadsPoint(point);
    }

    // AI data
    public int getCurrentAILane()
    {
        return (_movement.lanes + 1 - _movement.currentLane);
        //Corrected so the AI translates its currentLane to player's currentLane
    }
}