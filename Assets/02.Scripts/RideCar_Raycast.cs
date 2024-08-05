using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideCar_Raycast : MonoBehaviour
{
    public float raycastDistance = 5f; // 레이캐스트 거리
    public LayerMask carLayer; // car 레이어

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            // 카메라 앞쪽에서 레이캐스트 발사
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, raycastDistance, carLayer))
            {
                // car 레이어의 오브젝트와 충돌했을 때
                GetInCar(hit.transform);
            }
        }
    }

    void GetInCar(Transform carTransform)
    {
        // 차량 탑승 로직 구현
        // 예시: 플레이어를 차량의 자식 오브젝트로 만들고 위치를 조정
        transform.SetParent(carTransform);
        transform.localPosition = Vector3.zero;
    }
}