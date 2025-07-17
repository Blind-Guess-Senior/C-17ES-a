using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [SerializeField] private GameObject bkg_2, bkg_3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            bkg_2.SetActive(false);
            bkg_3.SetActive(true);
        }
    }
}
