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
    [SerializeField] private float destoryFallSpeed = 2f;

    private Vector3 originPoint;
    private Coroutine movementCoroutine;
    private List<Weighted> objectsOnPlatform = new List<Weighted>();
    private Rigidbody2D rb;
    private bool isDestroyed = false;

    private enum PlatformState
    {
        AtTop,
        MovingDown,
        AtBottom,
        MovingUp
    }

    [SerializeField] private PlatformState currentState = PlatformState.AtTop;

    // Start is called before the first frame update
    void Start()
    {
        originPoint = transform.position;
        if (endPoint == null)
        {
            Debug.Log(gameObject.name + ": No end point");
            this.enabled = false;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed) return;
        if (other.CompareTag("PlayerHeavyBody"))
        {
            Rigidbody2D playerRb = other.GetComponentInParent<Rigidbody2D>();
            if (playerRb != null)
            {
                Debug.Log(playerRb.velocity.y);
                // 检查垂直向下的速度是否足够大
                // velocity.y 是负值，所以我们和 -criticalFallSpeed 比较
                if (playerRb.velocity.y < -destoryFallSpeed)
                {
                    // 满足条件，开始摧毁流程
                    StartCoroutine(DestroySequence());
                    // **重要：** 直接返回，不再执行下面的重量计算逻辑
                    return;
                }
            }
        }

        Weighted weightedObj = other.GetComponentInParent<Weighted>();
        if (weightedObj != null && !objectsOnPlatform.Contains(weightedObj))
        {
            objectsOnPlatform.Add(weightedObj);
            UpdatePlatformState();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Weighted weightedObj = other.GetComponentInParent<Weighted>();
        if (weightedObj != null && objectsOnPlatform.Contains(weightedObj))
        {
            objectsOnPlatform.Remove(weightedObj);
            UpdatePlatformState();
        }
    }
    
    private IEnumerator DestroySequence()
    {
        isDestroyed = true; // 标记为已摧毁，防止任何其他交互

        // 停止所有移动
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);

        // 如果有粒子效果预制件，就在平台当前位置生成它
        // if (destructionParticlesPrefab != null)
        // {
        //     Instantiate(destructionParticlesPrefab, transform.position, Quaternion.identity);
        // }

        // （可选）在这里播放音效
        // AudioManager.Instance.Play("PlatformSmashSound");
        
        // 禁用平台的渲染器和碰撞体，让它看起来立刻消失了
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        
        // 等待一小段时间，确保粒子效果能被看到
        yield return new WaitForSeconds(0.1f);

        // 最后，彻底摧毁平台的游戏对象
        Destroy(transform.parent.gameObject);
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

    private void StartDescending()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(DescendCoroutine());
    }

    private void StartAscending()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(AscendCoroutine());
    }

    private IEnumerator DescendCoroutine()
    {
        currentState = PlatformState.MovingDown;
        while (Vector3.Distance(transform.position, endPoint.position) > 0.01f)
        {
            // 计算下一个位置
            Vector3 targetPosition =
                Vector3.MoveTowards(rb.position, endPoint.position, descendSpeed * Time.fixedDeltaTime);
            // **使用物理移动**
            rb.MovePosition(targetPosition);
            // **等待下一个物理帧**
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(endPoint.position);
        currentState = PlatformState.AtBottom;
        movementCoroutine = null;
    }

    private IEnumerator AscendCoroutine()
    {
        yield return new WaitForSeconds(ascendDelay);
        float currentTotalWeight = objectsOnPlatform.Sum(obj => obj.weight);
        if (currentTotalWeight >= weightThreshold)
        {
            movementCoroutine = null;
            UpdatePlatformState(); // 重新评估状态，可能会直接触发下降
            yield break; // 终止此上升协程
        }

        currentState = PlatformState.MovingUp;
        while (Vector3.Distance(transform.position, originPoint) > 0.01f)
        {
            // 计算下一个位置
            Vector3 targetPosition = Vector3.MoveTowards(rb.position, originPoint, ascendSpeed * Time.fixedDeltaTime);
            // **使用物理移动**
            rb.MovePosition(targetPosition);
            // **等待下一个物理帧**
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(originPoint);
        currentState = PlatformState.AtTop;
        movementCoroutine = null;
    }
}