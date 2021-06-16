using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// implements platform riding and pushing from other objects
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlatformRiding : MonoBehaviour
{
    //reference to the character controller 
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    //every time physcis updates, check to see if our collider is overlapping something
    //if it is, push ourselves out of it
    private void FixedUpdate()
    {
        //capsule collider collision detection

        //top of the capsule
        var capsulePoint1 = transform.position + new Vector3(0, (controller.height / 2) - controller.radius, 0);

        //bottom of the capsule
        var capsulePoint2 = transform.position + new Vector3(0, (controller.height / 2) + controller.radius, 0);

        //adjust this value if you have a lot of overlaps with other colliders
        //the number chosen here (10) is arbitrary
        Collider[] overlappingColliders = new Collider[10];

        //figure out what we are colliding with
        var overlapCount = Physics.OverlapCapsuleNonAlloc(capsulePoint1, capsulePoint2, controller.radius, overlappingColliders);

        for (int i = 0; i < overlapCount; i++)
        {
            //get the collider the capsule overlaps
            var overlappingCollider = overlappingColliders[i];

            //if this collider is our controller ignore it
            if (overlappingCollider == controller)
            {
                continue;
            }

            //computing how much movement we need to perform to not overlap this collider

            //first define some varibales to store the direction and distance
            Vector3 direction;
            float distance;

            //provide information both about our collider and the collided object
            Physics.ComputePenetration(
                controller,
                transform.position,
                transform.rotation,
                overlappingCollider,
                overlappingCollider.transform.position,
                overlappingCollider.transform.rotation,
                out direction,
                out distance
                );

            //don't get moved vertically
            direction.y = 0;

            //update our position to move out of the way
            transform.position += direction * distance;
        }

        //standing on a moving platform

        //raycast to our feet if it is a moving platform then inherit it's velocity

        var ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        //the maxmimum distance we want to look for
        float maxDistance = (controller.height / 2) + 0.2f;

        //cast the ray 
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //if it did

            var platform = hit.collider.gameObject.GetComponent<MovingPlatforms>();

            if (platform != null)
            {
                //Debug.Log("We did it!");
                //update our position based on the platform's current velocity
                transform.position += platform.velocity * Time.fixedDeltaTime;
            }
        }
    }
}

