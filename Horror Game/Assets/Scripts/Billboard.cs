using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    //reference to the camera
    [SerializeField] Camera mainCamera;

    //different way of rendering sprites
    [SerializeField] private bool useStaticBillboard = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var tempPosition = transform.position;
        var dist = Vector3.Distance(tempPosition, transform.position);
        if (!useStaticBillboard)
        {
            transform.LookAt(mainCamera.transform);
        }
        else
        {
            transform.rotation = mainCamera.transform.rotation;
        }

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
