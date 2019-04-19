using UnityEngine;
using System.Collections;

public class PlayerMovement : MovementController {

    Vector3[] options;     // Variable with possible options in a crossroad
    int chosenOption;      // Store array index of the chosen option
    public float fastSwitchingVelocity;
    public float changeSpeedRate;   //Increasing/Decreasing speed rate
    public float maxSpeed;
    public float minSpeed;

    Vector3 nextCrossRoadsPoint;
    bool requestMoveToCenter = false;
    int centerLane = -1;

    public new void Initialize()
    {
        nextCrossRoadsPoint = _rigidBody.position - new Vector3(-100.0f, -100.0f, -100.0f);
        base.Initialize();
    }


    public void FastSwitchLeft()
    {
        laneComponent = laneComponent - (currentLane - 1) * laneWidth;
        currentLane = 1;
        switchingVelocity = fastSwitchingVelocity;
        switchDir = -1;
    }

    public void FastSwitchRight()
    {
        laneComponent = laneComponent + (lanes - currentLane) * laneWidth;
        currentLane = lanes;
        switchingVelocity = fastSwitchingVelocity;
        switchDir = 1;
    }

    public void SetForwardSpeed(float speed)
    {
        //Speed adjust

        if (Mathf.Abs(speed) > 0)
        {
            float newForwardVelocity = currentForwardVelocity + speed * changeSpeedRate * Time.fixedDeltaTime;
            if ((newForwardVelocity < maxSpeed) && (newForwardVelocity > minSpeed))
            {
                currentForwardVelocity = newForwardVelocity;
            }
        }
        else
        {
            if (currentForwardVelocity != baseForwardVelocity && currentForwardVelocity != 0)
            {
                float oppositeSpeed = (currentForwardVelocity > baseForwardVelocity) ? -1 : 1;
                float newForwardVelocity = currentForwardVelocity + oppositeSpeed * changeSpeedRate * Time.fixedDeltaTime;
                currentForwardVelocity = newForwardVelocity;
                if (Mathf.Abs(currentForwardVelocity - baseForwardVelocity) < 0.1f)
                {
                    currentForwardVelocity = baseForwardVelocity;
                }

            }
        }
    }

    public bool CheckIfCrossRoads()
    {
        if (Vector3.Distance(_rigidBody.position, nextCrossRoadsPoint) < 8.0f)
        {
            options = new Vector3[] { -1 * entitySideVector, entityOrientation, entitySideVector };
            chosenOption = 1;
            return true;
        }
        return false;
    }


    public bool MovingToCenter()
    {
        SetSwitchingMovement();
        if (CheckIfMovementFinishing())
        {
            SetForwardMovement();
            return true;
        }
        return false;
    }

    public bool MoveToCenter()
    {
        bool result = requestMoveToCenter;
        if (currentLane > centerLane)
        {
            switchDir = -1;
        }
        else if (currentLane < centerLane)
            switchDir = 1;
        else
            result = false;
        if (centerLane != -1)
        {
            laneComponent = laneComponent - (currentLane - centerLane) * laneWidth;
            currentLane = centerLane;
        }
        requestMoveToCenter = false;
        return result;
    }

    public void InitiateMoveToCenter(int center)
    {
        if (center >= 1 && center <= lanes)
        {
            centerLane = center;
            requestMoveToCenter = true;
        }
    }

    /*
    *  METHODS FOR ORIENTATION CHANGING AND SELECTION (Currently only used in the player script)
    * 
    */

    public bool SlowDown()
    {
        bool finished = false;
        _rigidBody.position = Vector3.SmoothDamp(_rigidBody.position, nextCrossRoadsPoint, ref movement, 0.8f); //HARD-CODE
        if (Vector3.Distance(_rigidBody.position, nextCrossRoadsPoint) < 1.00f)
        {
            movement = Vector3.zero;
            currentLane = 3;
            finished = true;
        }
        return finished;
    }


    public bool changeOrientation()
    {
        bool finished = false;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, options[chosenOption], Time.fixedDeltaTime * turnAnimationSpeed, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
        if (Vector3.Angle(transform.forward, options[chosenOption]) < 1.0f)
        {
            finished = true;
        }
        return finished;
    }

    public int getChosenOption()
    {
        return chosenOption;
    }

    public void setChosenOption(int newOption)
    {
        chosenOption = newOption;
    }

    public int getOptionsLength()
    {
        return options.Length;
    }

    public void setNextCrossRoadsPoint(Vector3 nextPoint)
    {
        nextCrossRoadsPoint = nextPoint;
    }
}
