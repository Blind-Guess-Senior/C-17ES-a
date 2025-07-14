using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBoom : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float maxFallSpeed = 10f;
    public float explosionRadius;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
    }

    void Update()
    {
        float currentVelocityY = Mathf.Min(rb.velocity.y, maxFallSpeed);
        rb.velocity = new Vector2(0, currentVelocityY - fallSpeed * Time.deltaTime);
    }

    public void Explode()
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

    // 工具方法：供外部调用带参数爆炸
    public void InvokeExplodeWithRadius(float radius)
    {
        explosionRadius = radius;
    }
}
