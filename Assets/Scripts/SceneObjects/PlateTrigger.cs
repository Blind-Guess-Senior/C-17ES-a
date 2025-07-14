using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateTrigger : MonoBehaviour, IDestroyable
{
    [SerializeField] private Balance balance;
    [SerializeField] private float destoryFallSpeed = 2f;
    public bool isDestroyed = false;

    private void OnTriggerEnter2D(Collider2D other)
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
                    DestroySequence();
                    // **重要：** 直接返回，不再执行下面的重量计算逻辑
                    return;
                }
            }
        }

        Weighted weightedObj = other.GetComponent<Weighted>();
        if (weightedObj == null) return;

        if (transform.parent.name.Contains("Left"))
        {
            balance.AddToLeft(weightedObj);
        }
        else if (transform.parent.name.Contains("Right"))
        {
            balance.AddToRight(weightedObj);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Weighted weightedObj = other.GetComponent<Weighted>();
        if (weightedObj == null) return;

        if (transform.parent.name.Contains("Left"))
        {
            balance.RemoveFromLeft(weightedObj);
        }
        else if (transform.parent.name.Contains("Right"))
        {
            balance.RemoveFromRight(weightedObj);
        }
    }

    private void DestroySequence()
    {
        isDestroyed = true; // 标记为已摧毁，防止任何其他交互
        if (transform.parent.name.Contains("Left"))
        {
            balance.ClearLeft();
        }
        else if (transform.parent.name.Contains("Right"))
        {
            balance.ClearRight();
        }

        // 如果有粒子效果预制件，就在平台当前位置生成它
        // if (destructionParticlesPrefab != null)
        // {
        //     Instantiate(destructionParticlesPrefab, transform.position, Quaternion.identity);
        // }

        // （可选）在这里播放音效
        // AudioManager.Instance.Play("PlatformSmashSound");

        // Caution! Must be different from the way use entityroot since it must no be destroyed.
        transform.parent.gameObject.SetActive(false);
        
    }

    public void DestroySelf()
    {
        if(!isDestroyed)
        {
            DestroySequence();
        }
    }
}