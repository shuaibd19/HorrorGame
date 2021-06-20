using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class APlayerInteract : MonoBehaviour
{
    private Vector2 target;
    public GameObject doorObj;
    public GameObject itemObj;
    public GameObject UseObj;

    public GameObject ItemSlot1;    //itemslot positions in the inventory (an empty gameobject sits where they are)

    private GameObject prefabItem;   //an empty prefab to select them from the switch statement
    public GameObject CircleItem;   //make these images for the inventory screen buttons so you can click and use them
    private string storedUseItemRef = "";

    public TextMeshProUGUI InspectText;

    public Canvas UICanvas;
    bool itemlock = false;
    public bool itemUseBool = false;

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

    void Update()
    {
        //Getting the coordinates of the mouseposition in game
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Check if the user left clicks and get coords of the click
        if (Input.GetMouseButtonDown(0))
        {
            //detect what you hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
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
                    target = new Vector2(transform.position.x, transform.position.y);    //Nullify movement if you click on the inventory
                }
                if (hit.collider.gameObject.tag == "Interactable")
                {
                    UseObj = hit.collider.gameObject;   //store the selected world object to use an item on
                }
            }
        }
        //move player toward target coords
        if (!itemUseBool)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 5f);

            if (doorObj != null)
            {
                if (Vector2.Distance(doorObj.transform.position, transform.position) < 1.2f)    //only do the action when close to door. Just walk to it until then
                {
                    //load scene through doorway
                }
            }
            if (itemObj != null)
            {
                if (itemlock == false)
                {
                    if (Vector2.Distance(itemObj.transform.position, transform.position) < 1.2f)    //only do the action when close to door. Just walk to it until then
                    {
                        StartCoroutine(Pickup());
                        itemlock = true;
                    }
                }
            }
        }
        //else
        //{               //using an item movement
        //    transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 5f);

        //    if (UseObj != null)
        //    {
        //        if (itemlock == false)
        //        {
        //            if (Vector2.Distance(UseObj.transform.position, transform.position) < 1.2f)    //only do the action when close to door. Just walk to it until then
        //            {
        //                switch (UseObj.name)   //switch statement to find the prefab of the item's namesake
        //                {
        //                    case "Green Interactible":
        //                        if (storedUseItemRef == "Circle")
        //                        {
        //                            InspectText.text = "I can combine these two.";
        //                            StartCoroutine(UseTextEnum());
        //                            itemUseBool = false;
        //                            itemlock = true;
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            InspectText.text = "That doesn't work with that.";
        //                            StartCoroutine(UseTextEnum());
        //                            itemUseBool = false;
        //                            itemlock = true;
        //                            break;
        //                        }
        //                }
        //            }
        //        }
        //    }
        //    else
        //        itemUseBool = false;
        //}
    }
}
