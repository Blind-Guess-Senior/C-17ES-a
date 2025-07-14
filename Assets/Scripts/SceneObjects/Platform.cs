using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Object Attr")] [SerializeField]
    private int weightThreshold;

    [SerializeField] private Transform endPoint;
    [SerializeField] private float ascendSpeed = 2f;
    [SerializeField] private float descendSpeed = 2f;
    [SerializeField] private float ascendDelay = 1f;

    private Transform originPoint;
    private Coroutine movementCoroutine; 
    private List<Weighted> objectsOnPlatform = new List<Weighted>();
    private enum PlatformState { AtTop, MovingDown, AtBottom, MovingUp }
    private PlatformState currentState = PlatformState.AtTop;

    // Start is called before the first frame update
    void Start()
    {
        originPoint = transform;
        if (endPoint == null)
        {
            Debug.Log(gameObject.name + ": No end point");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Weighted weightedObj = other.GetComponent<Weighted>();
        if (weightedObj != null && !objectsOnPlatform.Contains(weightedObj))
        {
            objectsOnPlatform.Add(weightedObj);
            UpdatePlatformState();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Weighted weightedObj = other.GetComponent<Weighted>();
        if (weightedObj != null || objectsOnPlatform.Contains(weightedObj))
        {
            objectsOnPlatform.Remove(weightedObj);
            UpdatePlatformState();
        }
    }

    private void UpdatePlatformState()
    {
        float currentTotalWeight = objectsOnPlatform.Sum(obj => obj.weight);

        if (currentTotalWeight >= weightThreshold)
        {
            if (currentState == PlatformState.AtTop || currentState == PlatformState.MovingUp)
            {
                StartDescending();
            }
        }
        else
        {
            if (currentState == PlatformState.AtBottom || currentState == PlatformState.MovingDown)
            {
                StartAscending();
            }
        }
    }

    private void Descend()
    {
        // 停止任何可能正在进行的上升协程
        if (movementCoroutine != null) StopCoroutine(ascendCoroutine);

        transform.position = endPoint.position;
        hasDescended = true;
    }

    private void StartAscendSequence()
    {
        ascendCoroutine = StartCoroutine(AscendCoroutine());
    }

    private IEnumerator AscendCoroutine()
    {
        yield return new WaitForSeconds(ascendDelay);
        while (Vector3.Distance(transform.position, originPoint.position) > 0.01f)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, originPoint.position, ascendSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = originPoint.position;
        hasDescended = false;
        ascendCoroutine = null;
    }
}