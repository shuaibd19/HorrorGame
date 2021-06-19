using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//allows for limited top-down movement of a camera
public class TopDownCameraMovement : MonoBehaviour
{
    //the speed that the camera will move, in units per second
    [SerializeField] private float movementSpeed = 20;

    //the lower-left position of the camera, on its current x-z plane
    [SerializeField] private Vector2 minimumLimit = -Vector2.one;

    //the upper right position ofthe camera, on its current x-z plane
    [SerializeField] private Vector2 maximumLimit = Vector2.one;

    ////point and click position variable
    //Vector3 pointPos;

    private void Update()
    {
        //get how much the user wants to move the camera
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        //compute how much movement to apply this frame in world space
        var offset = new Vector3(horizontal, 0, vertical) * Time.deltaTime * movementSpeed;

        //figure out the new position
        var newPosition = transform.position + offset;

        //is this new position within our permitted bounds?
        if (bounds.Contains(newPosition))
        {
            //then move it
            transform.position = newPosition;
        }
        else
        {
            //otherwise figure out the closest point to the boundary, 
            //and move there instead
            transform.position = bounds.ClosestPoint(newPosition);
        }
    }

    //computes the bounding box the camera is allowed to be in
    Bounds bounds
    {
        get
        {
            //we'll create a bounding box that's zero units high, 
            //positioned at the current hieght of the camera
            var cameraHeight = transform.position.y;

            //figure ou the position of the corners of the boxes in the world space
            Vector3 minLimit = new Vector3(minimumLimit.x, cameraHeight, minimumLimit.y);

            Vector3 maxLimit = new Vector3(maximumLimit.x, cameraHeight, maximumLimit.y);

            //create a new bounds using these values and return it
            var newBounds = new Bounds();

            newBounds.min = minLimit;
            newBounds.max = maxLimit;
            return newBounds;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
