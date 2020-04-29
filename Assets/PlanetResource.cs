using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetResource : MonoBehaviour
{
    public float count;

    public TextMeshPro resourceText;

    public TextMeshPro planetText;

    public bool reciever = false;

    public float resourceTimer;

    public float resourceSpeed = 1f;

    public List<GameObject> ships = new List<GameObject>();

    public bool launching = true;

    public enum ResourceType
    {
        Scrap,
        Steel
    }

    public ResourceType resourceType;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LaunchLoop");
    }

    // Update is called once per frame
    void Update()
    {


        resourceTimer += Time.deltaTime * resourceSpeed;

        if (Mathf.Abs(resourceTimer) >= 1)
        {
            resourceTimer -= Mathf.Sign(resourceTimer);
            count += Mathf.Sign(resourceSpeed);

            if (count < 0)
            {
                count = 0;
            }
        }

        resourceText.text = count.ToString();
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
}
