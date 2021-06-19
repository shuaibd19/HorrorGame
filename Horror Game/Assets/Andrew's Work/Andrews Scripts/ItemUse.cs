using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemUse : MonoBehaviour
{
    public Button UseButton;
    public Button InspectButton;
    public GameObject panel;

    void Start()
    {
        panel.SetActive(false);
        UseButton.enabled = false;
        InspectButton.enabled = false;
    }
    
    void Update()
    {
        if (Input.GetMouseButton(0) && panel.activeSelf &&
             !RectTransformUtility.RectangleContainsScreenPoint(
                 panel.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main))
        {
            panel.SetActive(false);
        }
    }
}
