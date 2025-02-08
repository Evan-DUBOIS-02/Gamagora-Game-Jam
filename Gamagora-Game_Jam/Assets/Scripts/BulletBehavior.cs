using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] public float physicsFlareSpeed = 15f;
    [SerializeField] public float physicsFlareGravity = 3f;
    private float destroyTime;
    private Rigidbody2D rb;

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
        transform.right = rb.linearVelocity;
    }

    private void SetPhysicsVelocity()
    {
        rb.linearVelocity = transform.right * physicsFlareSpeed;
    }

    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }
}
