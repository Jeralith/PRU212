using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject exitConfirm;
    public GameObject player;
    private PlayerMovement _player;
    public bool isPaused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
        _player = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0.2f;
        isPaused = true;
        _player.active = false;
        
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = _player.timeScale;
        isPaused = false;
        _player.active = true;
    }

    public void QuitConfirm()
    {
        exitConfirm.SetActive(true);        
        pauseMenu.SetActive(false);
    }
}
