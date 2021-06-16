using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///implements pulling, grabbing, holding and throwing 
///a rigidbody is required beacuse we need one to connect out grabbing joint to
[RequireComponent(typeof(Rigidbody))]
public class Grabbing : MonoBehaviour
{
    //range from this object at which an object can picked up
    [SerializeField] private float grabbingRange = 3;

    //range from this object at which an object can be pulled toward us
    [SerializeField] private float pullingRange = 20;

    //the location at which objects that are picked up will be placed
    [SerializeField] private Transform holdPoint = null;

    //the key to press to pick up or drop an object 
    [SerializeField] private KeyCode grabKey = KeyCode.E;

    //the key to press to throw an object 
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;

    //the amount of force to apply on a thrown object
    [SerializeField] private float throwForce = 100f;

    //the amount of pull force on objects we are pulling toward us
    //frictional forces will be running against this
    [SerializeField] private float pullForce = 50f;

    //if the grab joint encounters this much force, break it
    [SerializeField] private float grabBreakingForce = 100f;

    //if the grab joint encounters this much torque, break it
    [SerializeField] private float grabBreakingTorque = 100f;

    //The joint that holds our grabber object. Null if we're not holding anything
    private FixedJoint grabJoint;

    //the rigidbody that we're holding. NUll if we're not holding annything
    private Rigidbody grabbedRbody;

    private void Awake()
    {
        //do some quick null checks on startup

        if (holdPoint == null)
        {
            Debug.LogError("Grab point must not be null!");
        }

        if (holdPoint.IsChildOf(transform) == false)
        {
            Debug.LogError("Grab hold point must be child of this object!");
        }

        var playerCollider = GetComponentInParent<Collider>();

        playerCollider.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void Update()
    {
        //is the user holdiung the grab key && not holding something
        if (Input.GetKey(grabKey) && grabJoint == null)
        {
            //attempt to perform a pull or a grab
            AttemptPull();
        }

        //did the user just press the grab key, and we're holding something
        else if(Input.GetKeyDown(grabKey) && grabJoint != null)
        {
            Drop();
        }

        //does the user want to throw the held object, and we're holding something
        else if(Input.GetKeyDown(throwKey) && grabJoint != null)
        {
            //apply the throw force
            Throw();
        }
    }

    //throws a held object 
    private void Throw()
    {
        //we can't throw something if we don't have anything to throw
        //do a null check
        if (grabbedRbody == null)
        {
            return;
        }

        //keep a reference to the body we were holding, because drop will reset it
        var thrownBody = grabbedRbody;

        //calculate the force to apply
        var force = transform.forward * throwForce;

        //apply it
        thrownBody.AddForce(force);

        //we need to drop what we're holding before we can throw it 
        Drop();
    }

    //drops the object
    private void Drop()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
        }

        //bail out if the object we were holding is not there anymore
        if (grabbedRbody == null)
        {
            return;
        }

        //re-enable collisions between this object and out collider(s)
        foreach (var myCollider in GetComponentsInParent<Collider>())
        {
            Physics.IgnoreCollision(myCollider, grabbedRbody.GetComponent<Collider>(), false);
        }

        grabbedRbody = null;
    }

    //attempts to pull or pick up the object directly ahead of this object
    private void AttemptPull()
    {
        //perform a raycast. If we hit something that has a rigidboyd and is not kinematic pick it up

        //create a ray from the current position and forward direction
        //first person camera ray cast
        var ray = new Ray(transform.position, transform.forward);

        //third person camera ray cast
        //var ray = new Ray(transform.parent.position, transform.forward);

        //store the raycast hit
        RaycastHit hit;

        //create a layer mask that represents every layer except the players
        var everythingExceptPlayers = ~(1 << LayerMask.NameToLayer("Player"));

        //filter the layermasks
        var layerMask = Physics.DefaultRaycastLayers & everythingExceptPlayers;

        //perform the raycast using the pulling range 
        var hitSomething = Physics.Raycast(ray, out hit, pullingRange, layerMask);

        //if nothing is in range return
        if (hitSomething == false)
        {
            return;
        }

        //if we hit something
        grabbedRbody = hit.rigidbody;
        //if the object is not a rigidbody or is kinematic
        if (grabbedRbody == null || grabbedRbody.isKinematic)
        {
            return;
        }

        //is the object within grabbing range?
        if (hit.distance < grabbingRange)
        {
            //we can pick it up

            //move the body to our grab position
            grabbedRbody.transform.position = holdPoint.position;

            //create a joint that will hold this in place and configure it
            grabJoint = gameObject.AddComponent<FixedJoint>();
            grabJoint.connectedBody = grabbedRbody;
            grabJoint.breakForce = grabBreakingForce;
            grabJoint.breakTorque = grabBreakingTorque;

            //ensure that this grabed object doesn't collide with this collider or in parent
            foreach(var myCollider in GetComponentsInParent<Collider>())
            {
                Physics.IgnoreCollision(myCollider, hit.collider, true);
            }
        }
        else
        {
            //it's not in grabbing range but in pulling range so pull it toward us
            //until it is in within grabbing range
            var pull = -transform.forward * this.pullForce;

            grabbedRbody.AddForce(pull);
        }
    }

    ////draw the location of the holdpoint
    //private void OnDrawGizmos()
    //{
    //    if (holdPoint == null)
    //    {
    //        return;
    //    }

    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawSphere(holdPoint.position, 0.1f);
    //}

    //called when a joint that's attached to the game object this
    //component is on has broken
    private void OnJointBreak(float breakForce)
    {
        //when our joint breaks, call drop to ensure that we clean up after ourselves
        Drop();
    }
}
