using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    public Rigidbody2D NestedRigidbody;
    // Start is called before the first frame update
    private bool _alreadyHit;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_alreadyHit)
            return;
        
        if (NestedRigidbody)
        {
            if (other.CompareTag("Bullet"))
            {
                _alreadyHit = true;
                var rb = other.gameObject.GetComponent<Rigidbody2D>();
                NestedRigidbody.AddForceAtPosition(rb.velocity*25, other.gameObject.transform.position);
            }
        }
    }
}
