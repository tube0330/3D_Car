using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.Utility;

public class PlayerCar : MonoBehaviourPun, IPunObservable
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft_Col;
    public WheelCollider frontRight_Col;
    public WheelCollider rearLeft_Col;
    public WheelCollider rearRight_Col;
    [Header("Wheel Models")]
    public Transform frontLeft_Model;
    public Transform frontRight_Model;
    public Transform rearLeft_Model;
    public Transform rearRight_Model;
    [Header("Mass Balance")]
    public Vector3 centerOfMass_var = new Vector3(0f, -0.5f, 0f);   // 무게중심 설정. 높이를 조절하여 차량의 무게중심을 조절할 수 있다.
    public Rigidbody rb;
    private Transform tr;
    [Header("Front Wheel Max Steer Angle")]
    private float maxSteerAngle = 35f;                   // 최대 조향각
    [Header("Max Torque")]
    private float maxTorque = 1000f;                     // 최대 토크
    [Header("Max Brake")]
    private float maxBrake = 150000f;                    // 최대 브레이크
    [Header("Current Speed")]
    public float currentSpeed = 0f;                      // 현재 속도
    private float steer = 0f;                       // 조향
    private float forward = 0f;                     // 전진
    private float back = 0f;                        // 후진
    private bool isReverse = false;                 // 후진 상태
    private float motor = 0f;                       // 모터
    private float brake = 0f;                       // 브레이크

    public bool isBrake = false;                    // 브레이크 밟았나[인스펙터 노출용]

    private Vector3 curPos = Vector3.zero;              // 동기화된 위치값
    private Quaternion curRot = Quaternion.identity;    // 동기화된 회전값

    void Awake()
    {
        photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
        photonView.ObservedComponents[0] = this;
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        curPos = tr.position;
        curRot = tr.rotation;
    }
    
    void Start()
    {
        if (photonView.IsMine)
        {
            rb.centerOfMass = centerOfMass_var;             // 무게중심 설정
            if (Camera.main.GetComponent<SmoothFollow>() != null)
            {
                Debug.Log("MainCamera Set");
                Camera.main.GetComponent<SmoothFollow>().target = tr;
                Camera.main.GetComponent<SmoothFollow>().rotationDamping = 3;
                Camera.main.GetComponent<SmoothFollow>().heightDamping = 3;
            }
            else
                Debug.Log("MainCamera null");
        }
    }

    void FixedUpdate()
    {
        //if (!photonView.IsMine) return;
        CarMoveWheel();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            CarMoveCondition();
            CarMoveInput();
            CarLight();
        }
        else
        {
            tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 10.0f);
            tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 10.0f);
        }
    }

    private void CarLight()
    {
        if (Input.GetKey(KeyCode.LeftShift)) return;
        if (Input.GetKey(KeyCode.S))
            CarLightCtrl.backLightsOn = true;
        else if (!Input.GetKey(KeyCode.S))
            CarLightCtrl.backLightsOn = false;

        if (Input.GetKey(KeyCode.LeftShift))
            CarBrakeOn();
        else if (!Input.GetKey(KeyCode.S))
            CarBrakeOff();
        void CarBrakeOn()
        {
            isBrake = true;
            CarLightCtrl.backLightsOn = isBrake;
        }
        void CarBrakeOff()
        {
            isBrake = false;
            CarLightCtrl.backLightsOn = isBrake;
        }
    }

    private void CarMoveInput()
    {
        currentSpeed = rb.velocity.sqrMagnitude;                      // 현재 속도 계산
        steer = Mathf.Clamp(Input.GetAxisRaw("Horizontal"), -1.0f, 1.0f);      // 조향 입력 (-1 ~ 1)
        forward = Mathf.Clamp(Input.GetAxisRaw("Vertical"), 0.0f, 1.0f);       // 전진 입력 (0 ~ 1)
        back = -1 * Mathf.Clamp(Input.GetAxisRaw("Vertical"), -1.0f, 0.0f);    // 후진 입력 (1 ~ 0)
        brake = Input.GetKey(KeyCode.LeftShift) ? 100 : 0;
        if (Input.GetKey(KeyCode.W))                    // 전진
            StartCoroutine(ForwardCar());                   // 전진 상태로 변경
        if (Input.GetKey(KeyCode.S))                    // 후진
            StartCoroutine(BackwardCar());                  // 후진 상태로 변경                   
    }

    IEnumerator ForwardCar()        // 전진 상태로 변경
    {
        yield return new WaitForSeconds(0.1f);
        currentSpeed = 0f;
        if (back > 0f)
            isReverse = true;
        if (forward > 0f)
            isReverse = false;
    }
    IEnumerator BackwardCar()       // 후진 상태로 변경
    {
        yield return new WaitForSeconds(0.1f);
        currentSpeed = 0.1f;
        if (back > 0f)
            isReverse = true;
        if (forward > 0f)
            isReverse = false;
    }

    private void CarMoveCondition()
    {
        if (isReverse)              // 후진 상태 
            motor = -back;              // 모터 토크 설정. 후진 상태에서는 back 값을 사용
        else                        // 전진 상태
            motor = forward;            // 모터 토크 설정. 전진 상태에서는 forward 값을 사용
    }

    private void CarMoveWheel()
    {
        rearLeft_Col.motorTorque = motor * maxTorque;   // 모터 토크 설정
        rearRight_Col.motorTorque = motor * maxTorque;  // 모터 토크 설정
        rearLeft_Col.brakeTorque = brake * maxBrake;      // 브레이크 토크 설정
        rearRight_Col.brakeTorque = brake * maxBrake;     // 브레이크 토크 설정

        frontLeft_Col.steerAngle = steer * maxSteerAngle;   // 조향각 설정. 좌우 조향각을 설정 (앞바퀴)
        frontRight_Col.steerAngle = steer * maxSteerAngle;  // 조향각 설정. 좌우 조향각을 설정 (앞바퀴)
        frontLeft_Model.localEulerAngles = new Vector3(frontLeft_Model.localEulerAngles.x, steer * maxSteerAngle, frontLeft_Model.localEulerAngles.z);   // 앞바퀴 모델의 회전값 설정. y축 회전값을 조향각으로 설정
        frontRight_Model.localEulerAngles = new Vector3(frontRight_Model.localEulerAngles.x, steer * maxSteerAngle, frontRight_Model.localEulerAngles.z);  // 앞바퀴 모델의 회전값 설정. y축 회전값을 조향각으로 설정
        frontLeft_Model.Rotate(frontLeft_Col.rpm / 60 * 360 * Time.deltaTime, 0, 0);    // 앞바퀴 모델의 회전값 설정. rpm 값에 따라 회전
        frontRight_Model.Rotate(frontRight_Col.rpm / 60 * 240 * Time.deltaTime, 0, 0);  // 앞바퀴 모델의 회전값 설정. rpm 값에 따라 회전
        rearLeft_Model.Rotate(rearLeft_Col.rpm / 60 * 360 * Time.deltaTime, 0, 0);      // 뒷바퀴 모델의 회전값 설정. rpm 값에 따라 회전
        rearRight_Model.Rotate(rearRight_Col.rpm / 60 * 360 * Time.deltaTime, 0, 0);    // 뒷바퀴 모델의 회전값 설정. rpm 값에 따라 회전
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if (stream.IsReading)
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}