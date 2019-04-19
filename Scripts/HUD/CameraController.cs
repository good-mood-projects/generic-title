using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {


    float yaw = 0.0f;
    float pitch = 0.0f;
    bool movementEnabled;
    EventHandler _eventBroker;

    public GameObject eventHandler;
    public float viewMoveSpeed = 0.05f;
    public float halfViewXLimit;
    public float halfViewYLimit;
    public float lockAnimationSpeed = 10.0f;

    // Use this for initialization
    void Start () {
        movementEnabled = true;
        // Use event broker and subscribe to events using methods
        _eventBroker = eventHandler.GetComponent<EventHandler>();
        _eventBroker.OnPlayerChoosingLane += toggleCamera;
        _eventBroker.OnDirectionChanged += unlockCamera;
	}

    //Basic toggle variable method
    public void toggleCamera()
    {
        movementEnabled = !movementEnabled;
    }

    //Unlock toggles variable and resets position to the center of the screen
    public void unlockCamera(int newDir)
    {
        pitch = 0;
        yaw = 0;
        toggleCamera();
    }
	
	// Update is called once per frame
	void Update () {


        //Camera movement is implemented here
        float temPitch = pitch - viewMoveSpeed * Input.GetAxis("Mouse Y");
        if (Mathf.Abs(temPitch) < halfViewYLimit) //Limit axis Y movement
        {
            pitch = temPitch;
        }
        float tempYaw = yaw + viewMoveSpeed * Input.GetAxis("Mouse X");
        if (Mathf.Abs(tempYaw) < halfViewXLimit) //Limits axis X movement
        {
            yaw = tempYaw;
        }

        // Movement restrictions when the player is selecting a path in a crossroads.
        if (movementEnabled)
        {
            Vector3 euler = new Vector3(pitch, yaw, 0.0f);
            transform.localRotation = Quaternion.Euler(euler);
        }
        else // In case that movement is restricted, force the player to look straight ahead
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, transform.parent.forward, Time.fixedDeltaTime * lockAnimationSpeed, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
}
