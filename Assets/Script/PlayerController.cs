using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- 公共变量 ---
    [Header("移动参数")]
    public float moveSpeed = 5f;

    [Header("跳跃参数 (基于网格)")]
    public float gridSize = 1f;
    public int jumpHeightInGrids = 3;
    public float jumpDuration = 0.3f;

    [Header("地面检测")]
    public Transform[] groundChecks;
    public float checkRadius = 0.1f;
    public LayerMask whatIsGround;

    // --- 私有变量 ---
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isJumping = false;
    private float lockedHorizontalSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = false;
        foreach (var check in groundChecks)
        {
            if (Physics2D.OverlapCircle(check.position, checkRadius, whatIsGround))
            {
                isGrounded = true;
                break;
            }
        }

        if (isGrounded && !isJumping)
        {
            moveInput = Input.GetAxis("Horizontal");
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            // 跳跃时，我们根据当前的输入意图来锁定速度
            lockedHorizontalSpeed = moveInput * moveSpeed;
            StartCoroutine(GridJump());
        }
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            if (!isJumping)
            {
                // 地面移动
                Vector2 groundVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
                rb.velocity = groundVelocity;

                // --- 核心修复点 ---
                // 只要在地面上，就持续更新“可锁定的水平速度”。
                // 这样当我们走路掉下平台时，这个变量里存的就是掉下去前最后一刻的速度。
                lockedHorizontalSpeed = groundVelocity.x;
            }
        }
        else // 如果在空中
        {
            // 无论是跳跃还是掉落，都使用锁定的水平速度
            rb.velocity = new Vector2(lockedHorizontalSpeed, rb.velocity.y);
        }
    }

    IEnumerator GridJump()
    {
        isJumping = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        Vector2 startPos = transform.position;

        float jumpHeight = jumpHeightInGrids * gridSize;
        float horizontalDistance = lockedHorizontalSpeed * jumpDuration;

        // --- 新增的修复代码 ---
        // 1. 获取玩家的碰撞体大小，为BoxCast做准备
        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
        Vector2 castDirection = new Vector2(horizontalDistance, jumpHeight).normalized;
        float castDistance = new Vector2(horizontalDistance, jumpHeight).magnitude;

        // 2. 在跳跃路径上进行一次盒子投射，只检测"whatIsGround"层
        RaycastHit2D hit = Physics2D.BoxCast(
            startPos,                      // 盒子的起始中心点
            playerCollider.size,           // 盒子的尺寸
            0f,                            // 盒子的角度
            castDirection,                 // 投射方向
            castDistance,                  // 投射距离
            whatIsGround                   // 只检测“地面”层（墙也属于这个层）
        );

        // 如果投射命中了物体（墙）
        if (hit.collider != null)
        {
            // --- 经典蹬墙跳的修改 ---
            // 不再是设为0，而是将水平方向反向
            horizontalDistance = -horizontalDistance;
            lockedHorizontalSpeed = -lockedHorizontalSpeed;

            // 你甚至可以给一个额外的“蹬墙”力度
            lockedHorizontalSpeed = -lockedHorizontalSpeed * 1.2f; // 增加20%的反向速度
        }

        Vector2 targetPos = startPos + new Vector2(horizontalDistance, jumpHeight);

        float elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        rb.gravityScale = originalGravity;
        rb.velocity = new Vector2(lockedHorizontalSpeed, 0);

        isJumping = false;
    }
}