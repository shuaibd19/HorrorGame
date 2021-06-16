using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    private void OnCollisionEnter(Collision collision)
    {
        //does the collided object have a damage reciver script?

        var otherDamageReciever = collision.gameObject.GetComponent<DamageReciever>();

        if (otherDamageReciever != null)
        {
            otherDamageReciever.TakeDamage(damageAmount);
        }

        //destroy the projectile
        Destroy(gameObject);
    }
}
