using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : MonoBehaviour
{
    //faster movement button
    [SerializeField] KeyCode moveFaster = KeyCode.LeftShift;

    //the movement speed in units per seconds
    [SerializeField] float moveSpeed = 6;

    //the height of a jump in units
    [SerializeField] float jumpHeight = 2;

    //the rate at which our vertical speed will be reduced in units 
    //per second
    [SerializeField] float gravity = 20;

    //the degree to which we can control our movement while in midair
    [Range(0, 10), SerializeField] float airControl = 5;

    //our current movement direction - in air = no control, on ground = control
    Vector3 moveDirection = Vector3.zero;

    //a cached reference to the character controller which we will use often
    CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        ///the following input vector describes the player's local space movement
        ///if on the ground this will immediately become the players movement
        ///if in the air it will interpolate between current movement and this vector
        ///to simulate momentum
        ///

        var input = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
            );

        //if the player is holding move faster button (left shift)
        if (Input.GetKey(moveFaster))
        {
            input *= moveSpeed * 2;
        }
        else
        {
            //multiply this movement with desired movement speed
            input *= moveSpeed;
        }

        //Controller specific - controller's move method uses world space directions 
        //so we need to convert this direction to world space
        input = transform.TransformDirection(input);

        //Is the controllers bottom collider touching something?
        if (controller.isGrounded)
        {
            //figure out how much movement is needed to be applied
            moveDirection = input;

            //is the player pressing the jump button?
            if (Input.GetButton("Jump"))
            {
                /// See reference for math explaination for jump calculation
                /// Unity Game Development Cookbook(p. 270)
                moveDirection.y = Mathf.Sqrt(2 * gravity * jumpHeight);
            }
            else
            {
                //we're on the ground and not jumping therefore set down momentum to 0
                moveDirection.y = 0;
            }

        }
        else
        {
            //we are not grounded and in the air therefore bring the player toward desired
            //input location 
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        //apply gravity over time
        moveDirection.y -= gravity * Time.deltaTime;

        //finally move the controller 
        controller.Move(moveDirection * Time.deltaTime);
    }
}
