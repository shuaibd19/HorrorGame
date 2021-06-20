using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    public float walkforce = 0f;
    public float runMoveForce = 0f;
    public float moveforce = 0f;
    public LayerMask whatisPlayer;
    public LayerMask WhatIsObstacles;
    private Rigidbody RB;
    public float Checkwall = 0f;
    private Vector3 moveDir;
    private bool targetfound;

    public float shootrate = 0f;
    private float shootTimeStamp = 0f;



    public GameObject playerObject;


    public float DistanceFromTarget = 0f;
    public float SafeDistance = 0f;

    public float runTurnRate = 0f;
    private float runTurnTimeStamp = 0f;
    public float RunTurnDisCheck = 0f;
    private bool shootFire = false;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        moveDir = ChooseDirection();
        transform.rotation = Quaternion.LookRotation(moveDir);
        moveforce = walkforce;
        runTurnRate = Random.Range(0.5f, 1.5f);

        // distnce between the player and the enemy
        DistanceFromTarget = Vector3.Distance(transform.position, playerObject.transform.position);
        
    }
    void attack()
    {
        if (!shootFire)
        {
            if (Time.time > shootTimeStamp)
            {
                playerObject.GetComponent<playermovement>();
                moveforce = runMoveForce;
                shootFire = true;
                moveDir = ChooseDirection();
                transform.rotation = Quaternion.LookRotation(moveDir);
                shootTimeStamp = Time.time + shootrate;
            }
            
        }
        else
        {
            Hide();
        }
    }

    void Hide()
    {
        if (DistanceFromTarget < SafeDistance)
        {
            RunToHide();
        }
        else
        {
            moveforce = walkforce;
            shootFire = false;
            targetfound = false;
            moveDir = ChooseDirection();
            transform.rotation = Quaternion.LookRotation(moveDir);

        }
            
    }
    void RunToHide()
    {
        RB.velocity = moveDir * moveforce;
        if ( Time.time > runTurnTimeStamp)
        {
            if (Physics.Raycast(transform.position , transform.right , RunTurnDisCheck , WhatIsObstacles))
            {
                moveDir = -transform.right;
                transform.rotation = Quaternion.LookRotation(moveDir);

            }
            else if (Physics.Raycast(transform.position,-transform.right, RunTurnDisCheck, WhatIsObstacles))
            {
                moveDir = transform.right;
                transform.rotation = Quaternion.LookRotation(moveDir);

            }
            else
            {
                moveDir = ChooseDirection();
                transform.rotation = Quaternion.LookRotation(moveDir);
            }

            runTurnRate = Random.Range(0.5f, 1.5f);
            runTurnTimeStamp = Time.time + runTurnRate;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (targetfound)
        {
            attack();
          
           
        }
        else
        {
            lookforTraget();
        }

    }
    // choose the random direction for the AI enemy 
    Vector3 ChooseDirection()
    {
        System.Random ran = new System.Random();
        int i = ran.Next(0, 1);

        Vector3 temp = new Vector3();

        if (i ==0)
        {
            temp = transform.right;

        }
        else if(i == 1)
        {
            temp = -transform.right;
        }
        

        return temp;
    }
    // AI enemy llok for the taget ( Player )
    void lookforTraget()
    {
        Move();
        targetfound = Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, whatisPlayer);

    }
    void Move()
    {
        RB.velocity = moveDir * moveforce;
        if (Physics.Raycast(transform.position, transform.forward, Checkwall, WhatIsObstacles))
        {
            moveDir = ChooseDirection();
            transform.rotation = Quaternion.LookRotation(moveDir);

        }
    }

}
