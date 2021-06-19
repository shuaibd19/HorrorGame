using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles the input and display of a selection box
[RequireComponent(typeof(Rigidbody))]
public class BoxSelectable : MonoBehaviour
{
    private bool selected = false;
    private Vector3 pointPos;

    [SerializeField] private float movementSpeed = 4f;
    public void Selected()
    {
        Debug.LogFormat("{0} was selected", gameObject.name);
        selected = true;
    }

    private void Update()
    {
        if (selected)
        {
            //point and click functionality
            //did the user click the left mouse button?
            if (Input.GetMouseButtonDown(1))
            {
                //get the position onscreen in screen coordinates
                var mousePosition = Input.mousePosition;

                //convert this position into ray that start at the camera and moves toward
                //where the mouse cursor is
                var ray = Camera.main.ScreenPointToRay(mousePosition);

                //store the information about any raycast hit in this variable
                RaycastHit hit;

                //did the ray hit something?
                if (Physics.Raycast(ray, out hit))
                {
                    //figure out where the ray hit an object
                    pointPos = hit.point;
                }

                var rb = gameObject.GetComponent<Rigidbody>();

                var direction = transform.position - pointPos;

                rb.velocity = -direction.normalized * movementSpeed;
                selected = false;
            }
        }
    }
}
