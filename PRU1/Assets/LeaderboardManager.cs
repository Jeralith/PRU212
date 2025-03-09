using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    private DatabaseReference dbReference;
    public Transform leaderboardContainer;  // The parent container for leaderboard rows
    public GameObject leaderboardRowPrefab; // The row template prefab

    void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://pru212-935c1-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
        LoadLeaderboard();
    }

    public void SubmitScore(string playerName, int score)
    {
        string key = dbReference.Child("leaderboard").Push().Key;
        LeaderboardEntry entry = new LeaderboardEntry(playerName, score);

        dbReference.Child("leaderboard").Child(key).SetValueAsync(entry.ToDictionary()).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Score submitted successfully!");
                LoadLeaderboard(); // 🔹 Only reload after Firebase confirms the save
            }
            else
            {
                Debug.LogError("Failed to submit score.");
            }
        });
    }
    public void LoadLeaderboard()
{
    dbReference.Child("leaderboard").OrderByChild("score").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsCompleted && task.Result.Exists)
        {
            List<KeyValuePair<string, int>> leaderboardEntries = new List<KeyValuePair<string, int>>();

            foreach (var child in task.Result.Children)
            {
                string playerName = child.Child("playerName").Value.ToString();
                int score = int.Parse(child.Child("score").Value.ToString());
                leaderboardEntries.Add(new KeyValuePair<string, int>(playerName, score));
            }

            leaderboardEntries.Sort((a, b) => b.Value.CompareTo(a.Value));

            int currentChildren = leaderboardContainer.childCount - 1;
            int rank = currentChildren + 1;

            while (leaderboardContainer.childCount > 10)
            {
                DestroyImmediate(leaderboardContainer.GetChild(0).gameObject);
            }

            foreach (var entry in leaderboardEntries)
            {
                GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContainer);
                row.SetActive(true);

                TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var text in texts)
                {
                    text.gameObject.SetActive(true);
                    text.enabled = true;
                }

                if (texts.Length >= 3)
                {
                    texts[0].text = rank.ToString();   
                    texts[1].text = entry.Key;        
                    texts[2].text = entry.Value.ToString();
                }

                rank++;
            }
        }
        else
        {
            Debug.LogWarning("Leaderboard is empty or data fetch failed.");
        }
    });
}

//     public void LoadLeaderboard()
// {
//     dbReference.Child("leaderboard").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
//     {
//         if (!task.IsCompleted)
//         {
//             Debug.LogWarning("LoadLeaderboard: Task not completed.");
//             return;
//         }

//         if (!task.Result.Exists)
//         {
//             Debug.LogWarning("LoadLeaderboard: No scores found in database.");
//             return;
//         }

//         List<KeyValuePair<string, int>> leaderboardEntries = new List<KeyValuePair<string, int>>();

//         foreach (var child in task.Result.Children)
//         {
//             if (child.Child("playerName").Value != null && child.Child("score").Value != null)
//             {
//                 string playerName = child.Child("playerName").Value.ToString();
//                 int score = int.Parse(child.Child("score").Value.ToString());
//                 leaderboardEntries.Add(new KeyValuePair<string, int>(playerName, score));
//             }
//         }

//         if (leaderboardEntries.Count == 0)
//         {
//             Debug.LogWarning("LoadLeaderboard: No valid leaderboard entries found.");
//             return;
//         }

//         leaderboardEntries.Sort((a, b) => b.Value.CompareTo(a.Value));

//         if (leaderboardEntries.Count > 10)
//         {
//             leaderboardEntries = leaderboardEntries.GetRange(0, 10);
//         }

//         foreach (Transform child in leaderboardContainer)
//         {
//             Destroy(child.gameObject);
//         }

//         int rank = 1;
//         foreach (var entry in leaderboardEntries)
//         {
//             GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContainer);
//             row.SetActive(true);

//             TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);
//             foreach (var text in texts)
//             {
//                 text.gameObject.SetActive(true);
//                 text.enabled = true;
//             }

//             if (texts.Length >= 3)
//             {
//                 texts[0].text = rank.ToString();   // Rank
//                 texts[1].text = entry.Key;         // Player Name
//                 texts[2].text = entry.Value.ToString(); // Score
//             }

//             rank++;
//         }
//     });
// }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "playerName", playerName },
            { "score", score }
        };
    }
}