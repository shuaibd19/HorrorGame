using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSingleton : Singleton<ExampleSingleton>
{
    //you can use call this function throughout the entire project by
    //using the following:
    //ExampleSingleton.instance.SingletonExample();

    //singletons only have a single instance
    //examples of things you could make a singleton inclue
    //gameplay managers, input managers or other utilities
    public void SingletonExample()
    {
        Debug.Log("You can acess me from the entire game!");
    }
}
