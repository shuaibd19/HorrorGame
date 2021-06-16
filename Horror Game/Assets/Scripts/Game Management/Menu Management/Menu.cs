using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
    //invoked when a menu appears on onscreen
    public UnityEvent menuDidAppear = new UnityEvent();

    //invoked when a menu is removed from the screen
    public UnityEvent menuWillDisappear = new UnityEvent();
}
