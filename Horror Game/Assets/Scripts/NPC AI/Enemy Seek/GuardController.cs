using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script used to control behaviour of guard nav mesh agents
public class GuardController : MonoBehaviour
{
    //FSM states for a guard
    private enum State
    {
        Patrol,
        Investigate,
        Chase
    }

    //current state set to patrol at start
    private State currentState = State.Patrol;

    //reference to the player we are looking for
    [SerializeField] private Transform player;
    //field of view distance
    [SerializeField] private float fovDistance = 20f;
    //field of view arc angle
    [SerializeField] private float fovAngle = 45f;

    //last place the player was seen used in the investigate state
    Vector3 lastPlaceSeen;

    //chase settings
    [SerializeField] private float chasingSpeed = 2f;
    [SerializeField] private float chastingRotSpeed = 2f;
    [SerializeField] private float chasingAccuracy = 5f;

    //patrol settings
    [SerializeField] private float patrolDistance = 10f;
    [SerializeField] private float patrolWait = 5f;
    [SerializeField] private float patrolTimePassed = 0;

    private void Start()
    {
        patrolTimePassed = patrolWait;
        lastPlaceSeen = this.transform.position;
    }

    private void Update()
    {
        //temporary variable to see if the state has changed
        State tempState = currentState;

        if (ICanSeePlayer(player))
        {
            currentState = State.Chase;
            lastPlaceSeen = player.position;
        }
        else
        {
            //i can no longer see the player so go investigate
            if (currentState == State.Chase)
            {
                currentState = State.Investigate;
            }
        }

        //state checking 
        switch (currentState)
        {
            case State.Patrol:
                //start Patrolling
                Patrol();
                break;
            case State.Investigate:
                //start investigating
                Investigate();
                break;
            case State.Chase:
                //chase the player
                Chase();
                break;
            default:
                break;
        }

        if (tempState != currentState)
        {
            Debug.LogFormat("Guard's state: {0}", currentState);
        }
    }

    private bool ICanSeePlayer(Transform player)
    {
        Vector3 direction = player.position - this.transform.position;

        float angle = Vector3.Angle(direction, this.transform.forward);

        RaycastHit hit;
        //if i hit something
        if (Physics.Raycast(this.transform.position, direction, out hit))
        {
            //is that thing the player
            if (hit.collider.gameObject.tag == "player")
            {
                //is the player close enough
                if (direction.magnitude < fovDistance)
                {
                    //is the player in my view space
                    if (angle < fovAngle)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void Chase()
    {
        //visual cue for chase state
        var rend = gameObject.GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.red);

        //this implementation is a bit dumb because it does not take into account
        //the nav mesh dafuq
        this.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        this.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();

        Vector3 direction = player.position - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(
            this.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * this.chastingRotSpeed
            );

        if (direction.magnitude > this.chasingAccuracy)
        {
            this.transform.Translate(0, 0, Time.deltaTime * this.chasingSpeed);
        }
    }

    private void Investigate()
    {
        //CrowdBehaviour.instance.setPlayerSpotted(true, lastPlaceSeen);

        //visual cue for investigate state
        var rend = gameObject.GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.yellow);

        //if the agent arrived at the investigating goal they should patrol there
        if (Vector3.Magnitude(transform.position - lastPlaceSeen) <= 0.5f)
        {
            currentState = State.Patrol;
        }
        else
        {
            this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(lastPlaceSeen);
        }
    }

    private void Patrol()
    {
        //CrowdBehaviour.instance.setPlayerSpotted(false);

        //visual cue for patrol state
        var rend = gameObject.GetComponent<Renderer>();
        rend.material.SetColor("_Color", Color.green);

        //accumulate the patrol time
        patrolTimePassed += Time.deltaTime;

        //if the guard has patrolled long enough
        if (patrolTimePassed > patrolWait)
        {
            //reset the patrol time 
            patrolTimePassed = 0;
            Vector3 patrollingPoint = lastPlaceSeen;

            //generate a random point on the xz axis at the patrol distance 
            //from the lasplaceseen position
            patrollingPoint += new Vector3(
                UnityEngine.Random.Range(-patrolDistance, patrolDistance),
                0,
                UnityEngine.Random.Range(-patrolDistance, patrolDistance)
                );

            //make a generated point a goal for the agent
            this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(patrollingPoint);
        }
    }

    public void InvestigatePoint(Vector3 point)
    {
        lastPlaceSeen = point;
        currentState = State.Investigate;
    }
}
