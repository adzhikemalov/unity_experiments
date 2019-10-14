using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector3 _oldVelocity;
    private Rigidbody _rigidbody;
    void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }
    
    void FixedUpdate () {
        _oldVelocity = _rigidbody.velocity;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!Collided)
        {
            Collided = true;
        
//            ContactPoint contact = collision.contacts[0];
//         
//            Vector3 reflectedVelocity = Vector3.Reflect(_oldVelocity, contact.normal);     
//            _rigidbody.velocity = reflectedVelocity;
//            Quaternion rotation = Quaternion.FromToRotation(_oldVelocity, reflectedVelocity);
//            transform.rotation = rotation * transform.rotation;
        }
    }

    public bool Collided { get; set; }
}
