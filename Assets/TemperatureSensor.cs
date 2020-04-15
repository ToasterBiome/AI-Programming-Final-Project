using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureSensor : MonoBehaviour
{

    public float currentTemperature;

    public Gradient tempGrad;

    public GameObject[] stars;

    // Start is called before the first frame update
    void Start()
    {
        stars = GameObject.FindGameObjectsWithTag("Star");
    }

    // Update is called once per frame
    void Update()
    {
        
        checkTemperature();
    }
    

    void checkTemperature()
    {
        currentTemperature = 0; //reset it so it can do it every frame

        if (stars != null) //if there is any
        {

            foreach (GameObject starGO in stars)
            {
                //get the star component
                Star star = starGO.GetComponent<Star>();



                //find if it in the influence radius of the star

                float distance = Vector3.Distance(transform.position, starGO.transform.position);

                if (distance <= star.influenceRadius)
                {
                    //within it, find influence

                    float influence = 1 - distance / star.influenceRadius; //realistically should be.. quadratic? 

                    float tempDelta = influence * star.temperature;

                    currentTemperature += tempDelta;


                }
            }
        }
    }

}
