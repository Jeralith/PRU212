using UnityEngine;

public class WallCollision : MonoBehaviour
{
    public bool isWalled;
    [SerializeField] private GameObject _player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            var player = _player.GetComponent<PlayerMovement>();
            
            isWalled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            var player = _player.GetComponent<PlayerMovement>();
            
            isWalled = false;
        }
    }
}
