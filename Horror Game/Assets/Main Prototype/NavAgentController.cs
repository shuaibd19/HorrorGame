using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//script used for controlling a nav mesh agent
[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentController : MonoBehaviour
{
    //radius of the noise
    [SerializeField] float noiseRadius = 20f;
    //reference to the camera used to shoot rays
    [SerializeField] private Camera cam;

    //bool to check the direction of the character
    [SerializeField] private bool facingRight = true;

    private SpriteRenderer rend;

    private void Start()
    {
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //move to mouse click
        if (Input.GetMouseButtonDown(0)) 
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);

                //checking for flipping the character sprite
                //save the difference in vectors between the mouse click position and the players position
                var delta = hit.point - transform.position;

                //Debug.Log(delta.x);

                //if the mouse points at right side of the player
                if (delta.x >= 0 && !facingRight)
                {
                    //transform.localScale = new Vector3(1, 1, 1); //activate looking right
                    facingRight = true;
                    rend.flipX = true;
                }
                else if (delta.x < 0 && facingRight)
                {
                    //mouse points to the left side of the player
                    //transform.localScale = new Vector3(-1, 1, 1);
                    facingRight = false;
                    rend.flipX = false;
                }

            }
        }

        //play noise on space bar
        if (Input.GetKey("space"))
        {
            //play the audio file
            StartCoroutine(PlayNoise());

            //create a sphere collider
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, noiseRadius);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                //check for collisions
                //if it's a guard trigger the investigation
                if (hitColliders[i].tag == "guard")
                {
                    //tell the guard to investigate this point
                    hitColliders[i].GetComponent<GuardController>().InvestigatePoint(this.transform.position);
                }
            }
        }

    }

    IEnumerator PlayNoise()
    {
        AudioSource audio = GetComponent<AudioSource>();

        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
    }
}
