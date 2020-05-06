using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{

    public TextMeshPro resourceText;

    public TextMeshPro planetText;


    public List<GameObject> ships = new List<GameObject>();

    public bool launching = true;

    public List<ResourceImport> imports = new List<ResourceImport>();
    public List<ResourceExport> exports = new List<ResourceExport>();
    public List<Factory> factories = new List<Factory>();
    public List<Producer> producers = new List<Producer>();

    // Start is called before the first frame update
    void Awake()
    {
        foreach(ResourceImport imp in GetComponents<ResourceImport>())
        {
            imports.Add(imp);
        }
        foreach (ResourceExport exp in GetComponents<ResourceExport>())
        {
            exports.Add(exp);
        }
        foreach (Factory fac in GetComponents<Factory>())
        {
            factories.Add(fac);
        }
        foreach (Producer pro in GetComponents<Producer>())
        {
            producers.Add(pro);
        }

    }

    // Update is called once per frame
    void Update()
    {
        resourceText.text = "";
        for (int i = 0; i < exports.Count; i++)
        {
            resourceText.text += exports[i].resource.name + ": " + exports[i].amount;
        }
        
    }

    public IEnumerator Enter(GameObject ship)
    {
        bool arrived = false;
        while (!arrived)
        {
            ship.transform.localScale = ship.transform.localScale * 0.97f;
            Debug.Log("scale: " + ship.transform.localScale.magnitude);
            if (ship.transform.localScale.magnitude < 0.05f)
            {
                arrived = true;
                ships.Add(ship);
                ship.gameObject.SetActive(false);
                ship.GetComponent<ShipController>().arrivedAtTarget = false;
                ship.GetComponent<ShipController>().target = null;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;

    }

    public void EnterPlanet(GameObject ship)
    {
        StartCoroutine(Enter(ship));
    }



    public IEnumerator LaunchLoop()
    {
        while (launching)
        {
            yield return new WaitForSeconds(2);
            Debug.Log("Trying to launch");
            if (ships.Count > 0)
            {
                //launch the oldest ship
                Debug.Log("Lauching " + ships[0].name);
                ships[0].GetComponent<ShipController>().Launch();
                ships[0].gameObject.SetActive(true);
                StartCoroutine(Exit(ships[0]));
            }

        }
    }

    public IEnumerator Exit(GameObject ship)
    {

        bool launched = false;
        while (!launched)
        {
            ship.transform.localScale = ship.transform.localScale * 1.1f;
            Debug.Log("Exit scale: " + ship.transform.localScale.magnitude);
            if (ship.transform.localScale.x >= 1f)
            {
                ship.transform.localScale = new Vector3(1, 1, 1);
                launched = true;
                ships.Remove(ship);


            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;

    }

    void OnMouseOver()
    {
        resourceText.gameObject.SetActive(true);
        planetText.gameObject.SetActive(true);
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        resourceText.gameObject.SetActive(false);
        planetText.gameObject.SetActive(false);
    }

    public ResourceExport getExporter(Resource resource)
    {
        for(int i = 0; i < exports.Count; i++)
        {
            if(exports[i].resource.name == resource.name)
            {
                return exports[i];
            }
        }
        Debug.LogError("There is no exporter for resource " + resource.name);
        return null;
    }
}
