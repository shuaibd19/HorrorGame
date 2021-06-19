using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool owner;
}

//a class that adds a new method to the gameobject calss
public static class PooledGameObjectExtensions
{
    //this is an extention method that is added to all instances of the Gameobject class
    public static void ReturnToPool(this GameObject gameObject) 
    {
        var pooledObject = gameObject.GetComponent<PooledObject>();

        //checking if it exists
        if (pooledObject == null)
        {
            //if it does not exist it means that this obejct didn't come from a pool
            Debug.LogWarningFormat(
                      gameObject,
                      "Cannot return {0} to object pool because it weas not created from one",
                      gameObject);
            return;
        }
        //tell the pool we came from this object and should be returned
        pooledObject.owner.ReturnObject(gameObject);
    } 
}
