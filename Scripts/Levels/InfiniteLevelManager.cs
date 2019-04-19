using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

/* 
 * GameManager class currently handles all track instantiation and destroy methods
 *          Probably it'd be more accurate to call it TrackManager and move all other 
 *          features to a GameManager class in the future.
 * 
 */

public class InfiniteLevelManager : MonoBehaviour {

    //Event broker, used to communicate with PlayerController class
    public GameObject eventHandler;
    EventHandler _eventBroker;

    public InputManager inputManager;

    // All crossroads related variables
    public GameObject crossRoads;
    public float crossroadsPrefabLength;
    public int prefabsPerCrossroad;         //Indicates the ratio of prefabs between each crossroad
    bool hasTempTrack;                      //Indicates if the GameManager is generating a temporal track
    Vector3 nextCrossroadsPoint;
    GameObject tempTrack;

    //AI Related
    public GameObject AIPrefab;
    public int prefabsPerEnemy;
    public bool AIActivated;

    // Data structures used to create the track
    public CellArray[] tracksByDifficulty;
    public Transform playerPosition;

    // Variables related with track instantiation and destroy methods
    private bool generationStopped;
    public float loadRate;
    //public float loadOffset;
    public float destroyRate;
    //public float destroyOffset;
    private IEnumerator loadCoroutine;
    private IEnumerator destroyCoroutine;

    // All ordinary track prefab and camera variables
    GameObject fullTrack;
    Vector3 currentDepth = Vector3.zero;
    float farView;                               //Indicates until which point the instantiation is made   
    public float farclipPlane;
    int prefabIndex;
    public float trackLength;                    //Initial trackLength
    public float trackPrefabLength;
    private float prefabOffsetRotation = 0.0f;

    // Audio-related variables
    public AudioClip[] backgroundMusic;
    AudioSource audioSource;

    void Awake()
    {
        //Subscribes to relevant events
        _eventBroker = eventHandler.GetComponent<EventHandler>();
        _eventBroker.OnDirectionChanged += playerDirectionChanged;
        _eventBroker.OnPlayerChoosingLane += playerChoosingLane;
        inputManager.onInit += doNothing;
        //_eventBroker.OnLevelTransition += sectionChanged;
        if (tracksByDifficulty.Length <= 0)
        {
            Debug.LogError("There are no track prefabs selected");
        }
        //Initialization of music source.
        audioSource = GetComponent<AudioSource>();
        SelectBackgroundMusic();
    }

    // Use this for initialization
    void Start () {
        hasTempTrack = false;
        farView = farclipPlane;
        prefabIndex = 0;
        //Locks cursor and makes it invisible.
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Variables to store prefabs (used in async methods)
        fullTrack = new GameObject("Full_Track");
        tempTrack = new GameObject("Temp_Track");

        //Calls method once to avoid initialization without track
        loadTrackModules();

        //Launchs 2 asyncrhonous tasks to destroy and load track modules. 
        loadCoroutine = loadTrackModulesRoutine();
        StartCoroutine(loadCoroutine);

        destroyCoroutine = CheckDestroy();
        StartCoroutine(destroyCoroutine);
        //InvokeRepeating("CheckDestroy", destroyOffset, destroyRate);
        //InvokeRepeating("loadTrackModules", loadOffset, loadRate);
    }

    // Update is called once per frame
    void Update () {
        //Hide cursor and lock it (only works in fullScreen mode for now)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            if (Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }
        //Resets the scene with R keystroke.
        if (Input.GetKeyDown(KeyCode.R))
        {
            //int scene = SceneManager.GetActiveScene().buildIndex;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
        }
        // Security check in case music is not playing anymore.
        if(!audioSource.isPlaying)
        {
            SelectBackgroundMusic();
        }
    }

    /*
     * CheckDestroy method. 
     * Iterates over each of the parent track object and deletes those that aren't 
     * close to the player and also behind the player.
     */ 
    private IEnumerator CheckDestroy()
    {
        while (true)
        {
            for (int i = 0; i < fullTrack.transform.childCount; i++)
            {
                GameObject g0 = fullTrack.transform.GetChild(i).gameObject;
                Vector3 heading = g0.transform.position - playerPosition.position;
                float distance = 2*trackPrefabLength;
                if ((Mathf.Pow(Vector3.Dot(heading, playerPosition.forward), 2) > distance * distance) && (Vector3.Dot(heading, playerPosition.forward) < 0))
                {
                    //Debug.Log("Element destroyed");
                    Destroy(g0);
                }
            }
            if (!hasTempTrack)
            {
                for (int i = 0; i < tempTrack.transform.childCount; i++)
                {
                    GameObject g0 = tempTrack.transform.GetChild(i).gameObject;
                    Vector3 heading = g0.transform.position - playerPosition.position;
                    float distance = 2*trackPrefabLength;
                    if ((Mathf.Pow(Vector3.Dot(heading, playerPosition.forward),2) > distance * distance) && (Vector3.Dot(heading, playerPosition.forward) < 0))
                    {
                        //Debug.Log("Element destroyed");
                        Destroy(g0);
                    }
                }
            }
            yield return new WaitForSeconds(destroyRate);
        }
    }

