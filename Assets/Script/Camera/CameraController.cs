using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target; // 玩家的Transform组件

    [Header("相机设置")]
    public float smoothSpeed = 0.125f; // 相机跟随的平滑度，数值越小越平滑
    public Vector3 offset; // 相机相对于玩家的偏移量

    // LateUpdate is called after all Update functions have been called.
    // This is the best place for camera logic to avoid jittery movement.
    void LateUpdate()
    {
        // 检查目标是否存在，防止报错
        if (target == null)
        {
            Debug.LogWarning("相机没有指定跟随目标！");
            return;
        }

        // 1. 计算期望到达的位置
        // 目标位置 = 玩家位置 + 偏移量
        Vector3 desiredPosition = target.position + offset;

        // 2. 使用Lerp（线性插值）平滑地移动相机
        // 这会让相机有一种“追赶”的感觉，而不是僵硬地瞬间移动
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. 更新相机位置
        transform.position = smoothedPosition;
    }
}