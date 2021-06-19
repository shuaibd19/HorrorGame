using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generic class 'T' must be a monobehaviour class however
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //the instance - only has a getter and other parts of the code won't
    //be able to modify it
    public static T instance
    {
        get
        {
            //if we don't have an instance already get one by either
            //finding it in the scene or creating one
            if (_instance == null)
            {
                _instance = FindOrCreateInstance();
            }

            return _instance;
        }
    }

    //this variable actually stores the instance it's private and can 
    //only be accessed through the 'instance' property
    private static T _instance;

    //attempts to find an instance of this singleton 
    //if one can't be found a new one is created
    private static T FindOrCreateInstance()
    {
        //attempt to locate an instance
        var instance = GameObject.FindObjectOfType<T>();

        if (instance != null)
        {
            //we found one - return it 
            return instance;
        }

        //we'll create a new game object and attach this to it
        //naming the singleton
        var name = typeof(T).Name + " Singleton";

        //create the container game object 
        var containerGameObject = new GameObject(name);

        //create and attach a new instance of whatever 'T' is 
        var singletonComponent = containerGameObject.AddComponent<T>();

        return singletonComponent;
    }
}
