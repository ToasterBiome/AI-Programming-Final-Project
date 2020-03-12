using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float speed = 0.035f;

    public float zoomLevel = 5;
    public float targetZoomLevel = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x < 100)
        {
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z + speed);
        }

        if (mousePos.x > Screen.width - 100)
        {
            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z - speed);
        }



        if (mousePos.y < 100)
        {
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z - speed);
        }

        if (mousePos.y > Screen.height - 100)
        {
            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z + speed);
        }

        targetZoomLevel -= Input.mouseScrollDelta.y;

        zoomLevel = Mathf.Lerp(zoomLevel, targetZoomLevel, Time.deltaTime * 4);

        if(Mathf.Approximately(targetZoomLevel,zoomLevel))
        {
            targetZoomLevel = zoomLevel;
        }

        /*

        if(targetZoomLevel <= 1)
        {
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x,15f,Time.deltaTime * 2), 45f, 0);
        } else
        {
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 45f, Time.deltaTime * 4), 45f, 0);
        }

        */

        Camera.main.orthographicSize = zoomLevel;


    }
}
