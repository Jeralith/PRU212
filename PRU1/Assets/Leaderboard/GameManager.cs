using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class GameManager : MonoBehaviour
{
    private bool isLoggedIn = false;

    void Start()
    {
        // Automatically log in for testing
        TestLogin();
    }

    public void TestLogin()
    {
        var request = new LoginWithCustomIDRequest 
        { 
            CustomId = "TestUser_" + UnityEngine.Random.Range(0, 10000), // Random ID to avoid duplicates
            CreateAccount = true 
        };

        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {
            Debug.Log("✅ Test Login Successful! PlayFab ID: " + result.PlayFabId);
            isLoggedIn = true;
        },
        error =>
        {
            Debug.LogError("❌ Login Failed: " + error.ErrorMessage);
            isLoggedIn = false;
        });
    }

    public void SaveTestScore()
    {
        if (!isLoggedIn)
        {
            Debug.LogError("⚠️ Must be logged in to submit a score!");
            return;
        }

        LeaderboardManager leaderboard = FindFirstObjectByType<LeaderboardManager>();
        if (leaderboard != null)
        {
            int randomScore = UnityEngine.Random.Range(0, 1000);
            leaderboard.SubmitScore("TestPlayer", randomScore);
            Debug.Log("🏆 Submitted Score: " + randomScore);
        }
        else
        {
            Debug.LogError("❌ LeaderboardManager not found in the scene.");
        }
    }
}
