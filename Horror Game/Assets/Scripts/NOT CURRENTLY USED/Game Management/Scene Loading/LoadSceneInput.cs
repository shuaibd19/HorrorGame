using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadSceneInput : MonoBehaviour
{
    private void Update()
    {
        //if the user presses the m button on the keyboard
        if (Input.GetKeyDown(KeyCode.M))
        {
            //pause the game
            Time.timeScale = 0;
            //disable the event system
            var es = FindObjectOfType<EventSystem>();
            if (es != null)
            {
                es.enabled = false;
            }
            //dsiable the canvas 
            var canv = FindObjectOfType<Canvas>();
            if (canv != null)
            {
                canv.enabled = false;

            }
            //if the menu is currently false
            if (!SceneLoading.instance.menuOpen)
            {
                //load the menu scene
                SceneLoading.instance.LoadSceneAdditive("Menu");
                //set the menu to being true
                SceneLoading.instance.setMenu();

            }
        }

        //if the user presses the escape button on the keyboard or the m button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if the menu is open
            if (SceneLoading.instance.menuOpen)
            {
                //unload the menu scene
                SceneLoading.instance.unloadSceneAdditive("Menu");
                //set the menu to bein false again
                SceneLoading.instance.setMenu();

                //resume time
                Time.timeScale = 1;

                //re-enable the event system
                var es = FindObjectOfType<EventSystem>();
                if (es != null)
                {
                    es.enabled = true;
                }
                //re-enable the canvas
                var canv = FindObjectOfType<Canvas>();
                if (canv != null)
                {
                    canv.enabled = true;
                }
            }
        }
    }
}
