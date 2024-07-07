using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    void Update()
    {
        if(!IsOwner){
            return;
        }
        // HandleMovementServerAuth();
        HandleMovement();
    }

    // private void HandleMovementServerAuth(){
    //     // 입력 값 가져오기
    //     float moveHorizontal = Input.GetAxis("Horizontal");
    //     float moveVertical = Input.GetAxis("Vertical");
    //     HandleMovementServerRpc(moveHorizontal,moveVertical);
    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void HandleMovementServerRpc(float moveHorizontal,float moveVertical){

    //     // 이동 벡터 계산
    //     Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

    //     // 이동 적용
    //     transform.Translate(movement * speed * Time.deltaTime, Space.World);

    // }

    private void HandleMovement(){
        // 입력 값 가져오기
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // 이동 벡터 계산
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // 이동 적용
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
 
    }
}

