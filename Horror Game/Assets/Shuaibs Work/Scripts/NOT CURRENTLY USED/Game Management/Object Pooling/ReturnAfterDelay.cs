using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an example of a script that works with an object pool. This object waits for
//one second then returns itself to the pool
public class ReturnAfterDelay : MonoBehaviour, IObjectPoolNotifier
{
    public void OnCreateOrDequeuedFromPool(bool created)
    {
        //Debug.Log("Dequeued from object pool!");
        StartCoroutine(DoReturnAfterDelay());
    }

    public void OnEnqueuedToPool()
    {
        //Debug.Log("Enqueued to object pool!");
    }

    private IEnumerator DoReturnAfterDelay()
    {
        //wait for one second and then return to the pool
        yield return new WaitForSeconds(3.0f);

        gameObject.ReturnToPool();
    }
}
