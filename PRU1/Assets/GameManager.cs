using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void SaveTestScore()
    {
        LeaderboardManager leaderboard = FindFirstObjectByType<LeaderboardManager>();
        leaderboard.SubmitScore("TestPlayer", Random.Range(0, 1000));
        
    }
}
