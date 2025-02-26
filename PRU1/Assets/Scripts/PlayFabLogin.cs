using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLogin : MonoBehaviour
{
    void Start()
    {
        LoginWithDeviceID();
    }

    void LoginWithDeviceID()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Đăng nhập PlayFab thành công! PlayFab ID: " + result.PlayFabId);
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Lỗi đăng nhập: " + error.GenerateErrorReport());
    }
}
