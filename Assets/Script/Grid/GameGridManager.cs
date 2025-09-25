using UnityEngine;

public class GameGridManager : MonoBehaviour
{
    [Header("��������")]
    public int width = 20;       // ����Ŀ��ӿ�� (�����Ϊ����)
    public int height = 10;      // ����Ŀ��Ӹ߶� (�����Ϊ����)
    public float gridSize = 1f;  // ÿ�����ӵı߳�

    [Header("����Ŀ��")]
    public Transform followTarget; // ����Χ�����Ŀ����ƣ�ͨ�������

    [Header("���ӻ�����")]
    public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    // OnDrawGizmos is called by Unity in the editor to draw helper visuals
    private void OnDrawGizmos()
    {
        // ���û�����ø���Ŀ����߸��Ӵ�С���Ϸ����򲻻���
        if (followTarget == null || gridSize <= 0) return;

        Gizmos.color = gridColor;

        // 1. ������������ĵ㣬�����䡰�����������������������
        // ��������ȷ�����������Ƕ������絥λ������������ҵ��ƶ�����������
        Vector3Int centerPoint = new Vector3Int(
            Mathf.RoundToInt(followTarget.position.x / gridSize),
            Mathf.RoundToInt(followTarget.position.y / gridSize),
            0
        );

        // 2. ������Ƶ���ʼ�ͽ�������
        int startX = centerPoint.x - (width / 2);
        int endX = centerPoint.x + (width / 2);
        int startY = centerPoint.y - (height / 2);
        int endY = centerPoint.y + (height / 2);

        // 3. ���ݼ�����ķ�Χ������������
        // ���ƴ�ֱ��
        for (int x = startX; x <= endX; x++)
        {
            Vector3 startPos = new Vector3(x * gridSize, startY * gridSize, 0);
            Vector3 endPos = new Vector3(x * gridSize, endY * gridSize, 0);
            Gizmos.DrawLine(startPos, endPos);
        }

        // ����ˮƽ��
        for (int y = startY; y <= endY; y++)
        {
            Vector3 startPos = new Vector3(startX * gridSize, y * gridSize, 0);
            Vector3 endPos = new Vector3(endX * gridSize, y * gridSize, 0);
            Gizmos.DrawLine(startPos, endPos);
        }
    }

    // ����ת��������Ȼ���������Ǻ�����
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