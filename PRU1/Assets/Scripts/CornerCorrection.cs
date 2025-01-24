
using UnityEngine;

public class CornerCorrection : MonoBehaviour
{
    private BoxCollider2D _hitbox;
    [SerializeField] private GameObject _player;

    private void Awake()
    {
        _hitbox = gameObject.GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        var wall = other.GetComponent<BoxCollider2D>();
        var player = _player.GetComponent<PlayerMovement>();
        if (wall != null)
        {
            player.CornerCorrection(other);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
         var wall = other.GetComponent<BoxCollider2D>();
        if (wall != null)
        {
            Debug.Log("Out");
        }
    }
}
