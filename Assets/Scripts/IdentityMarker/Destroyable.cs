using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField] private float powerThreshold = 1;
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
        if (!other.GetComponent<Bomb>())
        {
            return;
        }

        IBomb bomb = other.GetComponent<IBomb>();
        bomb.Explode();
        if (bomb.power >= powerThreshold)
        {
            // TODO: IS there need a visual effect? Maybe no?
            IDestroyable destroyable = GetComponentInParent<IDestroyable>();
            if(destroyable != null) 
                destroyable.DestroySelf();
            else
            {
                EntityRoot entityRoot = GetComponent<EntityRoot>();
                Destroy(entityRoot.gameObject);
            }
        }
    }
}
