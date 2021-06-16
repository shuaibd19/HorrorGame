using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Detects when a given target is visible to this object. A target is
/// visible when it's both in range and in front of the target. Both
/// the range and the angle of visiblity are configurable
/// </summary>
public class EnemyVisibility : MonoBehaviour
{
    //[SerializeField] public Transform target = null; //the object we are looking for

    [SerializeField] private Transform[] targets = { };

    [Range(1f, 30f), SerializeField] public float maxDistance = 10f; //range of detection

    [Range(0f, 360f), SerializeField] public float angle = 45f; //arc angle of detection

    [SerializeField] public bool visualise = true; //if enemy detects target change the colour

    //function for other classes to determine if the target is visible to the enemy
    public bool targetIsVisible { get; private set; }

    private void Update()
    {

        //targetIsVisible = CheckVisibility(target);
        targetIsVisible = CheckVisbilityArray(targets);

        if (visualise)
        {
            //update colour to yellow if it detects target, white if it can't
            var colour = targetIsVisible ? Color.yellow : Color.white;

            GetComponent<Renderer>().material.color = colour; //update the colour of the gameobject through renderer properties
        }

    }

    //returns true if this object can see the sepcified position
    public bool CheckVisibilityToPoint(Vector3 worldPoint)
    {
        //calculate the direction from our location to the point
        var directionVector = worldPoint - transform.position;

        //calculate the number of degrees from the forward direction
        var degreesToTarget = Vector3.Angle(transform.forward, directionVector);

        //the target is within the arc of detection if it's within
        //half of the specified angle. If it's not within the arc, it's not visible
        var withinArc = degreesToTarget < (angle / 2);
        if (withinArc == false)
        {
            return false;
        }

        //figure out the distance to the target
        var distanceToTarget = directionVector.magnitude;

        //take into account the range of detection
        var rayDistance = Mathf.Min(maxDistance, distanceToTarget); //return the smaller of the 2 values

        //create a new ray that goes from the current location to the specified direction
        var ray = new Ray(transform.position, directionVector);

        //storage of ray collision
        RaycastHit hit;

        //perform raycast and check for any collisions
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            foreach (var target in targets)
            {
                if (hit.collider.transform == target)
                {
                    return true;
                }
            }
            
            ////if we hit the target
            //if (hit.collider.transform == target) {
            //    return true;
            //}

            //if we hit a barrier between us and the target we cannot see the target
            return false;
        }
        else
        {
            //there is an unobstructed line of sight between us and the target
            return true;
        }
    }

    /// <summary>
    /// Returns true if a straight line can be drawn between this object
    /// and the traget. The target must be within range and within the visible arc
    /// </summary>
    public bool CheckVisibility(Transform target)
    {

        //calculate the direction from our location to the point
        var directionVector = target.position - transform.position;

        //calculate the number of degrees from the forward direction
        var degreesToTarget = Vector3.Angle(transform.forward, directionVector);

        //the target is within the arc of detection if it's within
        //half of the specified angle. If it's not within the arc, it's not visible
        var withinArc = degreesToTarget < (angle / 2);
        if (!withinArc)
        {
            return false;
        }

        //figure out the distance to the target
        var distanceToTarget = directionVector.magnitude;

        //take into account the range of detection
        var rayDistance = Mathf.Min(maxDistance, distanceToTarget); //return the smaller of the 2 values

        //create a new ray that goes from the current location to the specified direction
        var ray = new Ray(transform.position, directionVector);

        //storage of ray collision
        RaycastHit hit;

        //records info about whether the target is in range and not occluded
        var canSee = false;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            //did the ray hit the target
            if (hit.collider.transform == target)
            {
                canSee = true;
            }

            //visualise the ray
            Debug.DrawLine(transform.position, hit.point);
        }
        else
        {
            //the ray didn't hit anything i.e. target out of range

            //visualise the ray
            Debug.DrawRay(transform.position, directionVector.normalized * rayDistance);
        }

        return canSee;
    }

    public bool CheckVisbilityArray(Transform[] tgts)
    {
        //records info about whether the target is in range and not occluded
        var canSee = false;

        foreach (var target in tgts)
        {
            //calculate the direction from our location to the point
            var directionVector = target.position - transform.position;

            //calculate the number of degrees from the forward direction
            var degreesToTarget = Vector3.Angle(transform.forward, directionVector);

            //the target is within the arc of detection if it's within
            //half of the specified angle. If it's not within the arc, it's not visible
            var withinArc = degreesToTarget < (angle / 2);
            if (!withinArc)
            {
                return false;
            }

            //figure out the distance to the target
            var distanceToTarget = directionVector.magnitude;

            //take into account the range of detection
            var rayDistance = Mathf.Min(maxDistance, distanceToTarget); //return the smaller of the 2 values

            //create a new ray that goes from the current location to the specified direction
            var ray = new Ray(transform.position, directionVector);

            //storage of ray collision
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                //did the ray hit the target
                if (hit.collider.transform == target)
                {
                    canSee = true;
                }

                //visualise the ray
                Debug.DrawLine(transform.position, hit.point);
            }
            else
            {
                //the ray didn't hit anything i.e. target out of range

                //visualise the ray
                Debug.DrawRay(transform.position, directionVector.normalized * rayDistance);
            }

        }
        return canSee;
    }

}

#if UNITY_EDITOR
//a custom editor for the enemyvisibility class. Visualises and allows for eiditing 
//the visible range

[CustomEditor(typeof(EnemyVisibility))]
public class EnemyVisibilityEditor : Editor
{
    //called when unity needs to draw the scene view
    private void OnSceneGUI()
    {
        //get a reference to the enemyvisibility script we're looking at 
        var visibility = target as EnemyVisibility;

        //start drawing at 10% opcaity
        Handles.color = new Color(1, 1, 1, 0.1f);

        //drawing the angle arc of detection
        var forwardPointMinusHalfAngle = Quaternion.Euler(0, -visibility.angle / 2, 0) * visibility.transform.forward; //fancy maths stuff

        Vector3 arcStart = forwardPointMinusHalfAngle * visibility.maxDistance;

        Handles.DrawSolidArc(
            visibility.transform.position,      //centre of the arc
            Vector3.up,                         //Up direction of the arc                                                   
            arcStart,                           //point where it begins
            visibility.angle,                   //angle of the arc
            visibility.maxDistance              //radius of the arc
            );

        //drawing a scale handle at the edge of the arc, if the user
        //drags is it updates the arc size

        //reset the handle colour to full opacity
        Handles.color = Color.white;


        //compute the position fo the handle based on the object's position,
        //direction it's facing, and the distance
        Vector3 handlePosition = visibility.transform.position + visibility.transform.forward * visibility.maxDistance;

        //draw the handle and store the result
        visibility.maxDistance = Handles.ScaleValueHandle(
            visibility.maxDistance,                 //current value
            handlePosition,                         //handle position
            visibility.transform.rotation,          //orientation
            1,                                      //size
            Handles.ConeHandleCap,                  //cap to draw
            0.25f                                   //snap to multiples of this id the snapping key is held down
            );
    }
}
#endif