using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject exitConfirm;
    public GameObject optionsMenu;
    public GameObject player;
    private PlayerMovement _player;
    [SerializeField] private GameObject _backgroundMusic;
    private AudioLowPassFilter _lowPass;
    public bool isPaused;
    [SerializeField] private SpeedrunTimer timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //pauseMenu.SetActive(false);
        _player = player.GetComponent<PlayerMovement>();
        _lowPass = _backgroundMusic.GetComponent<AudioLowPassFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        exitConfirm.SetActive(false);
        optionsMenu.SetActive(false);
        timer.StopTimer();
        _lowPass.cutoffFrequency = 300f;
        Time.timeScale = 0f;
        isPaused = true;
        _player.active = false;

    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        exitConfirm.SetActive(false);
        optionsMenu.SetActive(false);
        _lowPass.cutoffFrequency = 22000f;
        Time.timeScale = _player.timeScale;
        isPaused = false;
        _player.active = true;
        timer.ResumeTimer();
    }
    public void OpenOptions()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void QuitConfirm()
    {
        exitConfirm.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void OpenQuitConfirm()
    {
        pauseMenu.SetActive(false);
        exitConfirm.SetActive(true);
    }
    public void Return()
    {
        exitConfirm.SetActive(false);
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        //_player.active = false;

    }
    public void Restart()
    {
        _player.Die();
        ResumeGame();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
