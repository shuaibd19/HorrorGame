using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Civilian : MonoBehaviour
{
    //FSM states for a civilian
    private enum State
    {
        FreeRoam,
        Flee
    }

    //current state of the civilian at the start
    private State currentState = State.FreeRoam;

    //reference to the player we are avoiding
    [SerializeField] private Transform player;
    //field of view distance 
    [SerializeField] private float fovDistance = 20f;
    //field of view angle
    [SerializeField] private float fovAngle = 45f;

    //search settings
    [SerializeField] private float searchAreaSize = 30f;
    [SerializeField] private float searchCellSize = 8f;

    //free roam settings
    [SerializeField] private float freeRoamDistance = 20f;
    [SerializeField] private float freeRoamWait = 6f;
    [SerializeField] private float freeRoamTimePassed = 0f;

    //turn visualisation on or off
    [SerializeField] private bool visualise = false;

    //reference to the navigation agent 
    UnityEngine.AI.NavMeshAgent agent;

    private void Start()
    {
        //cache a reference to our navigation agent
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private void Update()
    {
        //temporaray variable to see if the state has changed
        State tempState = currentState;

        if (ICanSeePlayer(player))
        {
            currentState = State.Flee;
        }
        else
        {
            if (currentState == State.Flee)
            {
                currentState = State.FreeRoam;
            }
        }

        switch (currentState)
        {
            case State.FreeRoam:
                //start free roam
                FreeRoam();
                break;
            case State.Flee:
                //start fleeing
                Flee();
                break;
            default:
                break;
        }

        if (tempState != currentState)
        {
            Debug.LogFormat("Civilian's state: {0}", currentState);
        }
    }

    private bool ICanSeePlayer(Transform player)
    {
        Vector3 direction = player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, direction, out hit))
        {
            //is this the player
            if (hit.collider.gameObject.tag == "player")
            {
                //is the player close enough
                if (direction.magnitude < fovDistance)
                {
                    //is the player within the angle
                    if (angle < fovAngle)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool CanPlayerSeePosition(Transform player, Vector3 position)
    {

        Vector3 direction = player.position - position;
        float angle = Vector3.Angle(direction, position);

        RaycastHit hit;

        if (Physics.Raycast(position, direction, out hit))
        {
            //is this the player
            if (hit.collider.gameObject.tag == "player")
            {
                //is the player close enough
                if (direction.magnitude < fovDistance)
                {
                    //is the player within the angle
                    if (angle < fovAngle)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void Flee()
    {
        //this.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        //this.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();

        agent.SetDestination(FindHidingSpot());
    }

    private void FreeRoam()
    {
        //accumulate the free roam time
        freeRoamTimePassed += Time.deltaTime;

        //if the civilian has free roamed in this area long enough
        if (freeRoamTimePassed > freeRoamWait)
        {
            //reset the free roam timer
            freeRoamTimePassed = 0;
            Vector3 freeRoamPoint = transform.position;

            //generate a random point on the xz axis at the patrol distance 
            freeRoamPoint += new Vector3(
                UnityEngine.Random.Range(-freeRoamDistance, freeRoamDistance),
                0,
                UnityEngine.Random.Range(-freeRoamDistance, freeRoamDistance)
                );

            //make the generated point a goal for the agent
            agent.SetDestination(freeRoamPoint);
        }
    }

    private Vector3 FindHidingSpot()
    {
        Vector3 hidingSpot = new Vector3();
        //set up a poisson disc sampler
        var distribution = new PoissonDiscSampler(searchAreaSize, searchAreaSize, searchCellSize);

        //assign the various hiding spots to a new list
        var candidateHidingSpots = new List<Vector3>();

        foreach (var point in distribution.Samples())
        {
            var searchPoint = point;

            //reposition the points so that the middle of the search
            //area is at the origin 0,0
            searchPoint.x -= searchAreaSize / 2f;
            searchPoint.y -= searchAreaSize / 2f;

            var searchPointLocalSpace = new Vector3(
                searchPoint.x,              //x position    
                transform.localPosition.y,  //y position - we only search on a flat plane
                searchPoint.y               //z position
                );

            //convert to world space
            var searchPointWorldSpace = transform.TransformPoint(searchPointLocalSpace);

            //find the nearest point on the navmesh
            UnityEngine.AI.NavMeshHit hit;

            bool foundPoint;

            foundPoint = UnityEngine.AI.NavMesh.SamplePosition(
                    searchPointWorldSpace,
                    out hit,
                    5,
                    UnityEngine.AI.NavMesh.AllAreas
            );

            if (foundPoint == false)
            {
                //we can't go here so continue
                continue;
            }

            var canSee = CanPlayerSeePosition(player, searchPointWorldSpace);

            if (canSee == false)
            {
                //we can't see the target from this position therefore return this ideal position
                candidateHidingSpots.Add(searchPointWorldSpace);
            }

            if (visualise)
            {
                //if it is a position the player will be able to see make it red
                //if it is a position the player cannot see make it a green
                Color debugColor = canSee ? Color.red : Color.green;

                Debug.DrawLine(transform.position, searchPointWorldSpace, debugColor, 0.1f);
            }
        }

        //if no hiding spots were found
        if (candidateHidingSpots.Count == 0)
        {
            //provide a dummy value
            hidingSpot = Vector3.zero;
            Debug.Log("Failed to find a hiding spot!");

            //indicate a failure 
            return Vector3.zero;
        }


        //for each of our candidate points calculate the length of the path needed to reach it

        //build a list of candidate points, matched with the length of the path needed to reach it
        List<KeyValuePair<Vector3, float>> paths;

        //for each point calculate the length
        paths = candidateHidingSpots.ConvertAll((Vector3 point) =>
        {
            //create a new path that reaches this point
            var path = new UnityEngine.AI.NavMeshPath();
            agent.CalculatePath(point, path);

            //store the distance of this path
            float distance;

            if (path.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
            {
                //if this path doesn't reach the target, consider it infinitely away
                distance = Mathf.Infinity;
            }
            else
            {
                //get up to 20 of the points in this path
                var corners = new Vector3[20];
                var cornerCount = path.GetCornersNonAlloc(corners);

                //start with the first point
                Vector3 current = corners[0];

                distance = 0;

                //figure out the cumulative distance for each point
                for (int i = 1; i < cornerCount; i++)
                {
                    //loop through adding each point to the respective path
                    var next = corners[i];
                    distance += Vector3.Distance(current, next);
                    current = next;
                }
            }

            //build the pair of key value pairs for the path and distance
            return new KeyValuePair<Vector3, float>(point, distance);
        });

        //sort this list based on distance so that shortest path is at the front of the list
        paths.Sort((a, b) =>
        {
            return a.Value.CompareTo(b.Value);
        });

        //return the point that's the shortest to reach
        hidingSpot = paths[0].Key;

        return hidingSpot;
    }
}
