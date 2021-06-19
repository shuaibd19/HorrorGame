using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<Menu> menus = new List<Menu>();

    private void Start()
    {
        //show the first menu on start
        ShowMenu(menus[0]);
    }

    public void ShowMenu(Menu menuToShow)
    {
        //ensure that this menu is the one we are tracking
        if (menus.Contains(menuToShow) == false)
        {
            Debug.LogErrorFormat("{0} is not in the list of menus!", menuToShow.name);
            return;
        }

        //enable this menu and disable the others
        foreach (var otherMenu in menus)
        {
            //is this the menu we want to display?
            if (otherMenu == menuToShow)
            {
                //mark it as active
                otherMenu.gameObject.SetActive(true);

                //tell the menu object to invoke its 'did appear' action
                otherMenu.menuDidAppear.Invoke();
            }
            else
            {
                //is this menu currently active
                if (otherMenu.gameObject.activeInHierarchy)
                {
                    //if so tell the meny object to invoke its will disappear action
                    otherMenu.menuWillDisappear.Invoke();
                }

                //and mark it as inactive
                otherMenu.gameObject.SetActive(false);
            }
        }
    }
}
