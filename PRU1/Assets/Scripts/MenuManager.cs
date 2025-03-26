using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] TextMeshProUGUI SystemText;
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
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        if (gameManager != null && UpdatedName == false)
        {
            PlayerName.text = gameManager.playerName;
            SystemText.text = "Welcome " + gameManager.playerName;
            print("Welcome " + gameManager.playerName); //console
            UpdatedName = true;
        }
    }
}
