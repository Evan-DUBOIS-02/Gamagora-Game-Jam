using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private bool fadeIn = false;
    private bool fadeOut = false;

    public float timeToFade;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1.0f;
        FadeOut();
    }

    void Update()
    {
        if(fadeIn)
        {
            if(_canvasGroup.alpha < 1f)
            {
                _canvasGroup.alpha += (1f / timeToFade) * Time.deltaTime;
                if (_canvasGroup.alpha >= 1 )
                {
                    _canvasGroup.alpha = 1f;
                    fadeIn = false;
                }
            }
        }

        if (fadeOut)
        {
            if (_canvasGroup.alpha >= 0)
            {
                _canvasGroup.alpha -= (1f / timeToFade) * Time.deltaTime;
                if (_canvasGroup.alpha <= 0)
                {
                    _canvasGroup.alpha = 0f;
                    fadeOut = false;
                }
            }
        }
    }

    public void FadeIn()
    {
        fadeIn = true;
    }

    public void FadeOut()
    {
        fadeOut = true;
    }
}
