using UnityEngine;

public class ExitConfirm : MonoBehaviour
{
    public GameObject exitConfirm;
    public GameObject pauseMenu;
    public bool isPaused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exitConfirm.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitConfirm.SetActive(false);
            Time.timeScale = 0.9f;
        }
    }
    public void OpenQuitConfirm()
    {
        pauseMenu.SetActive(false);
        exitConfirm.SetActive(true);
    }
    public void Return()
    {
        exitConfirm.SetActive(false);
        pauseMenu.SetActive(true);
        //_player.active = false;
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
