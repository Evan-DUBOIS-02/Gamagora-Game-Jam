using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform minPos;
    public Transform maxPos;
    public int currentDir = 1;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (currentDir == 1)
            transform.position = minPos.position;
        else
            transform.position = maxPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 dir = (maxPos.position - minPos.position).normalized;
        transform.position += dir * speed * Time.deltaTime * currentDir;

        if (currentDir == 1)
        {
            float currentDistFromMin = Vector3.Distance(transform.position, minPos.position);
            if (currentDistFromMin >= Vector3.Distance(minPos.position, maxPos.position))
                currentDir = -1;
        }
        else
        {
            float currentDistFromMax = Vector3.Distance(transform.position, maxPos.position);
            if (currentDistFromMax >= Vector3.Distance(minPos.position, maxPos.position))
                currentDir = 1;
        }
    }
}
