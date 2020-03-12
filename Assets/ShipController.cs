using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public ResourceTank tank;

    public GameObject target;

    public GameObject[] planets;

    // Start is called before the first frame update
    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");

        findNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion q = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 64f * Time.deltaTime);

        transform.Translate(Vector3.forward * Time.deltaTime);


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
        foreach (GameObject planetGO in planets)
        {
            //find the best planet to go to
            if (tank.count == 0) //empty, need to fill up
            {
                if (planetGO.GetComponent<PlanetResource>().reciever == false)  //can take resources
                {
                    target = planetGO;
                }
            } else
            {
                if (planetGO.GetComponent<PlanetResource>().reciever == true)  //can take resources
                {
                    target = planetGO;
                }
            }
        }
    }
}
