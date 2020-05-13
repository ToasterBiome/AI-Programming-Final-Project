using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public List<GameObject> knownPlanets = new List<GameObject>();

    public TextMeshPro text;


    public Tank tank;

    public GameObject target;

    public GameObject[] planets;

    public TemperatureSensor tempSensor;

    public float speed = 1f;

    public bool avoidingDanger = false;

    public bool arrivedAtTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        tempSensor = GetComponent<TemperatureSensor>();


        int knownImporters = 0;
        int knownExporters = 0;
        foreach (GameObject planetGO in planets)
        {
            PlanetManager manager = planetGO.GetComponent<PlanetManager>();


            if (manager.getExporter(tank.resource))
            {
                if (knownExporters < 1)
                {
                    knownPlanets.Add(planetGO);
                }
                else
                {
                    if (Random.Range(0, 1) >= 0.5f)
                    {
                        knownPlanets.Add(planetGO);
                    }
                }
            }

            if (manager.getImporter(tank.resource))
            {
                if (knownImporters < 1)
                {
                    knownPlanets.Add(planetGO);
                }
                else
                {
                    if (Random.Range(0, 1) >= 0.5f)
                    {
                        knownPlanets.Add(planetGO);
                    }
                }
            }



        }

        findNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = tank.resource.name + ": " + tank.count;
        text.transform.rotation = Camera.main.transform.rotation;


        Quaternion q = Quaternion.LookRotation(target.transform.position - transform.position);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 64f * Time.deltaTime * speed);




        Vector3 dir = target.transform.position - transform.position;
        //float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        Debug.DrawLine(target.transform.position, transform.position);

        Debug.DrawRay(transform.position, dir, Color.red);

        Vector3 targetDir = target.transform.position - transform.position;

        float whichWay = Vector3.Cross(transform.forward, targetDir).y;

        //float angle = Vector2.SignedAngle(new Vector2(target.transform.position.x, target.transform.position.z), new Vector2(transform.position.x, transform.position.z));
        //Debug.Log(whichWay);

        if (!avoidingDanger)
        {
            transform.Rotate(0, Mathf.Sign(whichWay) * speed, 0);
        }











        int layerMask = 1 << 8;




        RaycastHit hit;

        //Debug.DrawRay(transform.position, transform.TransformDirection(transform.forward - new Vector3(0,90f,0)) * 10f, Color.yellow);
        Debug.DrawRay(transform.position, transform.TransformDirection(Quaternion.Euler(0, 10f, 0) * Vector3.forward) * 1f, Color.yellow);
        Debug.DrawRay(transform.position, transform.TransformDirection(Quaternion.Euler(0, -10f, 0) * Vector3.forward) * 1f, Color.yellow);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1f, Color.yellow);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 10f, 0) * Vector3.forward), out hit, 1f, layerMask))
        {
            //Debug.Log("Something on the Right");
            //transform.rotation = transform.rotation * Quaternion.Euler(0, -1f * hit.distance * speed, 0);
            transform.Rotate(0, -speed, 0);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f, layerMask))
        {
            //Debug.Log("Something in the Center");
            //transform.rotation = transform.rotation * Quaternion.Euler(0, 1f * hit.distance * speed, 0);
            transform.Rotate(0, speed, 0);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, -10f, 0) * Vector3.forward), out hit, 1f, layerMask))
        {
            //Debug.Log("Something on the Left");
            //transform.rotation = transform.rotation * Quaternion.Euler(0, 1f * hit.distance * speed , 0);
            transform.Rotate(0, speed, 0);
        }



        //avoid dumbo stars

        Star starToAvoid = null;
        float maxInfluenceTemp = 0;
        float strength = 0;
        foreach (GameObject starGO in tempSensor.stars)
        {
            Star star = starGO.GetComponent<Star>();
            float distance = Vector3.Distance(transform.position, starGO.transform.position);

            if (distance <= star.influenceRadius)
            {
                //within it, find influence

                float influence = 1 - distance / star.influenceRadius; //realistically should be.. quadratic? 

                float tempDelta = influence * star.temperature;

                if (tempDelta > maxInfluenceTemp)
                {
                    maxInfluenceTemp = tempDelta;
                    starToAvoid = star;
                    strength = influence;
                }


            }
        }
        if (starToAvoid != null)
        {

            if (tempSensor.currentTemperature > 50f)
            {
                avoidingDanger = true;
                /*
                //avoid star pls
                Debug.Log("Too hot!");
                float distance = Vector3.Distance(starToAvoid.gameObject.transform.position, transform.position);

                Quaternion sq = Quaternion.LookRotation(starToAvoid.gameObject.transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, sq, (128f * (tempSensor.currentTemperature/50f)) * Time.deltaTime * speed * -1f);

                */

                float maxDistance = starToAvoid.influenceRadius * .5f; //2.5f,fix for different stars

                //so it should attempt to never go closer than 2.5 units

                float distance = Vector3.Distance(transform.position, starToAvoid.transform.position);

                if (distance < maxDistance)
                {
                    //Debug.Log("Too hot!");
                    Quaternion sq = Quaternion.LookRotation(starToAvoid.gameObject.transform.position - transform.position);
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, sq, -(distance / maxDistance));

                    Vector3 starDir = starToAvoid.transform.position - transform.position;

                    float ww = Vector3.Cross(transform.forward, starDir).y;

                    transform.Rotate(0, -Mathf.Sign(ww), 0);

                }

            }
            else
            {
                avoidingDanger = false;
            }

        }





        //transform.Translate(Vector3.forward * Time.deltaTime * speed);

        transform.position = transform.position + transform.forward * Time.deltaTime * speed;

        if (!arrivedAtTarget)
        {

            if (Vector3.Distance(transform.position, target.transform.position) <= 1f) //arrived
            {
                //Debug.Log("Arrived");
                arrivedAtTarget = true;

                target.GetComponent<PlanetManager>().EnterPlanet(gameObject);





            }
        }

    }

    public void Launch()
    {
        arrivedAtTarget = false;
        findNewTarget();
    }

    public bool Load()
    {
        if (tank.count != 0) //sell ur shit
        {
            target.GetComponent<ResourceImport>().amount += (int)tank.count;
            tank.count = 0;
            return true;
        }
        else
        {

            if (target.GetComponent<ResourceExport>().amount > 8)
            {
                tank.count += 8;
                target.GetComponent<ResourceExport>().amount -= 8;
                return true;
            }
            else
            {
                if (target.GetComponent<ResourceExport>().amount == 0)
                {
                    return false;
                }
                else
                {
                    tank.count += target.GetComponent<ResourceExport>().amount;
                    target.GetComponent<ResourceExport>().amount = 0;
                    return true;
                }

            }
        }
    }

    void findNewTarget()
    {
        GameObject bestTarget = null;
        float value = -1f;
        foreach (GameObject planetGO in knownPlanets)
        {





            //find the best planet to go to
            if (tank.count == 0) //empty, need to fill up
            {
                if (planetGO.GetComponent<ResourceExport>() != null)
                {
                    Debug.Log("1");
                    Debug.Log(planetGO.GetComponent<ResourceExport>().resource);
                    Debug.Log(tank.resource);
                    if (planetGO.GetComponent<ResourceExport>().resource == tank.resource)
                    {
                        Debug.Log("2");
                        float tempValue = planetGO.GetComponent<ResourceExport>().amount / Vector3.Distance(transform.position, planetGO.transform.position);
                        //Debug.Log("Value for " + planetGO.name + ": " + tempValue);
                        if (tempValue > value)
                        {
                            value = tempValue;
                            bestTarget = planetGO;
                        }
                    }
                }

            }
            else
            {
                if (planetGO.GetComponent<ResourceImport>() != null)
                {
                    if (planetGO.GetComponent<ResourceImport>().resource == tank.resource)
                    {
                        float tempValue = 1 -(planetGO.GetComponent<ResourceImport>().amount / Vector3.Distance(transform.position, planetGO.transform.position));
                        //Debug.Log("Value for " + planetGO.name + ": " + tempValue);
                        if (tempValue > value)
                        {
                            value = tempValue;
                            bestTarget = planetGO;
                        }

                    }
                }
            }
        }
        target = bestTarget;
    }
}
