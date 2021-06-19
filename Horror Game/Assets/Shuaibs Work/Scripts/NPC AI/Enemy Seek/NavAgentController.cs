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

    private void Update()
    {
        //move to mouse click
        if (Input.GetMouseButtonDown(0)) 
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);
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
