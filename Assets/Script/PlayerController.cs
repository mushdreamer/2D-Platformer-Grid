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
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask whatIsGround;

    // --- 私有变量 ---
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isJumping = false; // 这个变量现在只代表“正在执行上升协程”
    private float lockedHorizontalSpeed; // <--- 修改: 只锁定水平速度，更清晰

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // 只有在地面上且不处于跳跃上升过程中，才接收玩家的输入
        if (isGrounded && !isJumping)
        {
            moveInput = Input.GetAxis("Horizontal");
        }

        // 只有在地面上才能起跳
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            // 在起跳的瞬间，记录下当前的水平速度
            lockedHorizontalSpeed = moveInput * moveSpeed;
            StartCoroutine(GridJump());
        }
    }

    void FixedUpdate()
    {
        // 核心逻辑修正:
        // 1. 如果在地面上 (isGrounded)，则由玩家输入控制
        // 2. 如果在空中 (!isGrounded)，则使用起跳时锁定的速度
        if (isGrounded)
        {
            // 只有当不在上升过程中时，才应用地面移动
            if (!isJumping)
            {
                rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            }
        }
        else
        {
            // 如果在空中 (无论是上升动画还是自由落体)
            // 就强制保持起跳时的水平速度，让重力只影响Y轴
            rb.velocity = new Vector2(lockedHorizontalSpeed, rb.velocity.y);
        }
    }

    IEnumerator GridJump()
    {
        isJumping = true;

        // --- 上升阶段 ---
        // 在这个阶段，我们暂时让物理引擎“靠边站”，完全手动控制位置

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // 关闭重力
        rb.velocity = Vector2.zero; // 清空所有瞬时速度，防止干扰

        Vector2 startPos = transform.position;

        // 计算目标位置 (同时包含水平和垂直位移)
        float jumpHeight = jumpHeightInGrids * gridSize;
        float horizontalDistance = lockedHorizontalSpeed * jumpDuration;
        Vector2 targetPos = startPos + new Vector2(horizontalDistance, jumpHeight);

        // 使用Lerp平滑移动到最高点
        float elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            // 手动更新位置
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保精确到达目标点
        transform.position = targetPos;

        // --- 下落阶段 ---
        // 现在，把控制权还给物理引擎

        rb.gravityScale = originalGravity; // 恢复重力

        // **关键一步**: 为下落阶段设置初始速度。
        // 水平速度是之前锁定的速度，垂直速度为0，让它开始自然下落。
        rb.velocity = new Vector2(lockedHorizontalSpeed, 0);

        isJumping = false; // 上升协程结束
    }
}