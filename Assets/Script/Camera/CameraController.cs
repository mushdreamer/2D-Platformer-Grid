using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("����Ŀ��")]
    public Transform target; // ��ҵ�Transform���

    [Header("�������")]
    public float smoothSpeed = 0.125f; // ��������ƽ���ȣ���ֵԽСԽƽ��
    public Vector3 offset; // ����������ҵ�ƫ����

    // LateUpdate is called after all Update functions have been called.
    // This is the best place for camera logic to avoid jittery movement.
    void LateUpdate()
    {
        // ���Ŀ���Ƿ���ڣ���ֹ����
        if (target == null)
        {
            Debug.LogWarning("���û��ָ������Ŀ�꣡");
            return;
        }

        // 1. �������������λ��
        // Ŀ��λ�� = ���λ�� + ƫ����
        Vector3 desiredPosition = target.position + offset;

        // 2. ʹ��Lerp�����Բ�ֵ��ƽ�����ƶ����
        // ����������һ�֡�׷�ϡ��ĸо��������ǽ�Ӳ��˲���ƶ�
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. �������λ��
        transform.position = smoothedPosition;
    }
}