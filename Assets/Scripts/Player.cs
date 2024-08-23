using UnityEngine;
using Unity.Netcode;
//역할에 맞는 캐릭터 생성

public class Player : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string playerRole = GameStateManager.Instance.GetPlayerRole();

            if (playerRole == "Fox")
            {
                CreateFoxCharacter();
            }
            else if (playerRole == "Tiger")
            {
                CreateTigerCharacter();
            }
        }
    }

    private void CreateFoxCharacter()
    {
        Debug.Log("Creating Fox Character for Player.");
    }

    private void CreateTigerCharacter()
    {
        Debug.Log("Creating Tiger Character for Player.");
    }

    [ClientRpc]
    public void SetPlayerRoleClientRpc(string role)
    {
        GameStateManager.Instance.SetPlayerRole(role);
    }
}
