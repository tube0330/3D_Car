using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 무게중심
// 2. pathTransform 참조
// 3. 타이어의 휠 Collider 배치, 모델 배치

public class AICar : MonoBehaviour
{
    [Header("Center of Mass")]
    Transform tr;
    Rigidbody rb;
    Vector3 CentOfMass = new Vector3(0f, -0.5f, 0f);

    [Header("Path")]
    [SerializeField] Transform path;
    [SerializeField] Transform[] pathTransforms;
    [SerializeField] List<Transform> pathList;

    [Header("Wheel Collider")]
    [SerializeField] WheelCollider FrontL;
    [SerializeField] WheelCollider FrontR;
    [SerializeField] WheelCollider BackL;
    [SerializeField] WheelCollider BackR;

    [Header("Models")]
    [SerializeField] Transform FrontL_Tr;
    [SerializeField] Transform FrontR_Tr;
    [SerializeField] Transform BackL_Tr;
    [SerializeField] Transform BackR_Tr;

    [Header("Obstacle")]
    LayerMask trackLayer;
    [SerializeField] float sensorLength = 10f;
    [SerializeField] Vector3 FrontSensorPos = new(0f, 0.25f, 0.5f);    // 전방 센서 위치
    [SerializeField] Transform StartSensorPos;           // 센서 시작 위치
    [SerializeField] float FrontSideSensorPos = 0.4f;    // 측면 전방 센서 위치
    [SerializeField] float frontSensorAngle = 30f;       // 측면 센서 회전 각도
    bool isavoid = false;               // 회피 여부
    [SerializeField] float targetSteerAngle = 0;         // 목표 조향각


    public float curSpeed = 0;      // 현재 속도
    float maxSpeed = 100f;          // 최대 속도
    int curNode = 0;                // 현재 노드
    float maxTorque = 500f;         // 최대 토크
    float maxSteerAngle = 35f;      // 최대 조향각
    float maxBrake = 150000f;       // 최대 브레이크

    void Start()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = CentOfMass;
        path = GameObject.Find("PathTransform").transform;
        pathTransforms = path.GetComponentsInChildren<Transform>();
        trackLayer = 1 << LayerMask.NameToLayer("TRACK");

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path)
                pathList.Add(pathTransforms[i]);
        }
    }

    void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWayPointDistance();
        CarSensor();
    }

    void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(pathList[curNode].position);   // 현재 노드의 위치를 상대좌표로 변환합니다.
        float newSteer = relativeVector.x / relativeVector.magnitude * maxSteerAngle;           // 조향각을 계산합니다.
        //FrontL.steerAngle = newSteer;                                               // 좌측 앞바퀴의 조향각을 설정합니다.
        //FrontR.steerAngle = newSteer;                                               // 우측 앞바퀴의 조향각을 설정합니다.
        targetSteerAngle = newSteer;
        LerpToSteerAngle();
    }

    void Drive()
    {
        curSpeed = 2 * Mathf.PI * FrontL.radius * FrontL.rpm * 60 / 1000;           // 현재 속도를 계산합니다.

        if (curSpeed < maxSpeed)
        {
            BackL.motorTorque = maxTorque;
            BackR.motorTorque = maxTorque;
        }
        else
        {
            BackL.motorTorque = 0;
            BackR.motorTorque = 0;
        }
    }

    void CarSensor()
    {
        #region Raycast Obstacles
        RaycastHit hit;
        Vector3 sensorStartPos = tr.position;
        sensorStartPos += tr.forward * FrontSensorPos.z;
        sensorStartPos += tr.up * FrontSensorPos.y;
        float avoidMultiplier = 0f;
        isavoid = false;

        // FrontCenter Sensor
        if (Physics.Raycast(sensorStartPos, tr.forward, out hit, sensorLength, ~trackLayer))
        {
            isavoid = true;
            Debug.Log(isavoid);
        }
        Debug.DrawLine(sensorStartPos, sensorStartPos + tr.forward * sensorLength, isavoid ? Color.red : Color.green);

        #region FrontRight
        // FrontRight Sensor
        sensorStartPos += tr.right * FrontSideSensorPos;
        if (Physics.Raycast(sensorStartPos, tr.forward, out hit, sensorLength, ~trackLayer))
        {
            isavoid = true;
            Debug.Log(isavoid);
        }
        // FrontRight Angle Sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength, ~trackLayer))
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            isavoid = true;
            avoidMultiplier -= 0.5f;
        }
        Debug.DrawLine(sensorStartPos, sensorStartPos + tr.forward * sensorLength, isavoid ? Color.red : Color.green);
        #endregion

        #region  FrontLeft
        // FrontLeft Sensor
        sensorStartPos -= tr.right * FrontSideSensorPos * 2f;
        if (Physics.Raycast(sensorStartPos, tr.forward, out hit, sensorLength, ~trackLayer))
        {
            isavoid = true;
            Debug.Log(isavoid);
            Debug.DrawLine(sensorStartPos, sensorStartPos + tr.forward * sensorLength, isavoid ? Color.red : Color.green);
        }
        // FrontLeft Angle Sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength, ~trackLayer))
        {
            Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            isavoid = true;
            avoidMultiplier += 0.5f;
        }
        #endregion
        #endregion

        #region Avoid Obstacles
        if (avoidMultiplier == 0)    //피할 물체가 없다면
        {
            if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, tr.up) * transform.forward, out hit, sensorLength, ~trackLayer))
            {
                isavoid = true;

                if (hit.normal.x < 0)
                    avoidMultiplier = -1;

                else avoidMultiplier = 1;
            }
        }

        if (isavoid)
            targetSteerAngle = avoidMultiplier * maxSteerAngle;
        #endregion
    }

    void CheckWayPointDistance()
    {
        if (Vector3.Distance(tr.position, pathList[curNode].position) <= 20f)       // 현재 노드와의 거리가 10f 이하일 경우
        {
            if (curNode == pathList.Count - 1)  // 현재 노드가 마지막 노드일 경우
                curNode = 0;    // 첫 번째 노드로 이동합니다.
            else
                curNode++;      // 다음 노드로 이동합니다.
        }

    }

    void LerpToSteerAngle()
    {
        FrontL.steerAngle = Mathf.Lerp(FrontL.steerAngle, targetSteerAngle, Time.deltaTime * 10f);
    }
}
