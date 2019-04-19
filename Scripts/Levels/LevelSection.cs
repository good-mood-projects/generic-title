using UnityEngine;
using System.Collections;

/*
 * This script must be attached to any Transition Object to register collider triggers.
 * It will then emit an event in order to comunicate with a LevelManager
 */

public class LevelSection : MonoBehaviour
{
    public GameObject eventHandler;
    public Transform nextTransition;
    EventHandler _broker;

    void Awake()
    {
        if (eventHandler == null)
        {
            Debug.LogError(gameObject.name + " is missing an event handler object");
        }
        else
        {
            _broker = eventHandler.GetComponent<EventHandler>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)       
        {
            _broker.emitSectionTransition(nextTransition);
        }
    }
}

