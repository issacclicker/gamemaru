using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float gameDuration = 600f; // 10분을 초 단위로 설정
    private float elapsedTime = 0f;
    private bool isGameRunning = true;

    void Start()
    {
        StartCoroutine(GameTimer());
    }

    void Update()
    {
        if (isGameRunning)
        {
            elapsedTime += Time.deltaTime;

            // 게임 종료 조건: 경과 시간이 gameDuration을 초과하면 게임 종료
            if (elapsedTime >= gameDuration)
            {
                EndGame();
            }
        }
    }

    IEnumerator GameTimer()
    {
        while (isGameRunning)
        {
            yield return new WaitForSeconds(1f); // 1초마다 경과 시간 업데이트
        }
    }

    void EndGame()
    {
        isGameRunning = false;
        Debug.Log("Game Over - Time's up!");
        // 게임 오버 
    }
}
