using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

//script used for controlling a nav mesh agent
[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentControlCopy : MonoBehaviour
{
    //radius of the noise
    [SerializeField] float noiseRadius = 20f;
    //reference to the camera used to shoot rays
    [SerializeField] private Camera cam;

    //bool to check the direction of the character
    [SerializeField] private bool facingRight = true;

    private SpriteRenderer rend;
    
    public GameObject doorObj;
    public GameObject itemObj;
    public GameObject UseObj;

    public GameObject ItemSlot1;    //itemslot positions in the inventory (an empty gameobject sits where they are)

    private GameObject prefabItem;   //an empty prefab to select them from the switch statement
    public GameObject CircleItem;   //make these images for the inventory screen buttons so you can click and use them
    private string storedUseItemRef;

    public TextMeshProUGUI InspectText;

    public Canvas UICanvas;
    bool itemlock;
    public bool itemUseBool;

    private void Start()
    {
        rend = gameObject.GetComponent<SpriteRenderer>();
        storedUseItemRef = "";
        itemlock = false;
        itemUseBool = false;
    }

    private void Update()
    {
        //move to mouse click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.collider.gameObject.tag == "door")                          //All clickable items need a collider on them and their tag appropriately set
                {
                    doorObj = hit.collider.gameObject;  //store the doorway you just clicked
                }
                if (hit.collider.gameObject.tag == "item")
                {
                    itemObj = hit.collider.gameObject;  //store the item you just clicked
                }
                if (hit.collider.gameObject.tag == "Inventory")
                {
                    this.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;   //Nullify movement if you click on the inventory
                }
                if (hit.collider.gameObject.tag == "Interactable")
                {
                    UseObj = hit.collider.gameObject;   //store the selected world object to use an item on
                }

                if (!itemUseBool)
                {
                    this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);

                    if (doorObj != null)
                    {
                        //If we need to move through a doorway to a new zone, use this

                        //if (Vector3.Distance(doorObj.transform.position, transform.position) < 1.2f)    //only do the action when close to door. Just walk to it until then
                        //{
                        //    //load scene through doorway
                        //}
                    }
                    if (itemObj != null)
                    {
                        if (itemlock == false)
                        {
                            if (Vector3.Distance(itemObj.transform.position, transform.position) < 1.2f)    //only do the action when close to door. Just walk to it until then
                            {
                                StartCoroutine(Pickup());
                                itemlock = true;
                            }
                        }
                    }
                }
                else
                {               //using an item movement
                    this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);

                    if (UseObj != null)
                    {
                        if (itemlock == false)
                        {
                            if (Vector3.Distance(UseObj.transform.position, transform.position) < 1.2f)    //only do the action when close to door. Just walk to it until then
                            {
                                switch (UseObj.name)   //switch statement to find the prefab of the item's namesake
                                {
                                    case "Green Interactible":
                                        if (storedUseItemRef == "Circle")
                                        {
                                            InspectText.text = "I can combine these two.";
                                            StartCoroutine(UseTextEnum());
                                            itemUseBool = false;
                                            itemlock = true;
                                            break;
                                        }
                                        else
                                        {
                                            InspectText.text = "That doesn't work with that.";
                                            StartCoroutine(UseTextEnum());
                                            itemUseBool = false;
                                            itemlock = true;
                                            break;
                                        }
                                }
                            }
                        }
                    }
                    else
                        itemUseBool = false;
                }

                

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

        //this next section is for getting the attention of the guard and commencing his investigation state

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

    IEnumerator Pickup()
    {
        //play pickup animation
        yield return new WaitForSeconds(0.5f);

        switch (itemObj.name)   //switch statement to find the prefab of the item's namesake
        {
            case "Circle":
                prefabItem = CircleItem;
                break;
        }

        //copy item to inventory
        var pickupItem = Instantiate(prefabItem, ItemSlot1.transform.position, Quaternion.identity);   //need code to determine free item slots
        pickupItem.transform.SetParent(UICanvas.transform, true);

        //destroy item from world
        Destroy(itemObj);
        itemObj = null;
        itemlock = false;
    }

    public void WhichUseItem(string s)
    {
        storedUseItemRef = s;
        itemUseBool = true;
    }

    IEnumerator UseTextEnum()
    {
        InspectText.enabled = true;

        yield return new WaitForSeconds(3.0f);

        InspectText.enabled = false;
    }
}
