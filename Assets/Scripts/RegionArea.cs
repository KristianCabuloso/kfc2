using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionArea : MonoBehaviour
{
    public string regionName;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.isTrigger = true;
    }
}
