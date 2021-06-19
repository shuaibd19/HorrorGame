using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//triggers an objective when an object enters it
public class TriggerObjectiveOnClick : MonoBehaviour, IPointerClickHandler
{
    //the objective trigger and how to trigger it
    [SerializeField] private ObjectiveTrigger objective = new ObjectiveTrigger();

    //called when the player clicks on this object
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        //we just completed or failed this objective
        objective.Invoke();

        //disable this component so that it doesn't get run more than once
        this.enabled = false;
    }
}
