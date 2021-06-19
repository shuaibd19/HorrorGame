using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPoolNotifier
{
    //called when the object is being returned to the pool
    void OnEnqueuedToPool();

    //called when the object is leaving the pool or just created
    //this allows for recycling of objects rather than instantiating constantly
    void OnCreateOrDequeuedFromPool(bool created);
}

public class ObjectPool : MonoBehaviour
{
    //the prefab to be instantiated
    [SerializeField]
    private GameObject prefab;

    //the queue of objects that are not currently in use
    private Queue<GameObject> inactiveObjects = new Queue<GameObject>();

    //Gets an object from the pool, if queue is empty creates one
    public GameObject GetObject()
    {
        //are there any inactive objects in the queue
        if (inactiveObjects.Count > 0)
        {
            //get an object from the queue
            var dequeuedObject = inactiveObjects.Dequeue();

            //queues objects are children of the pool, so we move them back to the root
            dequeuedObject.transform.parent = null;
            dequeuedObject.SetActive(true);

            //if there are any monobehaviours on this object that implement the pool notifier interface
            //inform them that this object has left the pool
            var notifiers = dequeuedObject.GetComponents<IObjectPoolNotifier>();

            foreach (var notifier in notifiers)
            {
                //notify the script that it left the pool
                notifier.OnCreateOrDequeuedFromPool(false);
            }

            //return the object for use 
            return dequeuedObject;
        }
        else
        {
            //there's nothing in the pool so instantiate a new object
            var newObject = Instantiate(prefab);

            //adding the pool tag so that it can return to the pool when done
            var poolTag = newObject.AddComponent<PooledObject>();
            poolTag.owner = this;

            //make the pool tag not visible in the inspector as it does not require configuration
            poolTag.hideFlags = HideFlags.HideInInspector;

            //notify the object that is was created
            var notifiers = newObject.GetComponents<IObjectPoolNotifier>();

            foreach (var notifier in notifiers)
            {
                //notify the script that it left the pool
                notifier.OnCreateOrDequeuedFromPool(true);
            }

            //return the object we created
            return newObject;
        }
    }

    //disables an object and return it to the queue for later use
    public void ReturnObject(GameObject gameObject)
    {
        //find the component we need to notify
        var notifiers = gameObject.GetComponents<IObjectPoolNotifier>();

        foreach (var notifier in notifiers)
        {
            //let if know that its returning to the pool
            notifier.OnEnqueuedToPool();
        }

        //diable the object and make it a child of this one, so that
        //it doesn't clutter up the heirarchy view
        gameObject.SetActive(false);
        gameObject.transform.parent = this.transform;

        //put the object into the ianctive queue
        inactiveObjects.Enqueue(gameObject);
    }
}
