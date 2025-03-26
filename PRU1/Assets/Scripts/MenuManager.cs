using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] TextMeshProUGUI SystemText;

    [Header("MainMenu")]
    [SerializeField] GameObject MainMenuPage;

    [Header("LevelsMenu")]
    [SerializeField] GameObject LevelsMenuPage;

    bool UpdatedName = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateName();
    }

    private void UpdateName()
    {
        if (!UpdatedName && PlayerProfile.Instance != null)
        {
            PlayerName.text = PlayerProfile.Instance.playerName;
            SystemText.text = "Welcome " + PlayerProfile.Instance.playerName;
            UpdatedName = true;
        }
    }

    #region Button Functions -> Load Game Scenes

    public void LoadGameLevel1()
    {
        SceneManager.LoadScene("TuanAnhLevels");
    }
    public void LoadGameLevel2()
    {
        SceneManager.LoadScene("demodeng");
    }
    public void LoadGameLevel3()
    {
        SceneManager.LoadScene("NgocAnhLevels");
    }
    public void LoadGameLevel4()
    {
        SceneManager.LoadScene("LeTan");
    }
    public void LoadGameLevel5()
    {
        SceneManager.LoadScene("TempleofChaos-Nam");
    }

    #endregion

    #region Button Functions -> Pages

    public void OpenMainMenuPage()
    {
        MainMenuPage.SetActive(true);
        LevelsMenuPage.SetActive(false);
    }
    public void OpenLevelsMenuPage()
    {
        MainMenuPage.SetActive(false);
        LevelsMenuPage.SetActive(true);
    }
    public void LogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerProfile.Instance.playerName = null;
        PlayerName.text = "";
        SystemText.text = "Log out!";
        UpdatedName = false;
        SceneManager.LoadScene("LoginScene");
    }
    public void LoadRankScene()
    {
        SceneManager.LoadScene("Leaderboard");
    }
    #endregion
    public void QuitGame()
    {
        Application.Quit();
    }
}
