using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    public float translationFactor = 20;

    void LateUpdate()
    {
        Vector3 position = target.position;
        position.z = -10f;
        if (transform.position != position)
        {
            transform.position += (position - transform.position) / translationFactor;
        }
    }
}
