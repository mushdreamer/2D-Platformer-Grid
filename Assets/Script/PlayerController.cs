using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- 公共变量 (可以在Unity检视面板中调整) ---
    [Header("移动参数")]
    public float moveSpeed = 5f; // 水平移动速度

    [Header("跳跃参数 (基于网格)")]
    public float gridSize = 1f; // 每个网格单元的大小 (Unity中的1个单位)
    public int jumpHeightInGrids = 3; // 跳跃的高度，以网格数为单位
    public float jumpDuration = 0.3f; // 完成跳跃动画所需的时间

    [Header("地面检测")]
    public Transform groundCheck;     // 用于检测地面的空物体的位置
    public float checkRadius = 0.2f;  // 检测范围的半径
    public LayerMask whatIsGround;    // 定义哪一个图层是“地面”

    // --- 私有变量 ---
    private Rigidbody2D rb;          // 玩家的刚体组件
    private float moveInput;         // 水平输入 (-1 到 1)
    private bool isGrounded;         // 玩家是否在地面上
    private bool isJumping = false;  // 玩家是否正在执行跳跃协程

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // --- 地面检测 ---
        // 我们将地面检测也放在Update中，以确保输入判断的及时性
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // --- 输入检测 ---
        // 只有在不跳跃的时候，才接收水平输入
        if (!isJumping)
        {
            moveInput = Input.GetAxis("Horizontal");
        }
        else
        {
            moveInput = 0; // 跳跃过程中禁止水平移动
        }

        // 检测跳跃输入，并且必须在地面上，且当前没有在跳跃中
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            // 启动跳跃协程
            StartCoroutine(GridJump());
        }
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    void FixedUpdate()
    {
        // 只有在不跳跃的时候，才应用物理移动
        if (!isJumping)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    // --- 跳跃协程 ---
    IEnumerator GridJump()
    {
        // 1. 标记开始跳跃
        isJumping = true;

        // 2. 暂时关闭重力影响
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero; // 清除所有物理速度

        // 3. 计算目标位置
        Vector2 startPos = transform.position;
        float jumpHeight = jumpHeightInGrids * gridSize;
        Vector2 targetPos = startPos + new Vector2(0, jumpHeight);

        // 4. 平滑移动到目标点
        float elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            // 使用Lerp（线性插值）平滑地将玩家从起点移动到终点
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保最终位置精确
        transform.position = targetPos;

        // 5. 恢复重力，让玩家自然下落
        rb.gravityScale = originalGravity;

        // 6. 标记跳跃结束
        isJumping = false;
    }
}