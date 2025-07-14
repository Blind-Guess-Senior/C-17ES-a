using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateTrigger : MonoBehaviour
{
    [SerializeField] private Balance balance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Weighted weightedObj = other.GetComponent<Weighted>();
        if (weightedObj == null) return;

        if (transform.parent.name.Contains("Left"))
        {
            balance.AddToLeft(weightedObj);
        }
        else if (transform.parent.name.Contains("Right"))
        {
            balance.AddToRight(weightedObj);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Weighted weightedObj = other.GetComponent<Weighted>();
        if (weightedObj == null) return;

        if (transform.parent.name.Contains("Left"))
        {
            balance.RemoveFromLeft(weightedObj);
        }
        else if (transform.parent.name.Contains("Right"))
        {
            balance.RemoveFromRight(weightedObj);
        }
    }
}
