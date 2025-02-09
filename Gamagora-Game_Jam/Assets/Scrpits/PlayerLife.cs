using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] public List<LightBehaviour> lights = new List<LightBehaviour>();
    [SerializeField] private float zoomSize;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float deZoomSpeed = 2f;
    [SerializeField] Image deathEffect;
    [SerializeField] private Transform start;
    [SerializeField] private float delayBeforeZoom;

    private float normalSize;
    private Coroutine zoomCoroutine;
    private bool isDead = false;
    private bool isZooming = false;
    public bool isWin = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normalSize = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(isWin)
        {
            StopZoom();
            return;
        }

        bool isInLight = false;
        lights.RemoveAll(x => x == null);
        foreach (var light in lights)
        {
            isInLight |= light.isPlayerInLight;
        }

        if (isInLight)
            StopZoom();
        else
            StartZoomWithDelay();

        if (isZooming)
        {
            
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomSize, Time.deltaTime * zoomSpeed);

            var colorTmp = deathEffect.color;
            colorTmp.a = Mathf.Lerp(colorTmp.a, 1, Time.deltaTime * zoomSpeed);
            deathEffect.color = colorTmp;

            if (cam.orthographicSize <= zoomSize + 0.1f) isDead = true;
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, normalSize, Time.deltaTime * deZoomSpeed);

            var colorTmp = deathEffect.color;
            colorTmp.a = Mathf.Lerp(colorTmp.a, 0, Time.deltaTime * deZoomSpeed);
            deathEffect.color = colorTmp;

            isDead = false;
        }

        if (isDead)
        {
            transform.position = start.position;
        }
    }

    public void StartZoomWithDelay()
    {
        if (zoomCoroutine == null) // �vite de lancer plusieurs fois la coroutine
        {
            zoomCoroutine = StartCoroutine(ZoomAfterDelay());
        }
    }

    public void StopZoom()
    {
        isZooming = false;
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }
    }

    private IEnumerator ZoomAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeZoom);
        isZooming = true;
    }
}
