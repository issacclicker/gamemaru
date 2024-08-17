using System;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public Camera faceCamera;

    public static InGameUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void InitPlayer(PlayerMovement playerMovement)
    {
        faceCamera.gameObject.transform.SetParent(playerMovement.transform);

        // 동물에 따라 위치와 회전값이 달라져야 할 수도 있음.
        faceCamera.transform.localPosition = new Vector3(0f, 1.98f, 0.96f);
        faceCamera.transform.localRotation = Quaternion.Euler(14.594f, 180f, 0f);
    }
}