using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class UploadSpeedrunTime : MonoBehaviour
{
    [SerializeField] private SpeedrunTimer timer; // gán đối tượng Timer
    [SerializeField] private string statisticName = "Level3_Speedrun"; // Tên Leaderboard trên PlayFab
    [SerializeField] private string nextSceneName; // Tên Scene chuyển đến sau khi hoàn thành 

    private bool hasUploaded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasUploaded) return;

        if (collision.CompareTag("Player"))
        {
            hasUploaded = true;
            float time = (float)timer.GetElapsedSeconds();
            SendTimeToPlayFab(time);
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }

    private void SendTimeToPlayFab(float time)
    {
        int score = Mathf.RoundToInt(-time * 1000); // số âm, để leaderboard xếp thời gian tăng dần

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new System.Collections.Generic.List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log($"[PlayFab] Đã upload thời gian: {time:F3}s"),
            error => Debug.LogError("PlayFab Error: " + error.GenerateErrorReport()));
    }
}
