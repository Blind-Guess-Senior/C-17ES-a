using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomCheck : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<FallBoom>() != null)
        {
            FallBoom bomb = collision.gameObject.GetComponent<FallBoom>();
            bomb.Explode();
        }
    }
}
