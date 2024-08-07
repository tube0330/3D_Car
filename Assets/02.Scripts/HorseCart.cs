using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseCart : MonoBehaviour
{
    [SerializeField] List<Transform> pathList;
    Transform tr;
    int curNode = 0;    //현재노드
    private float timePrev = 0f;


    void Start()
    {
        tr = transform;
        var path = GameObject.Find("PathTransform").gameObject;

        if (path != null)
            path.GetComponentsInChildren<Transform>(pathList);

        pathList.RemoveAt(0);
        timePrev = Time.time;
    }
    void Update()
    {
        if (Time.time - timePrev > 3f)
        {
            MoveWayPoint();
        }

        CheckWayPoint();
    }

    void MoveWayPoint()
    {
        Quaternion rot = Quaternion.LookRotation(pathList[curNode].position - tr.position);
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * 15f);
        tr.Translate(Vector3.forward * Time.deltaTime * 5f);
    }

    void CheckWayPoint()
    {
        if (Vector3.Distance(tr.position, pathList[curNode].position) <= 3.0f)
        {
            if (curNode == pathList.Count - 1)    //마지��에
            {
                curNode = 0;
            }

            else curNode++;
        }
    }
}
