using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


/*
 * This script manages level configurated based on parameters set by LevelScript-based entities attached to it.
 * It will use LevelSection objects (that contain subsection GameObjects) to instantiate the track and the transition colliders.
 * This colliders will trigger the increment of both index variables (for subsections and LevelScript objects) in order to change parameters.
 */ 
[RequireComponent(typeof(ILevelScript))]
public class LevelManager : MonoBehaviour {

    public PlayerController player;
    public GameObject eventHandler;

    ILevelScript[] scripts;
    ILevelScript currentScript;
    EventHandler _events;

    //int currentSubSection = 0;
    int currentScriptIndex = 0;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _events = eventHandler.GetComponent<EventHandler>();
        _events.OnSectionTransition += triggerTransition;
        scripts = GetComponents<ILevelScript>();
        currentScript = scripts[currentScriptIndex];
        //Set first parameters
        currentScript.setParameters(null);
	}

    void Update()
    {
        
    }

    //This method subscribes to an event trigger 
    void triggerTransition(Transform nextTransition)
    {
        if (currentScript.getCurrentSubsection() == currentScript.getNumberOfSubsections())
        {
            currentScriptIndex++;
            if (currentScriptIndex == scripts.Length)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(0);
            }
            else
            {
                currentScript = scripts[currentScriptIndex];
                currentScript.setParameters(nextTransition);
            }
        }
        else
        {
            currentScript.setParameters(nextTransition);
        }
    }

    /*Parameter change is used accessing the MovementController component of the player or other entities if it were necessary
    void changeParameters()
    {
        currentScript.setParameters(player.GetComponent<PlayerMovement>(), currentSubSection);
        player.toggleShooting(currentScript.isShootingEnabled(currentSubSection));
        //TODO: Communicate other parameters.
    }/

    /* DEPRECATED
     * void Initialize()
    {
        Vector3 position = Vector3.zero;
        for (int i=0; i<sections.Length; i++)
        {
            for (int j=0; j<sections[i].getLength(); j++)
            {
                GameObject subsection = sections[i].getSubsection(j);
                float orientation = subsection.transform.rotation.eulerAngles.y;
                GameObject phase = Instantiate(subsection, Vector3.zero, Quaternion.identity) as GameObject;
                phase.transform.Rotate(0, -orientation, 0);
                Vector3 oldpos = position;
                //position.x = sections[i].widths[j] / 2;
                position.z = position.z + sections[i].lengths[j];
                Vector3 relativeP = position - oldpos;
                Vector3 relativePRotated = Quaternion.Euler(0, orientation, 0) * relativeP;
                position = relativePRotated + oldpos;
                phase.transform.position = oldpos;
                phase.transform.Rotate(0, orientation, 0);
                //gTrackModule.transform.RotateAround(nextCrossroadsPoint, Vector3.up, 90 * i);
                // ADD Transition
                if (!(i == 0 && j == 0))
                {
                    BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                    Vector3 colliderCenter = Vector3.zero + new Vector3(3.0f, 2.5f, oldpos.z-20.0f);
                    Vector3 colliderSize = Vector3.zero + new Vector3(20, 5, 1);
                    boxCollider.size = colliderSize;
                    boxCollider.center = colliderCenter;                  
                    //GameObject transitionCollider = Instantiate(transition, oldpos, Quaternion.identity) as GameObject;
                    boxCollider.transform.Rotate(0, orientation, 0);
                    //transitionCollider.transform.parent = transform;
                }
            }
        }
    }*/

}
