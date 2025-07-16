using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reborn : MonoBehaviour
{
     private GameManager gameManager;
     public Transform rebornPoint;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.RegisterHandler("Reborn", reborn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public object reborn(object[] args)
    {
        transform.position = rebornPoint.position;
        return null;
    }

    public void reborn_(){
        gameManager.EventOccur("Reborn");
    }
}
