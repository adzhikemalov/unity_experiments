using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    public Target Target;
    public Rigidbody2D NestedRigidbody;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Target || !Target.IsHitable)
            return;
        
        if (NestedRigidbody)
        {
            if (other.CompareTag("Bullet"))
            {
                Target.IsHitable = false;
                var rb = other.gameObject.GetComponent<Rigidbody2D>();
                NestedRigidbody.AddForceAtPosition(rb.velocity*25, other.gameObject.transform.position);
            }
        }
    }
}
