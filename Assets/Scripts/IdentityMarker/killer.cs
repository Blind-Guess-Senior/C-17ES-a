using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("KILLED");
        if (other.tag == "Player")
        {
            other.GetComponent<Reborn>().reborn_();
        }
    }
}
