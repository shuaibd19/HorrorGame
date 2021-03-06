using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements ability to interact with interactable objects
/// </summary>
public class Interacting : MonoBehaviour
{
    //the key to press to interact with an object
    [SerializeField] KeyCode interactionKey = KeyCode.F;

    //the range we can interact with objects
    [SerializeField] float interactingRange = 3;

    private SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //did the user press the interaction key?
        if (Input.GetKeyDown(interactionKey))
        {
            //attempt to interact
            AttemptInteraction();
        }
    }

    private void AttemptInteraction()
    {
        Ray ray;
        //create a ray from the current position and forward direction
        //first person camera ray cast
        //var ray = new Ray(transform.position, transform.right);

        //third person camera ray cast
        //var ray = new Ray(transform.parent.position, transform.forward);

        //if player is facing right
        if (rend.flipX)
        {
            //shoot a ray down the x axis
            ray = new Ray(transform.position, transform.right);
        }
        else
        {
            //shoot a ray down - x axis
            ray = new Ray(transform.position, -transform.right);
        }



        //store information about the ray hit
        RaycastHit hit;

        //create a layer mask that represents every layer except the players
        var everythingExceptPlayers = ~(1 << LayerMask.NameToLayer("Player"));

        //filtering the layers
        var layerMask = Physics.DefaultRaycastLayers & everythingExceptPlayers;

        //perform the raycast out
        if (Physics.Raycast(ray, out hit, interactingRange, layerMask)) 
        {
            //try to get the interactable component on the object we hit
            var interactable = hit.collider.GetComponent<Interactable>();

            //null checking
            if (interactable != null)
            {
                //signal that it was interacted with
                interactable.Interact(this.gameObject);
            }
        }
    }
}

