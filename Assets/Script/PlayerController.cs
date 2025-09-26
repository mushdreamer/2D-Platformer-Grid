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
        // 跳跃的水平距离现在直接使用起跳时锁定的速度
        float horizontalDistance = lockedHorizontalSpeed * jumpDuration;
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