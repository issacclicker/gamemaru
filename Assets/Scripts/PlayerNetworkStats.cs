using System;
using Unity.Netcode;

public class PlayerNetworkStats : NetworkBehaviour
{
    public static PlayerNetworkStats Instance;
    public int BeadCount => beadCount.Value;

    private NetworkVariable<int> beadCount;

    private void Awake()
    {
        Instance = this;
        beadCount = new NetworkVariable<int>(0);
        beadCount.OnValueChanged += OnBeadCountChanged;
    }

    private void OnBeadCountChanged(int previousvalue, int newvalue)
    {
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateBeadCountText(newvalue);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseBeadCountServerRpc(ulong clientId)
    {
        beadCount.Value++;
    }
}