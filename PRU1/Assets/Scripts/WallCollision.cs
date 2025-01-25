using UnityEngine;

public class WallCollision : MonoBehaviour
{
    
    [SerializeField] private GameObject _player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            var player = _player.GetComponent<PlayerMovement>();
            
            player.isWalled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            var player = _player.GetComponent<PlayerMovement>();
            
            player.isWalled = false;
        }
    }
}
