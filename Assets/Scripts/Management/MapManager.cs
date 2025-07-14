using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

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
        gameManager.RegisterHandler("UpdateMap", UpdateMap);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public object UpdateMap(params object[] args)
    {
        return null;
    }
}