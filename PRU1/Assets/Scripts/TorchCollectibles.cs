using System.Collections;
using UnityEngine;
using TMPro;

public class TorchCollectibles : MonoBehaviour
{
    [SerializeField] private TorchesStorage torchStorage;
    [SerializeField] private TextMeshProUGUI torchText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(CollectTorchEffect());
        }
    }

    private IEnumerator CollectTorchEffect()
    {
        // Lưu màu gốc
        Color originalColor = torchText.color;

        // Nhấp nháy màu đỏ-cam 2 lần
        for (int i = 0; i < 2; i++)
        {
            torchText.color = new Color(1f, 0.4f, 0f); // Cam đỏ
            yield return new WaitForSeconds(0.1f);
            torchText.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        torchStorage.AddTorch();
        Destroy(gameObject);
    }
}
