using TMPro;
using UnityEngine;

public class TorchesStorage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI torchText;
    public int torchCount = 0;

    public void AddTorch()
    {
        torchCount++;
        torchText.text = "Storage: " + torchCount + "/9 Torches";
    }
}
