using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICar : MonoBehaviour
{
    [Header("CentOfMass")]
    [SerializeField] Rigidbody rb;
    public Vector3 CentOfMass = new Vector3(0f, -0.5f, 0f);

    [Header("Path")]
    [SerializeField] Transform path;
    [SerializeField] Transform[] pathTransforms;
    [SerializeField] List<Transform> pathList;

    [Header("Wheel Colliders")]
    [SerializeField] WheelCollider Front_L;
    [SerializeField] WheelCollider Front_R;
    [SerializeField] WheelCollider Back_L;
    [SerializeField] WheelCollider Back_R;
    [Header("Modeling")]
    [SerializeField] Transform Front_L_tr;
    [SerializeField] Transform Front_R_tr;
    [SerializeField] Transform Back_L_tr;
    [SerializeField] Transform Back_R_tr;

    public float curSpeed = 0f; //현재 속도
    int curNode = 0;    //현재노드
    float maxSpeed = 100f;
    public float maxMotorTorque = 2000f; //wheel collider가 회전하는 최대 힘
    public float maxSteerAngle = 30f; //앞바퀴 회전각도

    [Header("Avoid Obstacle")]
    [SerializeField] float sensorLength = 10f;
    [SerializeField] Vector3 frontSensorPos = new Vector3(0f, 0.2f, 0f);    //전방센서
    [SerializeField] public Transform SensorStartPos;   //센서위치를 차 안에 오브젝트로 설정
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = CentOfMass;
        path = GameObject.Find("PathTransform").transform;
        pathTransforms = path.GetComponentsInChildren<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path)
                pathList.Add(pathTransforms[i]);
        }
    }

    /* void Update()
    {

    } */

    void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWayPointDistance();
    }

    void ApplySteer()   //앞바퀴 wheel collider의 회전각도에 따라 회전
    {
        Vector3 relativeVector = transform.InverseTransformPoint(pathList[curNode].position);   //월드좌표를 로컬좌표로 변환

        float newSteer = relativeVector.x / relativeVector.magnitude * maxSteerAngle; //Pathtransform.x값 / Pathtransform의 개체 크기 * 30f

        Front_L.steerAngle = newSteer;
        Front_R.steerAngle = newSteer;
    }

    void Drive()
    {
        curSpeed = 2 * Mathf.PI * Front_L.radius * Front_L.rpm * 60 / 1000;
        /* 바퀴 둘레 계산: 2 * Mathf.PI * Front_L.radius로 바퀴의 둘레를 구합니다.
        분당 거리 계산: 바퀴 둘레에 분당 회전수(rpm)를 곱하여 분당 이동 거리를 구합니다.
        초당 거리 계산: 분당 거리를 60으로 나누어 초당 이동 거리를 구합니다.
        속도 계산: 초당 이동 거리를 1000으로 나누어 미터/초 단위의 속도로 변환합니다. */

        if (curSpeed < maxSpeed)
        {
            //최대 구동력을 적용하여 차량이 계속 가속하도록 함
            Back_L.motorTorque = maxMotorTorque;
            Back_R.motorTorque = maxMotorTorque;
        }

        else
        {
            //속도가 설정된 최대 속도에 도달했으므로 구동력을 멈추어 더 이상의 가속을 방지하고, 속도를 일정하게 유지하려는 의도
            Back_L.motorTorque = 0;
            Back_R.motorTorque = 0;
        }
    }

    void CheckWayPointDistance()
    {
        Debug.Log(curNode);
        if(Vector3.Distance(transform.position, pathList[curNode].position) <= 10f)
        {
            if(curNode == pathList.Count - 1)    //마지막에 있을 때 다시 0으로 초기화
            curNode = 0;

            else curNode++; //다음 waypoint로 이동
        }
    }
}
