using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBoxScript : MonoBehaviour
{

    public GameObject breakedBox;

    public void Break()
    {
        if (!breakedBox) return;
        GameObject breaked = Instantiate(breakedBox, transform.position, transform.rotation);
        Rigidbody[] rbs = breaked.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in rbs)
        {
            rb.AddExplosionForce(150, transform.position, 30);
        }
        Destroy(gameObject);

        var gp = GameObject.Find("SceneSelector");
        if (gp)
        {
            var sc = gp.GetComponent<SceneSelector>();
            if (sc)
                sc.LoadNextScene();
        }
    }
}
