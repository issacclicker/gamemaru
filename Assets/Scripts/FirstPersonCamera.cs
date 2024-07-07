using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 1.6f, 0); // 카메라가 플레이어의 머리 위치에 위치하도록 설정

    private void Update()
    {
        // 카메라 위치 설정
        transform.position = player.position + offset;

        // 마우스 입력 값 가져오기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 플레이어 회전 적용
        player.Rotate(Vector3.up * mouseX);

        // 카메라 회전 적용
        transform.Rotate(Vector3.left * mouseY);

        // 카메라 회전 제한
        float xRotation = Mathf.Clamp(transform.localEulerAngles.x, 0f, 80f);
        transform.localEulerAngles = new Vector3(xRotation, 0, 0);
    }
}
