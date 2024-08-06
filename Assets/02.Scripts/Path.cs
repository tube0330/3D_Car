using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] List<Transform> Nodes = new List<Transform>();
    [SerializeField] Color lineColor;

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();

        Nodes = new List<Transform>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform) //자기 자신을 제외하고 하위 트랜스폼을 담음
                Nodes.Add(pathTransforms[i]);
        }

        for (int i = 0; i < Nodes.Count; i++)
        {
            Vector3 curNode = Nodes[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0)
                previousNode = Nodes[i - 1].position;

            else if (i == 0 && Nodes.Count > 1) //i가 0과 같고 Nodes.Count가 1 초과일 때
                previousNode = Nodes[Nodes.Count - 1].position;

            Gizmos.DrawLine(previousNode, curNode); //좌표선은 이전 노드에서 현재 노드로
            Gizmos.DrawSphere(curNode, 1.0f);       //현재 노드에 1만큼 색상 넣기
        }

    }
}

