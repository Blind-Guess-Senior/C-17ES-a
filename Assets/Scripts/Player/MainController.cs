using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [Header("引用组件")] 
    public Rigidbody2D rb;
    public GameObject body;
    public GameObject heavyBody;
    public Transform footPoint;
    public LayerMask groundLayer;
    public Transform wallCheckPoint;
    public LayerMask wallLayer;

    [Header("Ability")] 
    [SerializeField] private Dictionary<string, bool> abilities = new Dictionary<string, bool>();
    // Ability Cheat Sheet
    /*
     * LeftMove
     * RightMove
     * UpMove
     * DownMove
     */

    [Header("About Room")] public int currentRoomID;

    [Header("水平移动设置")] public float rightSpeed = 3f;
    public float leftSpeed = 2f;
    public float gravityRight = 1f;
    public float gravityLeft = 2f;
    public float gravitydefault = 1.5f;
    public int weightDefault = 8;
    public int weightLeft = 12;
    public int weightRight = 4;
    [Header("加速度设置")] 
    public float accelRight = 10f;
    public float accelLeft = 8f;
    public float decel = 12f;
    public float maxFallSpeedRight = 5f;
    public float maxFallSpeedLeft = 8f;
    public float maxFallSpeeddefault = 6.5f;


    [Header("跳跃设置")] 
    public float jumpForce = 8f;
    public float upFallSpeedRatio = 0.3f;

    [Header("下落加速设置")] 
    public float downFallSpeedRatio = 1.5f;
    [Header("下落技能设置")] 
    public GameObject fallingBoomPrefab; // 拖入的预制体
    public float boomHoldTimeThreshold = 1.2f; // 达成阈值的蓄力时间
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float minJumpForce = 4f;
    [SerializeField] private float maxJumpForce = 8f;
    [SerializeField] private float minExplosionRadius = 2f;
    [SerializeField] private float maxExplosionRadius = 5f;
    [SerializeField] private float minExplosionTime = 0.5f;
    [SerializeField] private float maxExplosionTime = 0.5f;
    [SerializeField] private int layerMask;

    private float fallKeyHoldTime = 0f;
    private bool hasSpawnedBoom = false;


    private float baseGravityScale = 1f;
    public bool isGrounded = false;
    public bool isWall = false;
    private bool isWingMode = false;
    private bool isFastFall = false;

    private bool hasBounced = false;
    private float minBouncespeed = 2f;
    private Vector2 preGroundedVelocity = Vector2.zero;
    private bool wasFallingBeforeGround = false;
    public Wall wallComponent;

    private bool inputDisabled = false;

    private PlayerAnimationControl animControl;

    private enum MoveDirection
    {
        None,
        Left,
        Right
    }

    private MoveDirection currentDirection = MoveDirection.None;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.RegisterHandler("GetAbility", GetAbility);
        gameManager.RegisterHandler("SwitchRoom", StartSwitchRoom);
        gameManager.RegisterHandler("SwitchRoomFinish", EndSwitchRoom);

        abilities.Add("LeftMove", true);
        abilities.Add("RightMove", true);
        abilities.Add("UpMove", true);
        abilities.Add("DownMove", true);

        animControl = GetComponentInChildren<PlayerAnimationControl>();
        layerMask = ~(1 << (int)LayerMask.NameToLayer("Player"));

    }

    void Update()
    {
        if (inputDisabled) return;
        CheckGrounded();
        CheckWall();

        // 左右移动输入，记录状态
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentDirection = MoveDirection.Right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentDirection = MoveDirection.Left;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                currentDirection = MoveDirection.None;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                currentDirection = MoveDirection.Right;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                currentDirection = MoveDirection.Left;
            }
        }

        SetAnimation();

        ApplyMovement();
        if (wasFallingBeforeGround)
        {
            if (!CheckAbility("RightMove")){
                return;
            }
            if (currentDirection == MoveDirection.Right && isGrounded)
            {
                Debug.Log("弹跳中...");
                float bounceVelocity = 0.9f * -preGroundedVelocity.y;
                if (bounceVelocity > minBouncespeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, bounceVelocity);
                    // Debug.Log($"弹跳速度: {bounceVelocity:F2}");
                }
                else
                {
                    // 停止弹跳
                    bounceVelocity = 0f;
                }
            }
            else if (!isGrounded)
            {
                // 离地时重置弹跳标志
                hasBounced = false;
            }

            wasFallingBeforeGround = false;
        }

        if (currentDirection == MoveDirection.Left && isWall)
        {
            if(!CheckAbility("LeftMove")){
                return;
            }
            Debug.Log("玩家碰撞到墙");
            // 当玩家碰撞到墙时触发
            if (currentDirection == MoveDirection.Left)
            {
                Debug.Log("正在向左碰墙，开始爬墙");

                // 施加一个向上的力
                float climbForce = 5f;
                rb.velocity = new Vector2(rb.velocity.x, climbForce);
            }
        }


        // 向上跳跃和缓降
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && CheckAbility("UpMove"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isWingMode = false;
        }

        if (Input.GetKey(KeyCode.UpArrow) && rb.velocity.y < 0 && !isGrounded && !isWingMode)
        {
            if(!CheckAbility("UpMove")){
                return;
            }   
            isWingMode = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * upFallSpeedRatio);
            rb.gravityScale = baseGravityScale * upFallSpeedRatio;
        }

        if ((Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) && isWingMode)
        {
            isWingMode = false;
            rb.gravityScale = baseGravityScale;
        }

        float maxFallSpeed = currentDirection == MoveDirection.Right
            ? maxFallSpeedRight
            : (currentDirection == MoveDirection.Left ? maxFallSpeedLeft : maxFallSpeeddefault);

        if (rb.velocity.y < -maxFallSpeed)
        {
            gravityLeft = -downFallSpeedRatio;
            gravityRight = -downFallSpeedRatio;
            gravitydefault = -downFallSpeedRatio;
        }
        else
        {
            gravityRight = 1f;
            gravityLeft = 2f;
            gravitydefault = 1.5f;
        }
        // Debug.Log($"垂直速度: {rb.velocity.y:F2}");

        // 速降 & 蓄力生成Boom
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // Debug.Log("蓄力中...");
            if(!CheckAbility("DownMove")){
                return;
            }


            // 更新蓄力时间，并限制最大值
            fallKeyHoldTime += Time.deltaTime;
            fallKeyHoldTime = Mathf.Min(fallKeyHoldTime, boomHoldTimeThreshold);
        }

        // 松开下键触发生成
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (!CheckAbility("DownMove"))
            {
                return;
            }

            if (isGrounded)
            {
                // 地面跳跃逻辑
                float powerRatio = fallKeyHoldTime / boomHoldTimeThreshold;

                float minJumpForce = 4f;
                float maxJumpForce = 12f;
                float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, powerRatio);

                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Debug.Log($"蓄力跳跃，力度: {jumpForce:F2}");
            }
            else if (!hasSpawnedBoom)
            {
                // 空中生成炸弹逻辑（原逻辑）
                hasSpawnedBoom = true;

                float powerRatio = fallKeyHoldTime / boomHoldTimeThreshold;
                float scale = Mathf.Lerp(minScale, maxScale, powerRatio);
                float explosionRadius = Mathf.Lerp(minExplosionRadius, maxExplosionRadius, powerRatio);
                float explosionTime = Mathf.Lerp(minExplosionTime, maxExplosionTime, powerRatio);

                float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, powerRatio);
                Debug.Log($"蓄力跳跃，力度: {jumpForce:F2}");

                if (fallingBoomPrefab != null)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    GameObject boom = Instantiate(fallingBoomPrefab, footPoint.position, Quaternion.identity);
                    boom.transform.localScale = Vector3.one * scale;

                    FallBoom fallBoom = boom.GetComponent<FallBoom>();
                    if (fallBoom != null)
                    {
                        fallBoom.SetBombAttr(explosionRadius, explosionTime);
                        // fallBoom.InvokeExplodeWithRadius(explosionRadius);
                    }

                    Rigidbody2D boomRb = boom.GetComponent<Rigidbody2D>();
                    if (boomRb != null)
                    {
                        Vector2 extra = new Vector2(0, rb.velocity.y * 0.2f);
                        boomRb.velocity = rb.velocity + extra;
                    }
                }
                else
                {
                    Debug.LogWarning("FallingBoomPrefab 尚未指定！");
                }
            }

            // 重置状态

            fallKeyHoldTime = 0f;
        }

        if (isGrounded)
        {
            isFastFall = false;
            rb.gravityScale = baseGravityScale;

            hasSpawnedBoom = false;
        }

        // 每帧检测当前体态来设置重力
        if (!isWingMode && !isFastFall)
        {
            if (heavyBody.activeSelf)
            {
                baseGravityScale = gravityLeft;
            }
            else if (body.activeSelf)
            {
                baseGravityScale = gravityRight;
            }
            else
            {
                baseGravityScale = gravitydefault;
            }

            rb.gravityScale = baseGravityScale;
        }
    }

    private void SetAnimation()
    {
        int yInput = (int)Input.GetAxisRaw("Vertical");
        //Debug.Log(yInput);

        if (currentDirection == MoveDirection.None)
        {
            if (yInput == 0)
                animControl.SetState(Enum.PlayerState.Idle);
            else if (yInput == 1)
                animControl.SetState(Enum.PlayerState.Up);
            else
                animControl.SetState(Enum.PlayerState.Down);
        }
        else if (currentDirection == MoveDirection.Left)
        {
            if (yInput == 0)
                animControl.SetState(Enum.PlayerState.Left);
            else if (yInput == 1)
                animControl.SetState(Enum.PlayerState.UpLeft);
            else
                animControl.SetState(Enum.PlayerState.DownLeft);
        }
        else if (currentDirection == MoveDirection.Right)
        {
            if (yInput == 0)
                animControl.SetState(Enum.PlayerState.Right);
            else if (yInput == 1)
                animControl.SetState(Enum.PlayerState.UpRight);
            else
                animControl.SetState(Enum.PlayerState.DownRight);
        }
    }

    // 根据状态进行移动
    private float vx = 0f;

    private void ApplyMovement()
    {
        float targetGravity = 1f;
        Weighted weighted = GetComponent<Weighted>();
        // 若当前方向没有权限，强制停下
        if ((currentDirection == MoveDirection.Right && !CheckAbility("RightMove")) ||
            (currentDirection == MoveDirection.Left && !CheckAbility("LeftMove")))
        {
            currentDirection = MoveDirection.None;
        }


        switch (currentDirection)
        {
            case MoveDirection.Right:
                vx += accelRight * Time.deltaTime;
                vx = Mathf.Min(vx, rightSpeed);
                targetGravity = gravityRight;
                rb.mass = 1f;
                heavyBody.SetActive(false);
                weighted.weight = weightRight;
                break;

            case MoveDirection.Left:
                vx -= accelLeft * Time.deltaTime;
                vx = Mathf.Max(vx, -leftSpeed);
                targetGravity = gravityLeft;
                rb.mass = 2f;
                heavyBody.SetActive(true);
                weighted.weight = weightLeft;
                break;

            case MoveDirection.None:
                if (vx > 0)
                {
                    vx -= decel * Time.deltaTime;
                    if (vx < 0) vx = 0;
                }
                else if (vx < 0)
                {
                    vx += decel * Time.deltaTime;
                    if (vx > 0) vx = 0;
                }

                targetGravity = gravitydefault;
                rb.mass = 1.5f;
                heavyBody.SetActive(false);
                weighted.weight = weightLeft;
                break;
        }

        rb.velocity = new Vector2(vx, rb.velocity.y);
    }


    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(footPoint.position, Vector2.down, 0.4f, layerMask);
        bool prevIsGrounded = isGrounded;
        isGrounded = hit.collider != null &&
                     hit.collider.GetComponentInParent<EntityRoot>() != null &&
                     hit.collider.GetComponentInParent<EntityRoot>().GetComponent<Ground>() != null;

        if (!prevIsGrounded && isGrounded)
        {
            // 刚接触地面时记录落地前的速度
            preGroundedVelocity = rb.velocity;
            wasFallingBeforeGround = rb.velocity.y < 0; // 是否是下落状态中着地

            // Debug.Log($"落地前速度: {preGroundedVelocity}");
        }

        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(footPoint.position, Vector2.down * 0.4f, rayColor);
    }

    private void CheckWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, Vector2.left, 0.1f, layerMask);
        // RaycastHit2D hit =
        //     Physics2D.BoxCast(wallCheckPoint.position, new Vector2(0.2f, 0.2f), 0f, Vector2.left, 0.1f, layerMask);
        isWall = hit.collider != null &&
                 hit.collider.GetComponentInParent<EntityRoot>() != null &&
                 hit.collider.GetComponentInParent<EntityRoot>().GetComponent<Wall>() != null;

        Color rayColor = isWall ? Color.green : Color.red;
        Debug.DrawRay(wallCheckPoint.position, Vector2.left * 0.1f, rayColor);
    }

    public object GetAbility(params object[] args)
    {
        if (args.Length >= 1 && args[0] is string abilityName)
        {
            if (abilities.ContainsKey(abilityName))
            {
                abilities[abilityName] = true;
            }
            else
            {
                abilities.Add(abilityName, true);
            }
        }

        return null;
    }

    public object StartSwitchRoom(params object[] args)
    {
        if (args.Length >= 2 && args[1] is int newRoomID)
        {
            currentRoomID = newRoomID;
            inputDisabled = true;
        }

        return null;
    }

    public object EndSwitchRoom(params object[] args)
    {
        if (args.Length >= 1)
        {
            inputDisabled = false;
        }

        return null;
    }

    bool CheckAbility(string abilityName)
    {
        return abilities.ContainsKey(abilityName) && abilities[abilityName];
    }
}