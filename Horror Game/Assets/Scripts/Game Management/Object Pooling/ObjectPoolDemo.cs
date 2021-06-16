using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//working example of object pool

public class ObjectPoolDemo : MonoBehaviour
{
    [Range(0.1f, 0.5f), SerializeField] float minTime;
    [Range(0.1f, 0.5f), SerializeField] float maxTime;
    [SerializeField] private ObjectPool pool;

    IEnumerator Start()
    {
        //get an object from the pool every 0.1-0.5 seconds
        while (true)
        {
            //get or create an object from the pool
            var o = pool.GetObject();

            //pick a point somewhere inside a sphere of radius 40
            //var position = Random.insideUnitSphere * 40;
            var x = Random.Range(20.0f, 145.0f);
            var y = Random.Range(5.0f, 15.0f);
            var z = Random.Range(0.0f, 50.0f);

            o.transform.localScale = Vector3.one * Random.Range(1.0f, 5.0f);

            var randPos = new Vector3(x, y, z);
            //place it 
            o.transform.position = randPos;

            //wait between 0.1 and 0.5 seconds and do it again
            var delay = Random.Range(minTime, maxTime);

            yield return new WaitForSeconds(delay);
        }
    }
}
