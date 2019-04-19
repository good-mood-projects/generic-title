using UnityEngine;
using System.Collections;

// Auxiliary GameObject which declares events and its public emitters.  
// Mainly used to link te GameManager and PlayerController.

public class EventHandler : MonoBehaviour {

    //Event used when a new crossroads point is created
    public delegate void CrossroadsChanged(Vector3 newPoint);
    public event CrossroadsChanged OnCrossRoadChanged;

    //Event used when the player chooses a direction in a crossroads
    public delegate void DirectionChanged(int direction);
    public event DirectionChanged OnDirectionChanged;

    //Event used to indicate that the player has stopped in a crossroads
    public event System.Action OnPlayerChoosingLane;

    //Event used to indicate that a new subsection has been reached
   
    public delegate void SectionTransition(Transform nextTransition);
    public event SectionTransition OnSectionTransition;


    // Public emit handlers for every GameObject with access
    public void newCrossRoadsPoint(Vector3 point)
    {
        OnCrossRoadChanged(point);
    }

    public void emitDirectionChanged(int newDirection)
    {
        OnDirectionChanged(newDirection);
    }

    public void emitChoosingLane()
    {
        OnPlayerChoosingLane();
    }

    public void emitSectionTransition(Transform nextTransition)
    {
        OnSectionTransition(nextTransition);
    }
}
