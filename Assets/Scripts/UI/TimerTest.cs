using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TimerTest : NetworkBehaviour
{
    public Text timerText; // 시간을 표시할 텍스트 UI 요소

    private NetworkVariable<float> networkedElapsedTime = new NetworkVariable<float>();

    private float localElapsedTime = 0f;
    private bool isGameEnded = false;

    private void Start()
    {
        if (IsServer)
        {
            networkedElapsedTime.Value = 0f;
        }
    }

    private void Update()
    {
        if (IsServer && !isGameEnded)
        {
            localElapsedTime += Time.deltaTime;
            networkedElapsedTime.Value = localElapsedTime;
        }

        // 모든 클라이언트에서 동일한 시간 업데이트
        timerText.text = "Time: " + networkedElapsedTime.Value.ToString("F2") + "s";
    }

    public void EndGame()
    {
        isGameEnded = true;
    }
}
