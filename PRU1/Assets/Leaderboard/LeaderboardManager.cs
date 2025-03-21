using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardContainer;
    public GameObject leaderboardRowPrefab;

    void Awake()
    {
        if (leaderboardContainer == null)
        {
            leaderboardContainer = GameObject.Find("LeaderboardContainer")?.transform;
            leaderboardRowPrefab = Resources.Load<GameObject>("LeaderboardRow");
        }
    }

    void Start()
    {
        LoadLeaderboard();
    }

    public void SubmitScore(string playerName, int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScores", // This must match the leaderboard name in PlayFab
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreSubmitSuccess, OnScoreSubmitError);
    }

    private void OnScoreSubmitSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score submitted successfully!");
        LoadLeaderboard();  // Reload the leaderboard after submitting
    }

    private void OnScoreSubmitError(PlayFabError error)
    {
        Debug.LogError("Failed to submit score: " + error.GenerateErrorReport());
    }

    public void LoadLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScores",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSuccess, OnLeaderboardError);
    }

    private void OnLeaderboardSuccess(GetLeaderboardResult result)
    {
        // Clear existing leaderboard UI
        // foreach (Transform child in leaderboardContainer)
        // {
        //     Destroy(child.gameObject);
        // }
        if (leaderboardRowPrefab == null)
        {
            Debug.LogError("⚠️ leaderboardRowPrefab is NULL! Make sure it's assigned in the Inspector.");
        }
        int rank = 1;
        foreach (var entry in result.Leaderboard)
        {
            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContainer);
            row.SetActive(true);

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length >= 3)
            {
                texts[0].text = rank.ToString();   // Rank
                texts[1].text = entry.DisplayName ?? "Unknown"; // Player Name
                texts[2].text = entry.StatValue.ToString(); // Score
            }

            rank++;
        }
    }

    private void OnLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Failed to load leaderboard: " + error.GenerateErrorReport());
    }

}
