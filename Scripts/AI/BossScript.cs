using UnityEngine;
using System.Collections;
using System;

public class BossScript : MonoBehaviour, ILevelScript {

    public int subsection = 0; 
    public InputManager inputManager;
    public PlayerMovement movement; //Allow calls to specific methods to turn the player.
    public SubSectionParameters[] SubSections; //2 kinds of behaviours: Moving normally, moving sideways (stopped, but rotating).
   // private bool rotating = false;

    public int getCurrentSubsection()
    {
        return subsection;
    }

    public int getNumberOfSubsections()
    {
        return -1; //This allows an infinite loop on the boss battle.
    }

    public bool isAIEnabled(int subSectionIndex)
    {
        return false;
    }

    public bool isShootingEnabled(int subSectionIndex)
    {
        return true;
    }

    public void setParameters(Transform nextTransition)
    {
        if (nextTransition != null) { // This means it is necesary to start an angular movement
            Debug.Log("Position: " + nextTransition.position);
            Debug.Log("Rotation: " + nextTransition.rotation.eulerAngles);
            Debug.Log("Player Rotation: " + movement.gameObject.transform.rotation.eulerAngles);
            //angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
            //x = Mathf.Cos(angle) * radius;
            //y = Mathf.Sin(angle) * radius;
            // STOP PLAYER 
            // INITIATE ANGULAR MOVEMENT
        }
        else
        {
            // SET PARAMETERS (SUBSECTIONS)
            //Set 3 movement speeds
            if (subsection != 0)
            {
                movement.InitiateMoveToCenter(3);
            }
            inputManager.toggleAllowInput(subsection==0);
            inputManager.setPlayerShooting(true);
            movement.lanes = SubSections[subsection].numberOfLanes;

            movement.baseForwardVelocity = (SubSections[subsection].normalPlayerSpeed);
            movement.setCurrentVelocity(movement.baseForwardVelocity);
            movement.maxSpeed = (SubSections[subsection].maxPlayerSpeed);
            movement.minSpeed = (SubSections[subsection].minPlayerSpeed);

            //Set lateral speeds
            movement.fastSwitchingVelocity = (SubSections[subsection].fastLateralSpeed);
            movement.slowSwitchingVelocity = (SubSections[subsection].slowLateralSpeed);

            //Set aceleration and brake able/disable
            inputManager.setDashMovement(SubSections[subsection].canDash);
            inputManager.canSpeedUp(SubSections[subsection].canSpeedUp);
            inputManager.canBrake(SubSections[subsection].canBrake);

            // END
            subsection++;
            if (subsection == getNumberOfSubsections()) // 
            {
                subsection = 0;
            }
        }
    }

    // Use this for initialization
    void Start () {
        //Debug.Log("Executed");
        if (movement == null)
        {
            Debug.LogError("Player movement Controller required in BossScript");
        }
    }
	
}
