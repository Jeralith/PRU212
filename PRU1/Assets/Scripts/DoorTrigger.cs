using System.Collections;
using TMPro;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private int requiredTorchCount = 6;
    [SerializeField] private GameObject blockTilemap;
    [SerializeField] private TorchesStorage torchStorage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (torchStorage.torchCount >= requiredTorchCount)
        {
            StartCoroutine(DestroyWithShake());
            
        }
        else
        {
            ShowMessage($"You need {requiredTorchCount} torches to open this door!");
        }
    }

    private IEnumerator DestroyWithShake()
    {
        Vector3 originalPos = blockTilemap.transform.position;
        float elapsed = 0f;
        float duration = 2f;
        float strength = 0.05f;

        while (elapsed < duration)
        {
            if (blockTilemap == null) yield break; // ← thêm dòng này để tránh lỗi

            float offsetX = Random.Range(-strength, strength);
            float offsetY = Random.Range(-strength, strength);
            blockTilemap.transform.position = originalPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (blockTilemap != null)
        {
            blockTilemap.transform.position = originalPos;
            Destroy(blockTilemap);
            Destroy(gameObject); // ← destroy luôn vùng trigger
        }
    }


    private void ShowMessage(string msg)
    {
        if (messageText == null) return;

        messageText.text = msg;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), messageDuration);
    }

    private void ClearMessage()
    {
        messageText.text = "";
    }
}
