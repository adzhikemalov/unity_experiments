using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    public int speed = 15;
    private Rigidbody2D _rigidbody2D;

    public void SetInitialForce(Vector2 force)
    {
        if (_rigidbody2D == null)
            _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = force.normalized * speed;
    }
    
    private Vector2 _velocity;
    private Vector2 _lastPos;
    void FixedUpdate ()
    {
        Vector3 pos3D = transform.position;
        Vector2 pos2D = new Vector2(pos3D.x, pos3D.y);
        
        _velocity = pos2D - _lastPos;
        _lastPos = pos2D;
    }
 
    private void OnCollisionEnter2D(Collision2D col)
    {
        Vector3 N = col.contacts[0].normal;
        Vector3 V = _velocity.normalized;
 
        Vector3 R = Vector3.Reflect(V, N).normalized;
        _rigidbody2D.velocity = new Vector2(R.x, R.y).normalized * speed;
    }
}
