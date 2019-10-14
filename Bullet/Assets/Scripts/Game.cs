using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject BulletPrefab;
    public GunController Gun;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var bullet = Instantiate(BulletPrefab, Gun.EndPoint.transform.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.SetInitialForce(Gun.GetForce());
        }
    }
}
