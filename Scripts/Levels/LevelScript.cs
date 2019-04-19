using UnityEngine;
using System.Collections;

[System.Serializable]
public struct SubSectionParameters
{
    public string name;
    public float normalPlayerSpeed;
    public float maxPlayerSpeed;
    public float minPlayerSpeed;
    public float slowLateralSpeed;
    public float fastLateralSpeed;
    public bool canSpeedUp;
    public bool canBrake;
    public bool canDash;
    public float changeSpeedRate;
    public int numberOfLanes;
    public bool shootingAllowed;
    public bool AI_Enabled;
};

//This interface defines any Level configuration script and also a struct containing info to use in configuration
public interface ILevelScript {

    void setParameters(Transform nextTransition);
    bool isShootingEnabled(int subSectionIndex);
    bool isAIEnabled(int subSectionIndex);
    int getNumberOfSubsections();
    int getCurrentSubsection();
}
