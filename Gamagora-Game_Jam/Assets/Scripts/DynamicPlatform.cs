using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
public class DynamicPlatform : MonoBehaviour
{
    public Light2D[] lightSource; // La lumière à vérifier
    public LayerMask obstacleMask; // Masque pour les obstacles
    public int segments = 10; // Nombre de divisions de la plateforme

    private BoxCollider2D[] colliders;
    private float platformWidth;

    

    void Start()
    {
        platformWidth = transform.localScale.x;
        GenerateColliders();
    }

    void Update()
    {
        UpdateColliders();
    }

    

    void GenerateColliders()
    {
        colliders = new BoxCollider2D[segments];

        float segmentWidth = platformWidth / segments;

        for (int i = 0; i < segments; i++)
        {
            GameObject segment = new GameObject("PlatformSegment_" + i);
            segment.transform.parent = transform;
            segment.transform.localPosition = new Vector3(-platformWidth / 2 + segmentWidth * (i + 0.5f), 0, 0);

            BoxCollider2D col = segment.AddComponent<BoxCollider2D>();
            col.size = new Vector2(segmentWidth, transform.localScale.y);
            colliders[i] = col;
        }
    }

    void UpdateColliders()
    {
        for(int i = 0; i < segments; i++)
        {
            bool isActive = false;
            for (int j = 0; j < lightSource.Length; j++)
            {
                if (lightSource[j].intensity <= 0) break;
                Vector3 checkPosition = colliders[i].transform.position;
                if (IsColliderInLight(lightSource[j], checkPosition))
                {
                    isActive = true;
                    break;
                }
            }
            colliders[i].enabled = isActive;
        }
    }

    private bool IsColliderInLight(Light2D light, Vector3 colPosition)
    {
        Vector3 lightPosition = light.transform.position;
        Vector3 toCol = (colPosition - lightPosition).normalized;

        float distance = Vector3.Distance(lightPosition, colPosition);

        if (distance > light.pointLightOuterRadius) return false;

        Vector3 lightDirection = light.transform.up;
        float angleToCol = Vector3.Angle(lightDirection, toCol);

        float innerAngle = light.pointLightInnerAngle;
        float outerAngle = light.pointLightOuterAngle;

        if (angleToCol > outerAngle * 0.5f) return false;

        RaycastHit2D hit = Physics2D.Raycast(lightPosition, toCol, distance, obstacleMask);
        if (hit.collider != null) return false;

        if (angleToCol <= innerAngle * 0.5f) return true;

        float t = (angleToCol - (innerAngle * 0.5f)) / ((outerAngle * 0.5f) - (innerAngle * 0.5f));
        return Random.value > t;
    }
}
