using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements ability to be interacted with
/// </summary>
[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    //this is where you add functionality to obects after being interacted with
    public void Interact(GameObject fromObject)
    {
        Debug.LogFormat("I've been interacted with by {0}!", fromObject);

        if (gameObject.tag == "door")
        {
            Debug.Log("I am a door!");
        }
    }
}
