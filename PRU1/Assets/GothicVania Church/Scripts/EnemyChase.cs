using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 10f;
    public float floatAmplitude = 0.7f;
    public float floatSpeed = 3f;
    public float detectionRange = 2f;
    public float obstacleAvoidanceForce = 5f;
    public LayerMask obstacleLayer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float verticalOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        Vector2 targetPosition = new Vector2(player.position.x, player.position.y + verticalOffset);

        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        if (IsPathBlocked(out RaycastHit2D hit))
        {
            moveDirection += Vector2.up * obstacleAvoidanceForce;
        }

        rb.linearVelocity = moveDirection * speed;

        if (rb.linearVelocity.x > 0)
            transform.localScale = new Vector3(3, 3, 3);
        else if (rb.linearVelocity.x < 0)
            transform.localScale = new Vector3(-3, 3, 3);
    }

    bool IsPathBlocked(out RaycastHit2D hit)
    {
        hit = Physics2D.Raycast(transform.position, transform.right * Mathf.Sign(rb.linearVelocity.x), detectionRange, obstacleLayer);
        return hit.collider != null;
    }
}
