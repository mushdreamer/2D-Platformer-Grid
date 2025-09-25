using UnityEngine;

public class GameGridManager : MonoBehaviour
{
    [Header("网格设置")]
    public int width = 20;       // 网格的可视宽度 (以相机为中心)
    public int height = 10;      // 网格的可视高度 (以相机为中心)
    public float gridSize = 1f;  // 每个格子的边长

    [Header("跟随目标")]
    public Transform followTarget; // 网格将围绕这个目标绘制，通常是玩家

    [Header("可视化设置")]
    public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    // OnDrawGizmos is called by Unity in the editor to draw helper visuals
    private void OnDrawGizmos()
    {
        // 如果没有设置跟随目标或者格子大小不合法，则不绘制
        if (followTarget == null || gridSize <= 0) return;

        Gizmos.color = gridColor;

        // 1. 计算网格的中心点，并将其“吸附”到最近的整数坐标上
        // 这样可以确保网格线总是对齐世界单位，不会随着玩家的移动而“滑动”
        Vector3Int centerPoint = new Vector3Int(
            Mathf.RoundToInt(followTarget.position.x / gridSize),
            Mathf.RoundToInt(followTarget.position.y / gridSize),
            0
        );

        // 2. 计算绘制的起始和结束坐标
        int startX = centerPoint.x - (width / 2);
        int endX = centerPoint.x + (width / 2);
        int startY = centerPoint.y - (height / 2);
        int endY = centerPoint.y + (height / 2);

        // 3. 根据计算出的范围来绘制网格线
        // 绘制垂直线
        for (int x = startX; x <= endX; x++)
        {
            Vector3 startPos = new Vector3(x * gridSize, startY * gridSize, 0);
            Vector3 endPos = new Vector3(x * gridSize, endY * gridSize, 0);
            Gizmos.DrawLine(startPos, endPos);
        }

        // 绘制水平线
        for (int y = startY; y <= endY; y++)
        {
            Vector3 startPos = new Vector3(startX * gridSize, y * gridSize, 0);
            Vector3 endPos = new Vector3(endX * gridSize, y * gridSize, 0);
            Gizmos.DrawLine(startPos, endPos);
        }
    }

    // 坐标转换函数依然保留，它们很有用
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSize);
        int y = Mathf.FloorToInt(worldPosition.y / gridSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float x = (gridPosition.x * gridSize) + (gridSize / 2f);
        float y = (gridPosition.y * gridSize) + (gridSize / 2f);
        return new Vector3(x, y, 0);
    }
}