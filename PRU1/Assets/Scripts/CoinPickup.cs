using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Object.FindFirstObjectByType<PlayerMovement>().AddScore(1); // or
            PlaySFXClip(coinPickupSFX);
            Destroy(gameObject);
        }
    }

    private void PlaySFXClip(AudioClip soundClip)
    {
        if (soundClip == null || SFXManager.instance == null) return;
        SFXManager.instance.PlaySFXClip(soundClip, transform, 1f);
    }
}
