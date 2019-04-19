using UnityEngine;
using System.Collections;

/*
 *  This class implements all movement commands for any given entity, and helps to separate from input-management or AI logic.
 *      It needs an animator (for vertical movements such as slide, jump, spring...) and a rigidbody (for horizontal move in any axis).
 */

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour {

    Animator _animController;       //Manages the animations on the main character
    protected Rigidbody _rigidBody;	//Must be attached to an object with rigidbody

    public float jumpSlideDuration; //Duration of the state being active in secs
    public float maxSlideDuration;  //Maximum duration of the slide state being active
    float jumpSlideTime;	//State changed timer


    // Relative and absolute position variables and movements.
    public float laneWidth;
    public int currentLane; 	//Player starting lane
    public int lanes;           //Number of lanes
    protected Vector3 entityOrientation;
    protected float laneComponent;	//Lane position (x axis for now)
    protected float switchDir;		//Direction of movement in the X axis.
    protected Vector3 entitySideVector;

    //Speed and animation variables
    public float slowSwitchingVelocity = 10f;
    public float turnAnimationSpeed;
    protected Vector3 movement;       //Current planned movement
    protected float switchingVelocity;
    protected float currentForwardVelocity;
    public float baseForwardVelocity;


    // Use this for initialization
    void Awake () {
        _animController = GetComponentInChildren<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        switchingVelocity = slowSwitchingVelocity;
    }
	
	public void Initialize()
    {
        
        currentForwardVelocity = baseForwardVelocity;
        entityOrientation = Vector3.Normalize(transform.forward);
        entitySideVector = Vector3.Normalize(transform.right);
        movement = entityOrientation * currentForwardVelocity;
        laneComponent = Vector3.Dot(_rigidBody.position, entitySideVector);
        switchDir = 0.0f;
        jumpSlideTime = 0.0f;
    }

    public void Jump()
    {
        _animController.SetTrigger("Jump");
        //Debug.Log("Player is jumping");
    }

    public void Slide()
    {
        _animController.SetTrigger("Slide");
        //Debug.Log("Player is sliding");
    }

    public bool Jumping()
    {
        bool finished = false;
        jumpSlideTime += Time.fixedDeltaTime;
        if (jumpSlideTime >= jumpSlideDuration)
        {
            jumpSlideTime = 0.0f;
            //Debug.Log("Player is running");
            finished = true;
        }
        return finished;
    }

    public bool Sliding(bool keepSliding)
    {
        bool finished = false;
        jumpSlideTime += Time.fixedDeltaTime;
        if (((jumpSlideTime >= jumpSlideDuration / 2) && (!keepSliding)) || (jumpSlideTime >= maxSlideDuration))
        {
            jumpSlideTime = 0.0f;
            //Debug.Log("Player is running");
            _animController.SetTrigger("FinishSlide");
            finished = true;
        }
        return finished;
    }

    public bool FinishSliding()
    {
        bool finished = false;
        jumpSlideTime += Time.fixedDeltaTime;
        if (jumpSlideTime >= jumpSlideDuration / 2)
        {
            jumpSlideTime = 0.0f;
            //Debug.Log("Player is running");
            finished = true;
        }
        return finished;
    }

    public void SpringMovement()
    {
        _animController.SetTrigger("Jump"); //Set a smoth change from slide to jump animation
        jumpSlideTime = 0.0f;
        //Debug.Log("Player is jumping");
    }

    public bool SwitchLane(float sideMov)
    {
        bool switching = false;
        if (sideMov != 0)
        {
            switchDir = sideMov;
            int tempLane = currentLane + (int)sideMov;

            if (tempLane <= lanes && tempLane >= 1) //Security check: limits of the track
            {
                laneComponent = laneComponent + sideMov * laneWidth;
                switchingVelocity = slowSwitchingVelocity;
                switching = true;
               
                currentLane = tempLane;
                // movement = new Vector3(switchingVelocity * switchDir, 0.0f, forwardVelocity);
            }
        }
        return switching;
    }

    public void SetSwitchingMovement()
    {
        movement = entityOrientation * currentForwardVelocity + switchingVelocity * switchDir * entitySideVector;
    }

    public void SetForwardMovement()
    {
        movement = entityOrientation * currentForwardVelocity;
    }

    public void CorrectPosition()
    {
        Vector3 orientationVector = Vector3.Dot(_rigidBody.position, entityOrientation) * entityOrientation;
        _rigidBody.position = orientationVector + laneComponent * entitySideVector;
    }

    public bool CheckIfMovementFinishing()
    {
        return Mathf.Abs(Vector3.Dot(_rigidBody.position, entitySideVector) - laneComponent) < 0.25f;
    }

    public void Move()
    {
        _rigidBody.MovePosition(_rigidBody.position + movement * Time.fixedDeltaTime);
    }

    public void setCurrentVelocity(float newVelocity)
    {
        currentForwardVelocity = newVelocity;
    }

    public void setSlowDown(float newVelocity)
    {
        baseForwardVelocity = newVelocity;
    }

    public float getCurrentVelocity()
    {
        return currentForwardVelocity;
    }

}
