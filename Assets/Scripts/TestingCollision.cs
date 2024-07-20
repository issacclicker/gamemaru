using Unity.Netcode;
using UnityEngine;

public class TestingCollision : NetworkBehaviour
{
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            // 서버에서만 상호작용 처리
            if (other.CompareTag("Player"))
            {
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }

                // 클라이언트들에게 상태 변경 전파
                HideObjectClientRpc();
            }
        }
    }

    [ClientRpc]
    private void HideObjectClientRpc()
    {
        // 모든 클라이언트에서 MeshRenderer 비활성화
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }
}
