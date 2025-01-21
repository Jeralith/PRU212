using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    private BoxCollider2D _platform;
    void Start()
    {
        _platform = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y > _platform.transform.position.y)
        {
            _platform.enabled = true;
        }
        else _platform.enabled = false;
    }
}
