using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryItemUse : MonoBehaviour
{
    public Button UseButton;
    public Button InspectButton;
    public GameObject panel;

    bool _active;

    void Start()
    {
        panel.SetActive(false);
        UseButton.enabled = false;
        InspectButton.enabled = false;
        _active = false;
    }

    public void ClickOn()
    {
        panel.SetActive(true);
        UseButton.enabled = true;
        InspectButton.enabled = true;
        _active = true;
    }

    void Update()
    {
        if (_active)
        {
            if (Input.GetMouseButton(0) && panel.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
            {
                panel.SetActive(false);
                _active = false;
            }
        }
    }
}
