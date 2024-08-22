using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GhostView : NetworkBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;

    public float verticalSpeed = 3f;  // 공중으로 뜨고 가라앉는 속도

    public GameObject camera;
    
    private CharacterController controller;
    private float rotationX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner == false)
        {
            camera.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어 이동
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        if (Input.GetKey(KeyCode.Space))
        {
            move.y += verticalSpeed;  // 공중으로 상승
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move.y -= verticalSpeed;  // 하강
        }

        controller.Move(move * speed * Time.deltaTime);

        // 마우스를 통한 시점 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0f, transform.localRotation.eulerAngles.y + mouseX, 0f);
        camera.GetComponent<Camera>().transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}


