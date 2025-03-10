using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadGameScene(int levelIndex)
    {
        string sceneName = "";

        switch (levelIndex)
        {
            case 1:
                sceneName = "TuanAnhLevels";
                break;
            case 2:
                sceneName = "NgocAnhLevels";
                break;
            case 3:
                sceneName = "TuanAnhLevels";
                break;
            case 4:
                sceneName = "TuanAnhLevels";
                break;
            case 5:
                sceneName = "TuanAnhLevels";
                break;
            default:
                Debug.LogError("Level không tồn tại!");
                return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
