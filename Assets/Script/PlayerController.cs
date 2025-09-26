using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- �������� ---
    [Header("�ƶ�����")]
    public float moveSpeed = 5f;

    [Header("��Ծ���� (��������)")]
    public float gridSize = 1f;
    public int jumpHeightInGrids = 3;
    public float jumpDuration = 0.3f;

    [Header("������")]
    public Transform[] groundChecks;
    public float checkRadius = 0.1f;
    public LayerMask whatIsGround;

    // --- ˽�б��� ---
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
            // ��Ծʱ�����Ǹ��ݵ�ǰ��������ͼ�������ٶ�
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
                // �����ƶ�
                Vector2 groundVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
                rb.velocity = groundVelocity;

                // --- �����޸��� ---
                // ֻҪ�ڵ����ϣ��ͳ������¡���������ˮƽ�ٶȡ���
                // ������������·����ƽ̨ʱ������������ľ��ǵ���ȥǰ���һ�̵��ٶȡ�
                lockedHorizontalSpeed = groundVelocity.x;
            }
        }
        else // ����ڿ���
        {
            // ��������Ծ���ǵ��䣬��ʹ��������ˮƽ�ٶ�
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
        // ��Ծ��ˮƽ��������ֱ��ʹ������ʱ�������ٶ�
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