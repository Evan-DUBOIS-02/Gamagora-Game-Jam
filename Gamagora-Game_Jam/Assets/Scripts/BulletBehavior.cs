using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] public float physicsFlareSpeed = 15f;
    [SerializeField] public float physicsFlareGravity = 3f;
    private float destroyTime;
    private Rigidbody2D rb;
    private bool isMoving = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = physicsFlareGravity;

        destroyTime = GetComponentInChildren<LightBehaviour>()._lifeTime;
        SetDestroyTime();

        SetPhysicsVelocity();
    }

    private void FixedUpdate()
    {
        if (isMoving)
            transform.right = rb.linearVelocity;
        else
            rb.simulated = false;
    }

    private void SetPhysicsVelocity()
    {
        rb.linearVelocity = transform.right * physicsFlareSpeed;
    }

    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("InvisibleObstacle") || collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            transform.SetParent(collision.gameObject.transform);
            isMoving = false;
        }
    }
}
