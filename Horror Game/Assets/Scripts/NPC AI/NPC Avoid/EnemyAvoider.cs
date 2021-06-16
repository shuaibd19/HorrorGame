using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//detects if the target can see us, and if it can, navigates to 
//somewhere they can't
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAvoider : MonoBehaviour
{
    //the object that is looking for us
    [SerializeField] EnemyVisibility visibility = null;

    //the size of the area where we're considering to hide
    [SerializeField] float searchAreaSize = 10f;

    //the density of the search field.. large numbers is more efficient 
    //but less hiding places
    [SerializeField] float searchCellSize = 1f;

    //if true draw lines to places considered to hide
    [SerializeField] bool visualise = true;

    //the navigation agent, which will navigate to the best hiding place
    NavMeshAgent agent;

    //the start method is a coroutine; when the game starts it will start
    //a continuous cycle of avoiding the target
    IEnumerator Start()
    {
        //cache a reference to our navigation agent
        agent = GetComponent<NavMeshAgent>();

        //do this forever
        while(true)
        {
            //can the target see us?
            if(visibility.targetIsVisible)
            {
                //find a place to run where it can't see us anymore
                Vector3 hidingSpot;

                if(FindHidingSpots(out hidingSpot) == false)
                {
                    //we didn't find anywhere to hide so wait a second and try again
                    yield return new WaitForSeconds(1.0f);
                    continue;
                }

                //tell the agent to start moving to this locatiojn
                agent.destination = hidingSpot;
            }

            //wait a bit, and then check to see if the target can still see us
            yield return new WaitForSeconds(0.1f);
        }
    }


    //attemps to find a nearby place where the target can't see us.
    //returns true if one was found; if it was, put the position in the 
    //hidingSpot variable
    bool FindHidingSpots(out Vector3 hidingSpot)
    {
        //setting up a poisson disc sampler
        var distribution = new PoissonDiscSampler(searchAreaSize, searchAreaSize, searchCellSize);

        var candidateHidingSpots = new List<Vector3>();

        foreach(var point in distribution.Samples())
        {
            var searchPoint = point;

            //reposition the points so that the middle of the search
            //area is at the origin - 0,0
            searchPoint.x -= searchAreaSize / 2f;
            searchPoint.y -= searchAreaSize / 2f;

            var searchPointLocalSpace = new Vector3(
                searchPoint.x,
                transform.localPosition.y,
                searchPoint.y
                );

            //can they see us from here
            var searchPointWorldSpace = transform.TransformPoint(searchPointLocalSpace);

            //find the nearest point on the navmesh
            NavMeshHit hit;

            bool foundPoint;
            
            foundPoint = NavMesh.SamplePosition(
                    searchPointWorldSpace,
                    out hit,
                    5,
                    NavMesh.AllAreas
                    );

            if(foundPoint == false)
            {
                //we can't get here so disregard this place
                continue;
            }

            searchPointWorldSpace = hit.position;

            var canSee = visibility.CheckVisibilityToPoint(searchPointWorldSpace);

            if(canSee == false)
            {
                //we can;t see the target from this position
                //therefore return this ideal position
                candidateHidingSpots.Add(searchPointWorldSpace);
            }

            if(visualise)
            {
                Color debugColor = canSee ? Color.red : Color.green;

                Debug.DrawLine(
                    transform.position,
                    searchPointWorldSpace,
                    debugColor,
                    0.1f
                    );
            }
        }

        if(candidateHidingSpots.Count == 0)
        {
            //we can't find any hiding spots

            //provide a dummy value
            hidingSpot = Vector3.zero;
            Debug.Log("Failed to locate any hiding spots!");
            //indicate our failure
            return false;

        }

        //for each of our candidate points, calculate the length of the path needed to reach it

        //build a list of candidate points, matched with the length of the path needed
        //to reach it
        List<KeyValuePair<Vector3, float>> paths;

        //for each point , calculate the length
        paths = candidateHidingSpots.ConvertAll((Vector3 point) =>
        {
            //create a new path that reaches this point
            var path = new NavMeshPath();
            agent.CalculatePath(point, path);

            //store the distance needed for this path
            float distance;

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                //if this path doesn't reach the target, consider it infinitely far away
                distance = Mathf.Infinity;
            }
            else
            {
                //get up to 32 of the points on this path
                var corners = new Vector3[32];
                var cornerCount = path.GetCornersNonAlloc(corners);

                //start with the first point
                Vector3 current = corners[0];

                distance = 0;

                //figure out the cumulative distance for each point
                for(int c = 1; c < cornerCount; c++)
                {
                    var next = corners[c];
                    distance += Vector3.Distance(current, next);
                    current = next;
                }
            }

            //build the pair of point and distance
            return new KeyValuePair<Vector3, float>(point, distance);
        });

        //sort this list based on distance, so that the shortest path is 
        //at the front of the list

        paths.Sort((a, b) =>
        {
            return a.Value.CompareTo(b.Value);
        });

        //return the point that's the shortest to reach
        hidingSpot = paths[0].Key;
        return true;
    }
}
