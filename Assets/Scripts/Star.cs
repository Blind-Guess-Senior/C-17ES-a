using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private GameObject bkg_1, bkg_2;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            bkg_1.SetActive(false);
            bkg_2.SetActive(true);
        }
    }
}
