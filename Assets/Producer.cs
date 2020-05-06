using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producer : PlanetComponent
{
    public Resource resource;

    public float resourceTimer;

    public float resourceSpeed = 1f;

    public ResourceExport exporter;


    // Start is called before the first frame update
    void Start()
    {
        exporter = manager.getExporter(resource);
    }

    private new void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        resourceTimer += Time.deltaTime * resourceSpeed;

        if (Mathf.Abs(resourceTimer) >= 1)
        {
            resourceTimer -= Mathf.Sign(resourceTimer);
            exporter.amount += (int)Mathf.Sign(resourceSpeed);

            if (exporter.amount < 0)
            {
                exporter.amount = 0;
            }
        }
    }
}
