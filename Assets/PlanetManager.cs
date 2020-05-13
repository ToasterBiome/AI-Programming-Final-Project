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
        foreach (ResourceImport imp in GetComponents<ResourceImport>())
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

    private void Start()
    {
        StartCoroutine("LaunchLoop");
    }

    // Update is called once per frame
    void Update()
    {
        resourceText.text = "";
        for (int i = 0; i < imports.Count; i++)
        {
            resourceText.text += "I: " + imports[i].resource.name + ": " + imports[i].amount;
            resourceText.text += "\n";
        }
        for (int i = 0; i < exports.Count; i++)
        {
            resourceText.text += "E: " + exports[i].resource.name + ": " + exports[i].amount;
        }

    }

    public IEnumerator Enter(GameObject ship)
    {
        bool arrived = false;
        while (!arrived)
        {
            ship.transform.localScale = ship.transform.localScale * (float)(1 - 0.03 * ship.GetComponent<Ship>().speed);
            Debug.Log("scale: " + ship.transform.localScale.magnitude);
            if (ship.transform.localScale.magnitude < 0.2f)
            {
                arrived = true;
                ships.Add(ship);
                ship.gameObject.SetActive(false);
                //ship.GetComponent<Ship>().arrivedAtTarget = false;
                //ship.GetComponent<Ship>().target = null;
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
                foreach (GameObject ship in ships)
                {


                    bool loadSuccessful = ship.GetComponent<Ship>().Load();
                    if (loadSuccessful)
                    {

                        //share a random planet they know with everyone
                        foreach (GameObject otherShip in ships)
                        {
                            if (ship != otherShip) //not yourself
                            {
                                //give it a random planet to know of
                                GameObject randomPlanet = ship.GetComponent<Ship>().knownPlanets[Random.Range(0, ship.GetComponent<Ship>().knownPlanets.Count - 1)];

                                if (!otherShip.GetComponent<Ship>().knownPlanets.Contains(randomPlanet))
                                { //if it doesn't know it
                                    otherShip.GetComponent<Ship>().knownPlanets.Add(randomPlanet); //add it
                                }
                            }

                        }

                        ship.GetComponent<Ship>().Launch();
                        ship.gameObject.SetActive(true);
                        StartCoroutine(Exit(ship));
                    }
                    else
                    {
                        //weren't enough resources, wait
                    }
                }

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
        for (int i = 0; i < exports.Count; i++)
        {
            if (exports[i].resource.name == resource.name)
            {
                return exports[i];
            }
        }
        //Debug.LogError("There is no exporter for resource " + resource.name);
        return null;
    }

    public ResourceImport getImporter(Resource resource)
    {
        for (int i = 0; i < imports.Count; i++)
        {
            if (imports[i].resource.name == resource.name)
            {
                return imports[i];
            }
        }
        //Debug.LogError("There is no importer for resource " + resource.name);
        return null;
    }
}
