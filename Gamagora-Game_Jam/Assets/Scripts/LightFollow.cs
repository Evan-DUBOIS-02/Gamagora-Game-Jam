using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFollow : MonoBehaviour
{
    public Material revealMaterial;
    public Light2D lightSource;

    void Update()
    {
        if (revealMaterial && lightSource)
        {
            revealMaterial.SetVector("_LightPos", lightSource.transform.position);
            revealMaterial.SetVector ("_LightDir", lightSource.transform.up);
            revealMaterial.SetFloat("_LightRadius", lightSource.pointLightOuterRadius);
            revealMaterial.SetFloat("_InnerAngle", lightSource.pointLightInnerAngle);
            revealMaterial.SetFloat("_OuterAngle", lightSource.pointLightOuterAngle);
        }
    }
}