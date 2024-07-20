using UnityEngine;

public class HalfScreen : MonoBehaviour
{
    [SerializeField] private GameObject halfScreenPanel;

    void Start()
    {
        // 시작할 때 패널을 비활성화
        halfScreenPanel.SetActive(false);
    }

    public void ActivateHalfScreenPenalty()
    {
        halfScreenPanel.SetActive(true);
    }

    public void DeactivateHalfScreenPenalty()
    {
        halfScreenPanel.SetActive(false);
    }
}
