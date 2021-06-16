using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enables pushing rigidbodys
public class Pushing : MonoBehaviour
{
    //define the possible types of pushing we can apply
    public enum PushMode
    {
        //don't allow any pushing
        NoPushing,

        //push by directly setting the velocity of things we hit
        DirectlySetVelocity,

        //push by applying a physical force to impact point
        ApplyForce
    }

    //the type of pushing we have selected
    [SerializeField] private PushMode pushMode = PushMode.DirectlySetVelocity;

    //the amount of force to apply when push mode is set to ApplyForce
    [SerializeField] private float pushPower = 3f;

    //called when a character collider on the object that this script is attached to touches any other collider
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //immediately exit if pushing is disabled
        if (pushMode == PushMode.NoPushing)
        {
            return;
        }

        //get the rigidbody attached to the collider we hit
        var hitRigidBody = hit.rigidbody;

        //is this rigidbody something we can push?
        if (hitRigidBody == null || hitRigidBody.isKinematic == true)
        {
            return;
        }

        //refernce to the controller that hit the object 
        CharacterController controller = hit.controller;

        //calculate the world position of the lowest point on the controller
        var footPosition = controller.transform.position.y - controller.center.y - controller.height / 2;

        //if the thing we hit is beneath us then don't push it
        if (hit.point.y <= footPosition)
        {
            return;
        }

        //apply the push based on our setting
        switch(pushMode)
        {
            case PushMode.DirectlySetVelocity:
                //directly apply the velocity - less realistic but can feel better
                hitRigidBody.velocity = controller.velocity;
                break;
            case PushMode.ApplyForce:
                //calculate how much push force to apply
                Vector3 force = controller.velocity * pushPower;

                //apply this force to the object we're pushing
                hitRigidBody.AddForceAtPosition(force, hit.point);
                break;
        }
    }
}
