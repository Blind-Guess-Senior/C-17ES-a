using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityGetter : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Object Attr")]
    [SerializeField] string abilityName;

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

        gameManager.EventOccur("GetAbility", abilityName);
        Destroy(gameObject);
    }
}