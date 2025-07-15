using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSwitchZone : MonoBehaviour
{
    private GameManager gameManager;
    private bool roomSwitched = false; // Is room switch ongoing?

    [SerializeField] private int currentRoomID;
    [SerializeField] private int nextRoomID;

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
        if (roomSwitched)
        {
            return;
        }

        if (!other.CompareTag("PlayerHeart"))
        {
            return;
        }

        if (other.GetComponentInParent<MainController>().currentRoomID != currentRoomID)
        {
            return;
        }
        
        roomSwitched = true;
        gameManager.EventOccur("SwitchRoom", currentRoomID, nextRoomID);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!roomSwitched)
        {
            return;
        }

        if (!other.CompareTag("PlayerHeart"))
        {
            return;
        }

        roomSwitched = false;
    }
}