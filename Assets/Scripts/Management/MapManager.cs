using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    private GameManager gameManager;

    [Header("Map Management")] 
    [SerializeField] private int roomCount;
    // MapID to isEnabled 
    [SerializeField] private Dictionary<int, bool> roomsEnabled = new Dictionary<int, bool>();
    
    // TODO: minimap

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
        
        for(int i = 0; i < roomCount; i++)
            roomsEnabled.Add(i, false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public object UpdateMap(params object[] args)
    {
        if (args.Length >= 1 && args[0] is int newMapID)
        {
            roomsEnabled[newMapID] = true;
            UpdateVisualMap(newMapID);
        }
        return null;
    }

    private void UpdateVisualMap(int newMapID)
    {
        
    }
}