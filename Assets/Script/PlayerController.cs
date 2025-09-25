using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- �������� (������Unity��������е���) ---
    [Header("�ƶ�����")]
    public float moveSpeed = 5f; // ˮƽ�ƶ��ٶ�

    [Header("��Ծ���� (��������)")]
    public float gridSize = 1f; // ÿ������Ԫ�Ĵ�С (Unity�е�1����λ)
    public int jumpHeightInGrids = 3; // ��Ծ�ĸ߶ȣ���������Ϊ��λ
    public float jumpDuration = 0.3f; // �����Ծ���������ʱ��

    [Header("������")]
    public Transform groundCheck;     // ���ڼ�����Ŀ������λ��
    public float checkRadius = 0.2f;  // ��ⷶΧ�İ뾶
    public LayerMask whatIsGround;    // ������һ��ͼ���ǡ����桱

    // --- ˽�б��� ---
    private Rigidbody2D rb;          // ��ҵĸ������
    private float moveInput;         // ˮƽ���� (-1 �� 1)
    private bool isGrounded;         // ����Ƿ��ڵ�����
    private bool isJumping = false;  // ����Ƿ�����ִ����ԾЭ��

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // --- ������ ---
        // ���ǽ�������Ҳ����Update�У���ȷ�������жϵļ�ʱ��
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // --- ������ ---
        // ֻ���ڲ���Ծ��ʱ�򣬲Ž���ˮƽ����
        if (!isJumping)
        {
            moveInput = Input.GetAxis("Horizontal");
        }
        else
        {
            moveInput = 0; // ��Ծ�����н�ֹˮƽ�ƶ�
        }

        // �����Ծ���룬���ұ����ڵ����ϣ��ҵ�ǰû������Ծ��
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            // ������ԾЭ��
            StartCoroutine(GridJump());
        }
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    void FixedUpdate()
    {
        // ֻ���ڲ���Ծ��ʱ�򣬲�Ӧ�������ƶ�
        if (!isJumping)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    // --- ��ԾЭ�� ---
    IEnumerator GridJump()
    {
        // 1. ��ǿ�ʼ��Ծ
        isJumping = true;

        // 2. ��ʱ�ر�����Ӱ��
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero; // ������������ٶ�

        // 3. ����Ŀ��λ��
        Vector2 startPos = transform.position;
        float jumpHeight = jumpHeightInGrids * gridSize;
        Vector2 targetPos = startPos + new Vector2(0, jumpHeight);

        // 4. ƽ���ƶ���Ŀ���
        float elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            // ʹ��Lerp�����Բ�ֵ��ƽ���ؽ���Ҵ�����ƶ����յ�
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // �ȴ���һ֡
        }

        // ȷ������λ�þ�ȷ
        transform.position = targetPos;

        // 5. �ָ��������������Ȼ����
        rb.gravityScale = originalGravity;

        // 6. �����Ծ����
        isJumping = false;
    }
}