using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemInspect : MonoBehaviour
{
    public TextMeshProUGUI InspectText;
    
    void Start()
    {
        InspectText.gameObject.SetActive(false);
    }
    
    
    void Update()
    {
        
    }
}
