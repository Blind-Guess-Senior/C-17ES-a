using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<Bomb>())
        {
            return;
        }

        IBomb bomb = other.GetComponent<IBomb>();
        bomb.Explode();
    }
}
