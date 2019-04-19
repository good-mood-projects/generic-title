using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    public float rotationSpeed;
    private bool pickedUp = false;

    Quaternion baseRotation;
    // Use this for initialization
    void Start()
    {
        baseRotation = transform.rotation;
    }
	// Update is called once per frame
	void Update () {
        if (!pickedUp)
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
    }

    public void Disable()
    {
        transform.rotation = baseRotation;
        pickedUp = true;
    }
}
