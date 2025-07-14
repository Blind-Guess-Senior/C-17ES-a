using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBoom : MonoBehaviour, IBomb
{
    [Header("IBomb Attr")]
    public float power { get; private set; }
    public float explodeTime { get; private set; }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        StartCoroutine(DestroySequence());
    }

    private bool hasExploded = false;

    public float fallSpeed = 5f;
    public float maxFallSpeed = 10f;
    public float explosionRadius;

    private Rigidbody2D rb;
    private CircleCollider2D coll;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        coll = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        float currentVelocityY = Mathf.Min(rb.velocity.y, maxFallSpeed);
        rb.velocity = new Vector2(0, currentVelocityY - fallSpeed * Time.deltaTime);
    }

    // Unused
    public void Explode_()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D nearbyCollider in colliders)
        {
            if (nearbyCollider.CompareTag("Wall_destroyable"))
            {
                Destroy(nearbyCollider.gameObject);
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator DestroySequence()
    {
        // 粒子效果
        // if (destructionParticlesPrefab != null)
        // {
        //     Instantiate(destructionParticlesPrefab, transform.position, Quaternion.identity);
        // }

        // （可选）在这里播放音效
        // AudioManager.Instance.Play("PlatformSmashSound");

        // 禁用渲染器和碰撞体，让它看起来立刻消失了
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // 等待一小段时间，确保粒子效果能被看到
        yield return new WaitForSeconds(explodeTime);

        // Destroy it self
        Destroy(gameObject);
    }

    public void SetBombAttr(float _power, float _explodeTime)
    {
        power = _power;
        explodeTime = _explodeTime;
    }

    // 工具方法：供外部调用带参数爆炸
    public void InvokeExplodeWithRadius(float radius)
    {
        explosionRadius = radius;
        coll.radius = radius;
    }
}