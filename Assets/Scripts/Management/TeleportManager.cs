using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    private GameManager gameManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.RegisterHandler("Teleport", Teleport);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public object Teleport(object[] args)
    {
        if (args.Length >= 5 && args[0] is GameObject teleporter && args[1] is int starterID &&
            args[2] is Transform starter && args[3] is int targetID && args[4] is Transform target)
        {
            target.GetComponent<Portal>().TeleportAsTargetSuccess();
            teleporter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            teleporter.transform.position = target.position;
        }

        Debug.LogError("Teleport failed. Parameter error. args: " + args);
        return null;
    }
}