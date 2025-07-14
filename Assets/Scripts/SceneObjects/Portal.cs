using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private GameManager gameManager;
    private static int portalCounter = 0;

    [Header("Object Attr")] public int portalID;
    [SerializeField] private Portal targetPortal;
    [SerializeField] private bool teleportCooldown = true; // True for enabled.

    void Awake()
    {
        portalID = portalCounter++;
    }

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
        // Debug.Log(other.gameObject.name);
        if (!teleportCooldown)
        {
            return;
        }

        if (!other.GetComponentInParent<Teleportable>())
        {
            return;
        }

        GameObject teleporter = other.GetComponentInParent<Teleportable>().GetComponentInParent<EntityRoot>()
            .gameObject;

        gameManager.EventOccur("Teleport",
            teleporter, portalID, transform, targetPortal.portalID, targetPortal.transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (teleportCooldown)
        {
            return;
        }

        if (other.GetComponentInParent<Teleportable>())
            teleportCooldown = true;
    }

    public void TeleportAsTargetSuccess()
    {
        teleportCooldown = false;
    }
}