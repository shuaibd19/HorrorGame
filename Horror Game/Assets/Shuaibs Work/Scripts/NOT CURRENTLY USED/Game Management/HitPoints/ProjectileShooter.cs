using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    //the amount of time to wait before creating another projectile
    [SerializeField] private float timeBetweenShots = 1f;

    //the speed that new projectiles should be moving at 
    [SerializeField] private float projectileSpeed = 10f;

    //reference to the object pool for more performant projectile system
    [SerializeField] private ObjectPool objectPool;

    private void Start()
    {
        //start creating projectiles
        StartCoroutine(ShootProjectiles());
    }

    //loop forever, creating a projectile every 'timeBetweenShots'
    IEnumerator ShootProjectiles()
    {
        //i guess this is where we can put input checks
        while (true)
        {
            ShootNewProjectile();

            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    //creates a new projectile and starts it moving
    void ShootNewProjectile()
    {
        //object pool demo
        var obj = objectPool.GetObject();

        //spawn the new object with the emitter's prosition and rotation
        obj.transform.position = gameObject.transform.position;
        obj.transform.rotation = gameObject.transform.rotation;

        //get the rigidbody on the new projectile
        var rBody = obj.GetComponent<Rigidbody>();

        if (rBody == null)
        {
            Debug.LogError("Projectile prefab has no rigidbody!");
            return;
        }

        //make if move away from the emitter's forward direction at projectile speed units per second
        rBody.velocity = transform.forward * projectileSpeed;

        //get both the projectiles collider and the emitters collider
        var col = obj.GetComponent<Collider>();
        var myCol = this.GetComponent<Collider>();

        //to avoid collision between shooter and own projectiles avoid the physics collisions
        if (col != null && myCol != null)
        {
            Physics.IgnoreCollision(col, myCol);
        }
    }
}
