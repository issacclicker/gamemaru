using Unity.Netcode;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    public NetworkVariable<bool> isReady = new NetworkVariable<bool>(false);

    public void SetReadyStatus(bool ready)
    {
        if (IsClient && IsOwner)
        {
            isReady.Value = ready;
            CheckReadyToStartGameServerRpc();  // 서버에 준비 상태 업데이트를 알립니다.
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CheckReadyToStartGameServerRpc()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            var waitingRoomManager = FindObjectOfType<WaitingRoomManager>();
            if (waitingRoomManager != null)
            {
                waitingRoomManager.CheckReadyToStartGame();  // 서버에서 준비 상태를 확인하고 게임 시작
            }
        }
    }
}
