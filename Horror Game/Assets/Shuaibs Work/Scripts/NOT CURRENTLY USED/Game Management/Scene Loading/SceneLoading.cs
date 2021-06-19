using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//used for loading scenes
public class SceneLoading : Singleton<SceneLoading>
{
    public bool menuOpen = false;

    //used to set the menuOpen bool to true or false
    public void setMenu()
    {
        menuOpen = !menuOpen;
    }

    //this should be used to load entirely new scenes / levels of a game
    public void LoadSceneAsync(string sceneName)
    {
        //start loading the scene - we'll get back an object that represents the scene loading operation
        var operation = SceneManager.LoadSceneAsync(sceneName);

        Debug.Log("Starting load...");

        //don't proceed to the scene once the loading has finished
        operation.allowSceneActivation = false;

        //start a coroutine that will run while the scene loads and will
        //run code after loading finishes
        StartCoroutine(WaitForLoading(operation));
    }

    IEnumerator WaitForLoading(AsyncOperation operation)
    {
        //wait for the scene load to reach at least 90%
        while(operation.progress < 0.9f)
        {
            yield return null;
        }

        //we're done

        Debug.Log("Loading complete!");

        //enable scene activation
        operation.allowSceneActivation = true;
    }

    //to load scenes additively on top of each other use this method
    public void LoadSceneAdditive(string sceneName)
    {
        //load the scene in addition to the current one
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    //to unload  scene asynchronously
    public void unloadSceneAdditive(string sceneName)
    {
        var unloadOperation = SceneManager.UnloadSceneAsync(sceneName);

        StartCoroutine(WaitForUnloading(unloadOperation));
    }

    IEnumerator WaitForUnloading(AsyncOperation operation)
    {
        yield return new WaitUntil(() => operation.isDone);

        //free up memory of textures etc
        Resources.UnloadUnusedAssets();
    }
}
