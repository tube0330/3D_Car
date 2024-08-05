using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    private RideCar C_ridecar;

    [Header("wheel Colliders")]
    public WheelCollider frontL_col;
    public WheelCollider frontR_col;
    public WheelCollider backL_col;
    public WheelCollider backR_col;

    [Header("wheel Models")]
    public Transform frontL_M;
    public Transform frontR_M;
    public Transform backL_M;
    public Transform backR_M;

    [Header("Mass Balance")]
    public Vector3 CentOfMass = new Vector3(0.0f, -0.5f, 0f);   //자동차나 마차 등 바퀴가 있는 모델의 무게중심의 y축은 항상 0.5로 잡아야 함
    public Rigidbody rb;

    [Header("앞바퀴 최대 회전각")]
    public float maxSteerAngle = 35f;

    [Header("최대 마찰력")]
    public float maxTorque = 2500f;

    [Header("최대 브레이크 강도")]
    public float maxBreak = 5000f;

    [Header("현재 스피드")]
    public float currentSpeed = 0f;
    private float steer = 0f;   //A, D 키 값을 받을 변수. 방향 잡는 용도
    private float forward = 0f; //W만 받아 전진하는 변수
    private float back = 0.0f;  //S만 받아 후진하는 변수
    bool isReverse = false; //전진인지 후진인지 판단하는 변수
    private float motor = 0f; //음수면 후진, 양수면 전진
    private float brake = 0f; //브레이크

    [Header("Lights")]
    public GameObject headLight_L;
    public GameObject headLight_R;
    private bool isHeadLightOn = false;
    public GameObject breakLight_L;
    public GameObject breakLight_R;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = CentOfMass;

        headLight_L.SetActive(false);
        headLight_R.SetActive(false);
        breakLight_L.SetActive(false);
        breakLight_R.SetActive(false);

        C_ridecar = transform.GetChild(5).GetComponent<RideCar>();
    }

    void Update()
    {
        if (!C_ridecar.isRide) return;

        if (Input.GetKeyDown(KeyCode.F))
            HeadLightOnOff();

    }

    void FixedUpdate()
    {
        if (!C_ridecar.isRide)
        {
            backL_col.brakeTorque = maxBreak;
            backR_col.brakeTorque = maxBreak;
            backL_col.motorTorque = 0f;
            backR_col.motorTorque = 0f;
            return;
        }

        currentSpeed = rb.velocity.sqrMagnitude;    //휠 콜라이더가 마찰력에 의해 리지드바디의 전체 속도(이륜)를 전달

        steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
        forward = Mathf.Clamp(Input.GetAxis("Vertical"), 0f, 1f);       //전진만 함
        back = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f);   //후진만 함


        if (C_ridecar.isRide)
        {
            if (Input.GetKey(KeyCode.B))
                brake = maxBreak;

            else CarMove();
        }   

        //뒷바퀴 Torque 회전력
        backL_col.motorTorque = motor * maxTorque;
        backR_col.motorTorque = motor * maxTorque;

        //뒷바퀴 break
        backL_col.brakeTorque = brake * maxBreak;
        backR_col.brakeTorque = brake * maxBreak;

        //앞바퀴 y축 회전각도
        frontL_col.steerAngle = steer * maxSteerAngle;
        frontR_col.steerAngle = steer * maxSteerAngle;

        //A, D 입력에 따라 y축 회전
        frontL_M.localEulerAngles = new Vector3(frontL_M.localEulerAngles.x, steer * maxSteerAngle, frontL_M.localEulerAngles.z);
        frontR_M.localEulerAngles = new Vector3(frontR_M.localEulerAngles.x, steer * maxSteerAngle, frontR_M.localEulerAngles.z);

        //모델링 회전 wheel colliders의 회전 Torque 값을 받아 같이 회전
        frontL_M.Rotate(frontL_col.rpm * Time.deltaTime, 0f, 0f);
        frontR_M.Rotate(frontR_col.rpm * Time.deltaTime, 0f, 0f);
        backL_M.Rotate(backL_col.rpm * Time.deltaTime, 0f, 0f);
        backR_M.Rotate(backR_col.rpm * Time.deltaTime, 0f, 0f);
    }

    private void HeadLightOnOff()
    {
        isHeadLightOn = !isHeadLightOn;
        //Debug.Log(isHeadLightOn);
        headLight_L.SetActive(isHeadLightOn);
        headLight_R.SetActive(isHeadLightOn);
    }

    private void BreakLightOnOff(bool state)
    {
        breakLight_L.SetActive(state);
        breakLight_R.SetActive(state);
    }

    private void CarMove()
    {
        if (Input.GetKey(KeyCode.W))
            StartCoroutine(ForwardCar());

        if (Input.GetKey(KeyCode.S))
            StartCoroutine(BackwardCar());

        if (isReverse)   //후진중일경우
        {
            BreakLightOnOff(isReverse);
            motor = -1 * back;
            brake = forward;
        }

        else    //전진중일경우
        {
            BreakLightOnOff(isReverse);
            motor = forward;
            brake = back;
        }
    }

    IEnumerator ForwardCar()
    {
        yield return new WaitForSeconds(0.1f);
        currentSpeed = 0f;

        if (back > 0f) isReverse = true;
        if (forward > 0f) isReverse = false;

        BreakLightOnOff(isReverse);
    }

    IEnumerator BackwardCar()
    {
        yield return new WaitForSeconds(0.1f);
        currentSpeed = 0.1f;

        if (back > 0f) isReverse = true;
        if (forward > 0f) isReverse = false;

        BreakLightOnOff(isReverse);
    }
}
