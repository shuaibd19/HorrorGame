using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles the input and display of a selection box
public class BoxSelection : MonoBehaviour
{
    //dragable inspector reference to the image gameobject's recttransform
    public RectTransform selectionBox;

    //this variable will store the location of wherever we first click before draggin
    private Vector2 initialClickPosition = Vector2.zero;

    //the rectangle that box has dragged in screen space
    public Rect SelectionRect { get; private set; }

    //if true the user is actively dragging the box
    public bool isSelecting { get; private set; }

    private void Start()
    {
        //setting the anchors to be positioned at zero -zero means
        //that the box's size won't change as its parent changes size
        selectionBox.anchorMin = Vector2.zero;
        selectionBox.anchorMax = Vector2.zero;

        //setting the pivot point to zero means that box will pivot around
        //its lower-left corner
        selectionBox.pivot = Vector2.zero;

        //hid the box at the start
        selectionBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        //when we start draggin record the position of the mouse and start
        //showing the box
        if (Input.GetMouseButtonDown(0))
        {
            //get the initial click position of the mouse. No need to convert 
            //to gui space since we are using the lower left as the anchor and pivot
            initialClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //show the box
            selectionBox.gameObject.SetActive(true);
        }

        //while we are dragging update the position and size of the box based 
        //on the mouse position
        if (Input.GetMouseButton(0))
        {
            //store the current mouse position in screen space
            Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //figure out the lower-left corner and the upper right corner
            var xMin = Mathf.Min(currentMousePosition.x, initialClickPosition.x);
            var xMax = Mathf.Max(currentMousePosition.x, initialClickPosition.x);
            var yMin = Mathf.Min(currentMousePosition.y, initialClickPosition.y);
            var yMax = Mathf.Max(currentMousePosition.y, initialClickPosition.y);

            //build a rectangle from these corners
            var screenSpaceRect = Rect.MinMaxRect(xMin, yMin, xMax, yMax);

            //the anchor of the box has been configured to be its lower-left corner so by setting
            //its anchoredPosition we set its lower-left corner
            selectionBox.anchoredPosition = screenSpaceRect.position;

            //the size delta is hwo far the box entends from its anchor because the anchor's
            //minimum and maximum are the same point, changing its size delta directly changes
            //the final size
            selectionBox.sizeDelta = screenSpaceRect.size;

            //update our selection box
            SelectionRect = screenSpaceRect;
        }

        //when we release the mouse button hide the box and record that we're no longer selecting
        if (Input.GetMouseButtonUp(0))
        {
            SelectionComplete();

            //hide the box
            selectionBox.gameObject.SetActive(false);

            //we're no longer selecting
            isSelecting = false;
        }
    }

    //called when the user finishes dragging a selection box
    private void SelectionComplete()
    {
        Camera mainCamera = GetComponent<Camera>();

        //get the bottom-left and top-right corners of the screen-space
        //selection view and covert them to viewport space
        var min = mainCamera.ScreenToViewportPoint(new Vector3(SelectionRect.xMin, SelectionRect.yMin));
        var max = mainCamera.ScreenToViewportPoint(new Vector3(SelectionRect.xMax, SelectionRect.yMax));

        //we want to create a bounding box in viewport space
        //we have the x and y coordinates of the bottom -left and top -right
        //corners - now we'll include the z coordinates
        min.z = mainCamera.nearClipPlane;
        max.z = mainCamera.farClipPlane;

        //construct a bounding box
        var viewportBounds = new Bounds();
        viewportBounds.SetMinMax(min, max);

        //check each object that has a selectable component
        foreach (var selectable in FindObjectsOfType<BoxSelectable>())
        {
            //figure out where this object is in viewport space
            var selectedPosition = selectable.transform.position;

            var viewportPoint = mainCamera.WorldToViewportPoint(selectedPosition);

            //is that point within our viewport bounding box? if it is they're selected
            var selected = viewportBounds.Contains(viewportPoint);

            if (selected && selectable.gameObject.tag == "moveAble")
            {
                //let them know
                selectable.Selected();
            }
        }
    }
}
