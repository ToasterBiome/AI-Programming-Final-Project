using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetResource : MonoBehaviour
{
    public float count;

    public TextMeshPro resourceText;

    public bool reciever = false;

    public float resourceTimer;

    public float resourceSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        resourceTimer += Time.deltaTime * resourceSpeed;

        if(Mathf.Abs(resourceTimer) >= 1)
        {
            resourceTimer -= Mathf.Sign(resourceTimer);
            count += Mathf.Sign(resourceSpeed);

            if(count < 0)
            {
                count = 0;
            }
        }

        resourceText.text = count.ToString();
    }
}
