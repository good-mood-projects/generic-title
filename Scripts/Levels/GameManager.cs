using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject menu;
    public InputManager inputManager;
    public GameObject shootingSystem;
    public CameraController playerCamera;

    bool playing;

    void Start()
    {
        playing = true;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Escape) && playing)
        {
            Debug.Log("Paused");
            inputManager.toggleAllowInput(false);
            inputManager.setPlayerShooting(false);
            Time.timeScale = 0.0f;
            menu.SetActive(true);
            shootingSystem.SetActive(false);
            playerCamera.toggleCamera();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playing = false;
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && !playing)
        {
            Debug.Log("Resumed");
            inputManager.toggleAllowInput(true);
            inputManager.restoreShooting();
            Time.timeScale = 1.0f;
            menu.SetActive(false);
            shootingSystem.SetActive(true);
            playerCamera.toggleCamera();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playing = true;
        }
    }
}
