using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBehaviour : MonoBehaviour
{
    [Header("Player")]
    private Transform player;
    [SerializeField] private LayerMask obstacleMask;

    // Life time
    [Header("Die")]
    [SerializeField] private bool _die;
    [SerializeField] public float _lifeTime;
    [SerializeField] private float _lifeTimePercentReducing;
    private float _currentLifeTime = 0f;
    private float _initialOuterRange;
    private float _initialInnerRangeDiff;

    // Rythme
    [Header("Rythme")]
    [SerializeField] private List<float> _rythme = new List<float>();
    private int _currentRythmeIndex = 0;
    private float _currentRythmeTime;

    // Light
    private Light2D _light;

    public bool isPlayerInLight = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        _light = GetComponent<Light2D>();
        _initialOuterRange = _light.pointLightOuterRadius;
        _initialInnerRangeDiff = _light.pointLightOuterRadius - _light.pointLightInnerRadius;
    }

    void FixedUpdate()
    {
        // Die check
        _currentLifeTime += Time.deltaTime;
        if (_die)
        {
            float lifePercentage = _currentLifeTime / _lifeTime;
            if(lifePercentage >= _lifeTimePercentReducing)
            {
                float fadeFactor = (1 - lifePercentage) / (1 - _lifeTimePercentReducing); 
                _light.pointLightOuterRadius = Mathf.Lerp(0, _initialOuterRange, fadeFactor);
                _light.pointLightInnerRadius = _light.pointLightOuterRadius - _initialInnerRangeDiff;
            }
            if(_currentLifeTime >= _lifeTime)
                Destroy(gameObject);
        }
            

        // Rythme check
        _currentRythmeTime += Time.deltaTime;
        if (_rythme.Count > 0)
        {
            if (_currentRythmeIndex >= _rythme.Count)
                _currentRythmeIndex = 0;

            if(_currentRythmeTime >= _rythme[_currentRythmeIndex])
            {
                _light.intensity = _currentRythmeIndex % 2;
                _currentRythmeIndex++;
                _currentRythmeTime = 0;
            }
        }

        if(IsPlayerInLight() && _light.intensity > 0)
        {
            isPlayerInLight = true;
        }
        else
        {
            isPlayerInLight = false;
        }
    }

    private bool IsPlayerInLight()
    {
        Vector3 lightPosition = _light.transform.position;
        Vector3 playerPosition = player.position;
        Vector3 toPlayer = (playerPosition - lightPosition).normalized;

        float distance = Vector3.Distance(lightPosition, playerPosition);

        // Vérifier si le joueur est dans le rayon de la lumière
        if (distance > _light.pointLightOuterRadius) return false;
        
        Vector3 lightDirection = _light.transform.up;
        float angleToPlayer = Vector3.Angle(lightDirection, toPlayer);

        float innerAngle = _light.pointLightInnerAngle; // Inner
        float outerAngle = _light.pointLightOuterAngle; // Outer

        // Vérifier si le joueur est dans le cône de lumière
        if (angleToPlayer > outerAngle * 0.5f) return false;

        RaycastHit2D hit = Physics2D.Raycast(lightPosition, toPlayer, distance, obstacleMask);
        if (hit.collider != null) return false;

        // Si entre inner et outer, faire une interpolation
        if (angleToPlayer <= innerAngle * 0.5f) return true;

        // Zone de transition entre Inner et Outer
        float t = (angleToPlayer - (innerAngle * 0.5f)) / ((outerAngle * 0.5f) - (innerAngle * 0.5f));
        return Random.value > t; // Plus il est proche du bord, moins il est détecté
    }
}
