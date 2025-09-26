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
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask whatIsGround;

    // --- ˽�б��� ---
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isJumping = false; // �����������ֻ��������ִ������Э�̡�
    private float lockedHorizontalSpeed; // <--- �޸�: ֻ����ˮƽ�ٶȣ�������

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // ֻ���ڵ������Ҳ�������Ծ���������У��Ž�����ҵ�����
        if (isGrounded && !isJumping)
        {
            moveInput = Input.GetAxis("Horizontal");
        }

        // ֻ���ڵ����ϲ�������
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            // ��������˲�䣬��¼�µ�ǰ��ˮƽ�ٶ�
            lockedHorizontalSpeed = moveInput * moveSpeed;
            StartCoroutine(GridJump());
        }
    }

    void FixedUpdate()
    {
        // �����߼�����:
        // 1. ����ڵ����� (isGrounded)����������������
        // 2. ����ڿ��� (!isGrounded)����ʹ������ʱ�������ٶ�
        if (isGrounded)
        {
            // ֻ�е���������������ʱ����Ӧ�õ����ƶ�
            if (!isJumping)
            {
                rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            }
        }
        else
        {
            // ����ڿ��� (��������������������������)
            // ��ǿ�Ʊ�������ʱ��ˮƽ�ٶȣ�������ֻӰ��Y��
            rb.velocity = new Vector2(lockedHorizontalSpeed, rb.velocity.y);
        }
    }

    IEnumerator GridJump()
    {
        isJumping = true;

        // --- �����׶� ---
        // ������׶Σ�������ʱ���������桰����վ������ȫ�ֶ�����λ��

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // �ر�����
        rb.velocity = Vector2.zero; // �������˲ʱ�ٶȣ���ֹ����

        Vector2 startPos = transform.position;

        // ����Ŀ��λ�� (ͬʱ����ˮƽ�ʹ�ֱλ��)
        float jumpHeight = jumpHeightInGrids * gridSize;
        float horizontalDistance = lockedHorizontalSpeed * jumpDuration;
        Vector2 targetPos = startPos + new Vector2(horizontalDistance, jumpHeight);

        // ʹ��Lerpƽ���ƶ�����ߵ�
        float elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            // �ֶ�����λ��
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // �ȴ���һ֡
        }

        // ȷ����ȷ����Ŀ���
        transform.position = targetPos;

        // --- ����׶� ---
        // ���ڣ��ѿ���Ȩ������������

        rb.gravityScale = originalGravity; // �ָ�����

        // **�ؼ�һ��**: Ϊ����׶����ó�ʼ�ٶȡ�
        // ˮƽ�ٶ���֮ǰ�������ٶȣ���ֱ�ٶ�Ϊ0��������ʼ��Ȼ���䡣
        rb.velocity = new Vector2(lockedHorizontalSpeed, 0);

        isJumping = false; // ����Э�̽���
    }
}