    /*
     * This method selects a prefab from the pool of possible prefabs.
     * The function based on difficulty can be tweaked to take into
     * account the distance ran by the player or other variables
     */ 
    GameObject selectTrackPrefab()
    {
        if (prefabIndex < 2)
        {
            prefabIndex++;
            return tracksByDifficulty[0].getPrefab(0);
        }
        if ((prefabIndex%prefabsPerCrossroad==0)&&(!hasTempTrack))
        {
            prefabIndex = 0;
            hasTempTrack = true;
            farView = farView / 2;
            loadRate = loadRate * 2;
            return crossRoads;
        }
        if (prefabIndex % prefabsPerEnemy == 0 && AIActivated)
        {
            GameObject AIEnemy = Instantiate(AIPrefab, new Vector3(currentDepth.x,currentDepth.y+1,currentDepth.z+1), Quaternion.identity) as GameObject;
            AIEnemy.transform.Rotate(0, 180, 0);
        }
        prefabIndex++;
        // TODO: Create biased random selection based on dificulty or other factors
        // Currently selects a random difficulty, and then selects a random element in the array
        int difIndex = UnityEngine.Random.Range(0, tracksByDifficulty.Length);
        int cellIndex = UnityEngine.Random.Range(0, tracksByDifficulty[difIndex].getLength());

        return tracksByDifficulty[difIndex].getPrefab(cellIndex);
    }

    /* Procedural track generation on run-time
     * - Currently, best prodecural approach
     * * * * * * * * * * * * * * * * * * * * * * * * * * */
    private IEnumerator loadTrackModulesRoutine()
    {
        while (true)
        {
            if (!generationStopped)
            {
                loadTrackModules();
            }
            yield return new WaitForSecondsRealtime(loadRate);
        }
    }

    void loadTrackModules()
    {
        // This while loop that creates prefabs in a different way depending on the situaton
        while (farView + Vector3.Dot(playerPosition.position, playerPosition.forward) > Vector3.Dot(currentDepth, playerPosition.forward))
        {
            if (!hasTempTrack)
            {
                GameObject trackPrefab = selectTrackPrefab();
                // This previous function can change the value of "hasTempTrack" so it is necessary to check again
                if (!hasTempTrack)
                {
                    generateTrackModule(trackPrefab, trackPrefabLength);    //Keep adding the usual prefabs
                }
                else //Create a crossroads prefab and next time enter on the parent "else"
                {
                    GameObject gTrackModule = generateTrackModule(crossRoads, crossroadsPrefabLength);
                    nextCrossroadsPoint = gTrackModule.gameObject.transform.position;
                    _eventBroker.newCrossRoadsPoint(nextCrossroadsPoint);
                }
                //yield return new WaitForEndOfFrame();
            }
            // This part generates 3 different choices for the player (only when the crossroads prefab has been created)
            else
            {
                currentDepth += trackPrefabLength / 2 * playerPosition.forward;
                for (int i = -1; i < 2; i++)
                {
                    GameObject trackPrefab = selectTrackPrefab(); //Got the select inside the iteration to generate diferent types of tracks
                    GameObject gTrackModule = Instantiate(trackPrefab, currentDepth, Quaternion.identity) as GameObject;
                    gTrackModule.transform.Rotate(0, prefabOffsetRotation, 0);
                    gTrackModule.transform.RotateAround(nextCrossroadsPoint, Vector3.up, 90 * i);
                    gTrackModule.transform.parent = tempTrack.transform;
                    //yield return new WaitForEndOfFrame();
                }
                currentDepth += trackPrefabLength / 2 * playerPosition.forward;
            }
        }
    }

    // Basic method that creates a track module to add in the fullTrack
    GameObject generateTrackModule(GameObject prefab, float prefabLength)
    {
        currentDepth += prefabLength / 2 * playerPosition.forward;
        GameObject gTrackModule = Instantiate(prefab, currentDepth, Quaternion.identity) as GameObject;
        gTrackModule.transform.Rotate(0, prefabOffsetRotation, 0);
        gTrackModule.transform.parent = fullTrack.transform;
        currentDepth += prefabLength / 2 * playerPosition.forward;
        return gTrackModule;
    }

    /*
     * Methods subscribed to events
     */ 
    void playerDirectionChanged(int newDir)
    {
        //Receives -1, 0 or 1 and changes rotation (this allows to keep adding prefabs correctly)
        prefabOffsetRotation += newDir * 90;
        for (int i = 0; i < tempTrack.transform.childCount; i++)
        {
            GameObject g0 = tempTrack.transform.GetChild(i).gameObject;
            //Moves all prefabs to the main track gameObject parent.
            g0.transform.parent = null;
            g0.transform.parent = fullTrack.transform;
        }
        // Next 3 lines reposition the currentDepth variable (necessary for the instantiation of prefabs)
        Vector3 relativeP = currentDepth - nextCrossroadsPoint;
        Vector3 relativePRotated = Quaternion.Euler(0, newDir * 90, 0) * relativeP;
        currentDepth = relativePRotated + nextCrossroadsPoint;
        // Reset prefab creation variables
        prefabIndex = 0;
        generationStopped = false;
        hasTempTrack = false;
        farView = farView * 2;
        loadRate = loadRate / 2;
    }

    // Allows the gameManager to stop creating prefabs when the player has stopped moving (avoids creating erroneous prefabs due to rotations)
    void playerChoosingLane()
    {
        generationStopped = true;
    }

    /*Update AI variables
    private void sectionChanged(bool value)
    {
        AIActivated = value;
    }*/

    //Function that selects background music
    void SelectBackgroundMusic()
    {
        audioSource.clip = backgroundMusic[UnityEngine.Random.Range(0, backgroundMusic.Length)];
        audioSource.Play();
    }

    void doNothing()
    {
        return; //Avoiding regression errors;
    }
}