using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetComponent : MonoBehaviour
{
    public PlanetManager manager;

    // Start is called before the first frame update
    protected void Awake()
    {
        manager = GetComponent<PlanetManager>();
    }
}
