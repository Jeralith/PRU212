using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public Transform respawnPoint;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            var rb = player.GetComponent<Rigidbody2D>();

            player.transform.position = respawnPoint.position;

        }
    }
}
