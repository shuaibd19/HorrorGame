using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventoryItemUse : MonoBehaviour
{
    public GameObject Player;
    private PlayerControl pc;
    public Button UseButton;
    public Button InspectButton;
    public GameObject panel;

    public TextMeshProUGUI InspectText;

    bool _active;

    void Start()
    {
        panel.SetActive(false);
        UseButton.enabled = false;
        InspectButton.enabled = false;
        _active = false;
        InspectText.enabled = false;
        pc = Player.GetComponent<PlayerControl>();
    }

    public void ClickOn()
    {
        panel.SetActive(true);
        UseButton.enabled = true;
        InspectButton.enabled = true;
        _active = true;
    }

    public void Use()
    {
        switch (gameObject.name)   //switch statement to find the item
        {
            case "CirclePrefab":
                pc.WhichUseItem("Circle");
                break;
        }
    }

    public void Inspect()
    {
        StartCoroutine(InspectTimer());
    }

    IEnumerator InspectTimer()
    {
        switch (gameObject.name)   //switch statement to find the item
        {
            case "CirclePrefab":
                InspectText.text = "It's a green testing circle.";
                break;
        }
        InspectText.enabled = true;
        panel.SetActive(false);
        UseButton.enabled = false;
        InspectButton.enabled = false;
        _active = false;

        yield return new WaitForSeconds(3.0f);

        InspectText.enabled = false;
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
