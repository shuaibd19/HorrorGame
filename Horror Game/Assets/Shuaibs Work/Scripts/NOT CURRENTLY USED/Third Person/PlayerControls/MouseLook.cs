using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements mouselook. Horizontal mouse movement rotates the body 
/// around the y-axis, while vertical mouse movement rotates the head
/// around the x-axis
/// </summary>

public class MouseLook : MonoBehaviour
{
    //Speed at which we turn - tweak for mouse sensitivity to your liking
    [SerializeField] float turnSpeed = 180f;

    //upper bound of head tilt from dead level - must be bigger than loweranglelimit
    [SerializeField] float headUpperAngleLimit = 50f;

    //lower bound of head tilt from dead level
    [SerializeField] float headLowerAngleLimit = -60f;

    //current rotation from dead level in degrees
    float yaw = 0f;
    float pitch = 0f;

    //initial orientations of head and body that we use in conjuction 
    //with yaw and pitch
    Quaternion inHeadOrientation;
    Quaternion inBodyOrientation;

    //reference to the head object
    Transform head;

    // Start is called before the first frame update
    void Start()
    {
        //find head object in the children of the player (the main camera)
        head = GetComponentInChildren<Camera>().transform;

        //cache the orientation of the body and the head
        inBodyOrientation = transform.localRotation;
        inHeadOrientation = head.transform.localRotation;

        //lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        //read the current horizontal and vertical movement and scale it based on the amount
        //that's elapsed and the movement speed
        var horizontal = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * turnSpeed;
        var vertical = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * turnSpeed * -1.0f; //inverting y axis for better experience

        //update yaw and pitch values
        yaw += horizontal;
        pitch += vertical;

        //clamping the pitch so that we can never look directly up or down
        pitch = Mathf.Clamp(pitch, headLowerAngleLimit, headUpperAngleLimit);

        //computing the rotation for the body and head 
        var bodyRotation = Quaternion.AngleAxis(yaw, Vector3.up);
        var headRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        //creating new rotations for the body and head and combining them 
        //with the start rotations

        transform.localRotation = bodyRotation * inBodyOrientation; //body
        head.localRotation = headRotation * inHeadOrientation; //head

    }
}
