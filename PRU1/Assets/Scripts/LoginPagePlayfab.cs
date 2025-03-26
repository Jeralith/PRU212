using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Collections;

public class LoginPagePlayfab : MonoBehaviour
{
    [SerializeField]
    private GameObject WelcomeObject;
    [SerializeField]
    private PlayerProfile playerProfile;
    [SerializeField]
    private TextMeshProUGUI WelcomeText;
    //[SerializeField] TextMeshProUGUI TopText;
    [SerializeField] TextMeshProUGUI MessageText;

    [Header("Login")]
    [SerializeField] TMP_InputField EmailInput;
    [SerializeField] TMP_InputField PassInput;
    [SerializeField] GameObject LoginPage;

    [Header("Register")]
    [SerializeField] TMP_InputField EmailRegisterInput;
    [SerializeField] TMP_InputField UsernameRegisterInput;
    [SerializeField] TMP_InputField PassRegisterInput;
    [SerializeField] GameObject RegisterPage;

    [Header("Recovery")]
    [SerializeField] TMP_InputField EmailRecoverInput;
    [SerializeField] GameObject RecoveryPage;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Button Functions
    public void RegisterUser()
    {
        // if statement if pass is les than 6 message text = Too short pass;
        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = UsernameRegisterInput.text,
            Email = EmailRegisterInput.text,
            Password = PassRegisterInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = EmailInput.text,
            Password = PassInput.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);

    }
    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = EmailRecoverInput.text,
            TitleId = "14D480"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySucces, OnErrorRecovery);
    }

    private void OnErrorRecovery(PlayFabError error)
    {
        MessageText.text = "No Email Found!";
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRecoverySucces(SendAccountRecoveryEmailResult result)
    {
        OpenLoginPage();
        MessageText.text = "Recovery Mail Sent";
    }

    private void OnLoginSuccess(LoginResult result)
    {
        string name = null;
        if (result.InfoResultPayload != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }
        
        WelcomeObject.SetActive(true);
        // write some good for getting user name
        WelcomeText.text = "Welcome " + name;

        if (PlayerProfile.Instance != null)
        {
            PlayerProfile.Instance.playerName = name;
        }
        StartCoroutine(LoadNextScene());
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        MessageText.text = "New account is created!";
        Debug.Log("New account is created!");
        OpenLoginPage();
    }

    private void OnError(PlayFabError error)
    {
        MessageText.text = error.ErrorMessage;
        Debug.LogError( error.GenerateErrorReport());
    }

    public void OpenLoginPage()
    {
        LoginPage.SetActive(true);
        RegisterPage.SetActive(false);
        RecoveryPage.SetActive(false);
    }
    public void OpenRegisterPage()
    {
        LoginPage.SetActive(false);
        RegisterPage.SetActive(true);
        RecoveryPage.SetActive(false);
    }
    public void OpenRecoveryPage()
    {
        LoginPage.SetActive(false);
        RegisterPage.SetActive(false);
        RecoveryPage.SetActive(true);
    }
    #endregion

    IEnumerator LoadNextScene()
    {
        MessageText.text = "Loggin in";
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MenuScene");
        Debug.Log("Loggin in!");
    }
}
