using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ScoreManager : NetworkBehaviour
{
    private NetworkVariable<int> foxScore = new NetworkVariable<int>(0);
    private NetworkVariable<int> tigerScore = new NetworkVariable<int>(0);
    public int winningScore = 1; // 일단 제대로 작동하는지 확인하기 위해 1점 얻으면 이기는 것으로 함

    public void AddFoxScore(int points)
    {
        if (IsServer)
        {
            foxScore.Value += points;
            CheckWinCondition();
        }
    }

    public void AddTigerScore(int points)
    {
        if (IsServer)
        {
            tigerScore.Value += points;
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (foxScore.Value >= winningScore)
        {
            AnnounceWinner("Fox");
        }
        else if (tigerScore.Value >= winningScore)
        {
            AnnounceWinner("Tiger");
        }
    }

    private void AnnounceWinner(string winner)
    {
        AnnounceWinnerClientRpc(winner);
        EndGameServerRpc(); // 게임 종료
    }

    [ClientRpc]
    private void AnnounceWinnerClientRpc(string winner)
    {
        Debug.Log(winner + " wins!");
        FindObjectOfType<TimerTest>()?.EndGame(); // Timer 종료
    }

    [ServerRpc]
    private void EndGameServerRpc()
    {
        Debug.Log("Game Over. Fox: " + foxScore.Value + " vs Tiger: " + tigerScore.Value);
        NetworkManager.Singleton.Shutdown(); 
    }

    public void EndGame()
    {
        Debug.Log("Game Over. Fox: " + foxScore.Value + " vs Tiger: " + tigerScore.Value);
        NetworkManager.Singleton.Shutdown(); 
    }

    public int GetFoxScore()
    {
        return foxScore.Value;
    }

    public int GetTigerScore()
    {
        return tigerScore.Value;
    }
}