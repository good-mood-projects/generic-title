using UnityEngine;
using System.Collections;

[System.Serializable]
public struct CustomInputs
{
    public bool Jump;
    public bool Slide;
    public float MoveSide;
    public bool Shoot;
    public bool SecondaryShoot;
    public bool FastMoveRight;
    public bool FastMoveLeft;
    public float ChangeSpeed;
    public bool StartRun;
    public float ChangeOrientationLeft;
    public float ChangeOrientationRight;
}


public class InputManager : MonoBehaviour {

    public CustomInputs playerInputs;

    public System.Action onInit;

    bool shootingAllowed;
    bool dashingAllowed;
    bool brakingAllowed;
    bool accelerationAllowed;
    bool couldShootMem;
    // Use this for initialization


    bool anyInputAllowed;

    void Start () {
        anyInputAllowed = true;
        InitializeInputs(ref playerInputs);
    }
	

	/*void FixedUpdate () {
        playerInputs = getInputs();
	}
    */

    void Update()
    {
        playerInputs = getFrameDependentInput();
        playerInputs = getInputs();
    }


    CustomInputs getFrameDependentInput()
    {
        CustomInputs tmpInputs = playerInputs;
        tmpInputs.Shoot = false;
        tmpInputs.SecondaryShoot = false;
        tmpInputs.StartRun = false;

        if (anyInputAllowed)
        {
            if (Input.GetMouseButton(0) && shootingAllowed)
                tmpInputs.Shoot = true;

            if (Input.GetMouseButton(1) && shootingAllowed)
                tmpInputs.SecondaryShoot = true;

            if (Input.anyKey && !Input.GetKey(KeyCode.Escape) && playerInputs.ChangeSpeed == 0.0f)
            {
                tmpInputs.StartRun = true;

                onInit();
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && shootingAllowed)
                tmpInputs.Shoot = true;

            if (Input.GetMouseButton(1) && shootingAllowed)
                tmpInputs.SecondaryShoot = true;
        }
        return tmpInputs;
    }

    CustomInputs getInputs()
    {
        CustomInputs tmpInputs = playerInputs;
        InitializeInputs(ref tmpInputs);
        if (anyInputAllowed)
        {
            if (Input.GetKey(KeyCode.Space))
                tmpInputs.Jump = true;


            if (Input.GetKey(KeyCode.LeftShift))
                tmpInputs.Slide = true;

            if (Input.GetKey(KeyCode.Q) && dashingAllowed)
                tmpInputs.FastMoveLeft = true;

            if (Input.GetKey(KeyCode.E) && dashingAllowed)
                tmpInputs.FastMoveRight = true;

            if ((Input.GetAxisRaw("Vertical") > 0 && accelerationAllowed) || (Input.GetAxisRaw("Vertical") < 0 && brakingAllowed))
                tmpInputs.ChangeSpeed = Input.GetAxisRaw("Vertical");

            tmpInputs.MoveSide = Input.GetAxisRaw("Horizontal");
        }
        return tmpInputs;
    }

    void InitializeInputs(ref CustomInputs inputs)
    {
        inputs.ChangeSpeed = 0.0f;
        inputs.FastMoveLeft = false;
        inputs.FastMoveRight = false;
        inputs.Slide = false;
        inputs.Jump = false;
        inputs.MoveSide = 0.0f;
        inputs.ChangeOrientationLeft = 0.0f;
        inputs.ChangeOrientationRight = 0.0f;
    }

    public void setPlayerShooting(bool value)
    {
        couldShootMem = shootingAllowed;
        shootingAllowed = value;
    }

    public void setDashMovement(bool value)
    {
        dashingAllowed = value;
    }

    public void canSpeedUp(bool value)
    {
        accelerationAllowed = value;
    }

    public void canBrake(bool value)
    {
        brakingAllowed = value;
    }

    public void toggleAllowInput(bool value)
    {
        anyInputAllowed = value;
    }

    public void restoreShooting()
    {
        shootingAllowed = couldShootMem;
    }
}
