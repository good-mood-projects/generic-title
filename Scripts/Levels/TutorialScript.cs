using UnityEngine;
using System.Collections;
using System;

/*
 * This script defines subsection parameters for a section named Tutorial/Level0. 
 * Implements LevelScript interface (set parameters)
 */ 
public class TutorialScript : MonoBehaviour, ILevelScript
{
    public int subsection;
    public InputManager inputManager;
    public PlayerMovement movement;
    public SubSectionParameters[] SubSections;

    void Start()
    {
        //Debug.Log("Executed");
        if (inputManager == null)
        {
            Debug.LogError("Input manager required in TutorialScript");
        }
        if (movement == null)
        {
            Debug.LogError("Player movement Controller required in TutorialScript");
        }
        inputManager.onInit += InitializeRun;
    }

    void InitializeRun()
    {
        if (subsection == 1)
            setParameters(null);
    }

    public int getCurrentSubsection()
    {
        return subsection;
    }

    public int getNumberOfSubsections()
    {
        return SubSections.Length;
    }

    public bool isAIEnabled(int subSectionIndex)
    {
        return SubSections[subSectionIndex].AI_Enabled;
    }

    public bool isShootingEnabled(int subSectionIndex)
    {
        return SubSections[subSectionIndex].shootingAllowed;
    }

    //This function will act in a different way depending on the subsection index. It'll implement changes on the enviroment and other variables
    //In order to allow a dynamic level.
    public void setParameters(Transform nextTransition)
    {
        //Set 3 movement speeds
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


        inputManager.setPlayerShooting(SubSections[subsection].shootingAllowed);
        // Creates odd lanes to the left side of the track (3ed and 5th)
        int targetNumberOfLanes = SubSections[subsection].numberOfLanes;
        if ((targetNumberOfLanes == 3 && SubSections[subsection-1].numberOfLanes == 2)|| (targetNumberOfLanes == 5 && SubSections[subsection - 1].numberOfLanes == 3))
        {
            //if (movement.currentLane == )
            movement.currentLane = movement.currentLane + 1;
        }
        movement.lanes = SubSections[subsection].numberOfLanes;

        //SubSections[subsection].AI_Enabled = true;

        subsection = subsection + 1;   
    }
}
