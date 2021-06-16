using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for rednering moving platforms
/// </summary>
public class MovingPlatforms : MonoBehaviour
{
    //the position that platorm will move through stored in local position
    [SerializeField] private Vector3[] points = { };

    //the speed at which it will move between them
    [SerializeField] private float speed = 10f;

    //the index into the 'points' array; this is the point we're trying to move toward
    private int nextPoint = 0;

    //where the platform was when the game started
    private Vector3 startPosition;

    //how fast this platform is currently moving in units per second
    public Vector3 velocity { get; private set; }

    //returns the point that we're currently moving toward
    private Vector3 currentPoint
    {
        get
        {
            //if we have no points return to current position
            if (points == null || points.Length == 0)
            {
                return transform.position;
            }
            //return the point we're trying to get to
            return points[nextPoint] + startPosition;
        }
    }

    private void Start()
    {
        if (points == null || points.Length < 2)
        {
            Debug.LogError("Platform needs 2 or more points to work");
            return;
        }
        //all movement points are relative to position at start of game so record that
        startPosition = transform.position;

        //start the cycle at the first point
        transform.position = currentPoint;
    }

    private void FixedUpdate()
    {
        //move toward the tartget at a fixed speed
        var newPosition = Vector3.MoveTowards(transform.position, currentPoint, speed * Time.deltaTime);

        //have we reached the target1
        if (Vector3.Distance(newPosition, currentPoint) < 0.001)
        {
            //snap to the target point
            newPosition = currentPoint;

            //move to the next target, wrapping around to start if necessary
            nextPoint += 1;
            nextPoint %= points.Length;
        }
        //calculate our current velocity in units per second
        velocity = (newPosition - transform.position) / Time.deltaTime;

        //update to our new position
        transform.position = newPosition;
    }

    //used for drawing path that platforms follow
    private void OnDrawGizmosSelected()
    {
        if (points == null || points.Length == 0)
        {
            return;
        }

        //points stored in local space so we need to offset them into world space
        Vector3 offsetPosition = transform.position;

        //when moving during run time
        if (Application.isPlaying)
        {
            offsetPosition = startPosition;
        }

        Gizmos.color = Color.blue;

        for (int i = 0; i < points.Length; i++)
        {
            //get this points and the next one arapping around to the first
            var p1 = points[i];
            var p2 = points[(i + 1) % points.Length];

            //draw the point 
            Gizmos.DrawSphere(offsetPosition + p1, 0.1f);

            //draw connecting line between points
            Gizmos.DrawLine(offsetPosition + p1, offsetPosition + p2);
        }
    }
}
