using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Dictionary<string, List<Func<object[], object>>> rigisteredHandlers =
        new Dictionary<string, List<Func<object[], object>>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void RegisterHandler(string key, Func<object[], object> handler)
    {
        if (rigisteredHandlers.TryGetValue(key, out List<Func<object[], object>> handlerList))
        {
            handlerList.Add(handler);
        }
        else
        {
            rigisteredHandlers.Add(key, new List<Func<object[], object>> { handler });
        }
    }

    public void EventOccur(string eventName, params object[] args)
    {
        Debug.Log(args);
        if (!rigisteredHandlers.TryGetValue(eventName, out List<Func<object[], object>> handlerList))
        {
            return;
        }

        handlerList.ForEach(handler => handler.Invoke(args));
    }
}