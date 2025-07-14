using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Object Attr")]
    [SerializeField] int mapPointID;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerBody"))
        {
            return;
        }

        gameManager.EventOccur("UpdateMap", mapPointID);
        Destroy(gameObject);
    }
}