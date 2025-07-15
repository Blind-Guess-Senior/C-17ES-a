using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    private GameManager gameManager;
    
    [Header("Map Management")]
    // MapID to 0.bool isEnabled 1.Vector3 CenterPoint
    [SerializeField] private Dictionary<int, object[]> maps = new Dictionary<int, object[]>();
    
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
    }

    // Update is called once per frame
    void Update()
    {
    }

    public object UpdateMap(params object[] args)
    {
        if (args.Length >= 1 && args[0] is int newMapID)
        {
            maps[newMapID][0] = true;
            UpdateVisualMap(newMapID);
        }
        return null;
    }

    private void UpdateVisualMap(int newMapID)
    {
        
    }
}