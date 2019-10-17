using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GunController : MonoBehaviour
{
    public GameObject StartPoint;
    public GameObject EndPoint;

    public GameObject Sight;
    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookAt = mouseScreenPosition;
        float AngleRad = Mathf.Atan2(lookAt.y - transform.position.y, lookAt.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, AngleDeg);

        Sight.gameObject.SetActive(Input.GetMouseButton((int) MouseButton.LeftMouse));
    }

    public Vector2 GetForce()
    {
        var vector3 = EndPoint.transform.position - StartPoint.transform.position;
        return new Vector2(vector3.x, vector3.y);
    }
}
