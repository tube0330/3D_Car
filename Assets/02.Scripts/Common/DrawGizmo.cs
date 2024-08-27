using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;
    float gizmoRadius = 0.5f;

    // OnDrawGizmos는 씬 뷰에서 객체를 시각화할 때 호출됩니다.
    private void OnDrawGizmos()
    {
        // Gizmo 색상 설정
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
