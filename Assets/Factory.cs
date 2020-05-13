using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : PlanetComponent
{
    public Resource input;
    public int inputCount;
    public Resource output;
    public int outputCount;
    public float timeToCraft;

    public ResourceImport importer;
    public ResourceExport exporter;


    void Start()
    {
        exporter = manager.getExporter(output);
        importer = manager.getImporter(input);
    }

    private new void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        if(importer.amount >= inputCount)
        {
            importer.amount -= inputCount;
            exporter.amount += outputCount;
        }
    }
}
