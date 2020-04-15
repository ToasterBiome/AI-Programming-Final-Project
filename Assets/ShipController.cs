using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public ResourceTank tank;

    public GameObject target;

    public GameObject[] planets;

    public TemperatureSensor tempSensor;

    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        tempSensor = GetComponent<TemperatureSensor>();
        findNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion q = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 64f * Time.deltaTime * speed);

        int layerMask = 1 << 8;




        RaycastHit hit;

        //Debug.DrawRay(transform.position, transform.TransformDirection(transform.forward - new Vector3(0,90f,0)) * 10f, Color.yellow);
        Debug.DrawRay(transform.position, transform.TransformDirection(Quaternion.Euler(0, 10f, 0) * Vector3.forward) * 1f, Color.yellow);
        Debug.DrawRay(transform.position, transform.TransformDirection(Quaternion.Euler(0, -10f, 0) * Vector3.forward) * 1f, Color.yellow);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1f, Color.yellow);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 10f, 0) * Vector3.forward), out hit, 1f, layerMask))
        {
            //Debug.Log("Something on the Right");
            transform.rotation = transform.rotation * Quaternion.Euler(0, -1f * hit.distance * speed, 0);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f, layerMask))
        {
            //Debug.Log("Something in the Center");
            transform.rotation = transform.rotation * Quaternion.Euler(0, 1f * hit.distance * speed, 0);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, -10f, 0) * Vector3.forward), out hit, 1f, layerMask))
        {
            //Debug.Log("Something on the Left");
            transform.rotation = transform.rotation * Quaternion.Euler(0, 1f * hit.distance * speed , 0);
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

                if(tempDelta  > maxInfluenceTemp)
                {
                    maxInfluenceTemp = tempDelta;
                    starToAvoid = star;
                    strength = influence;
                }


            }
        }
        if(starToAvoid != null)
        {
            
            if(tempSensor.currentTemperature > 65f)
            {
                //avoid star pls

                float distance = Vector3.Distance(starToAvoid.gameObject.transform.position, transform.position);

                Quaternion sq = Quaternion.LookRotation(starToAvoid.gameObject.transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Inverse(sq), speed * 32f * (4/distance) * Time.deltaTime);

            }

        }
        




        transform.Translate(Vector3.forward * Time.deltaTime * speed);


        if(Vector3.Distance(transform.position,target.transform.position) <= 1f) //arrived
        {
            Debug.Log("Arrived");

            if (target.GetComponent<PlanetResource>().reciever) //sell ur shit
            {
                target.GetComponent<PlanetResource>().count += tank.count;
                tank.count = 0;

            }
            else
            {
                if (target.GetComponent<PlanetResource>().count > 8)
                {
                    tank.count += 8;
                    target.GetComponent<PlanetResource>().count -= 8;
                }
                else
                {
                    tank.count += target.GetComponent<PlanetResource>().count;
                    target.GetComponent<PlanetResource>().count = 0;
                }
            }



            
            findNewTarget();
        }


    }

    void findNewTarget()
    {
        GameObject bestTarget = null;
        float value = 0f;
        foreach (GameObject planetGO in planets)
        {
            
            //find the best planet to go to
            if (tank.count == 0) //empty, need to fill up
            {
                if (planetGO.GetComponent<PlanetResource>().reciever == false)  //can give resources
                {
                    float tempValue = planetGO.GetComponent<PlanetResource>().count / Vector3.Distance(transform.position, planetGO.transform.position);
                    Debug.Log("Value for " + planetGO.name + ": " + tempValue);
                    if (tempValue > value)
                    {
                        value = tempValue;
                        bestTarget = planetGO;
                    }
                }
            } else
            {
                if (planetGO.GetComponent<PlanetResource>().reciever == true)  //can take resources
                {
                    float tempValue = (1/planetGO.GetComponent<PlanetResource>().count) / Vector3.Distance(transform.position, planetGO.transform.position);
                    Debug.Log("Value for " + planetGO.name + ": " + tempValue);
                    if (tempValue > value)
                    {
                        value = tempValue;
                        bestTarget = planetGO;
                    }
                    
                }
            }
        }
        target = bestTarget;
    }
}
