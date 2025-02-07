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
        FadeOut();
    }

    void Update()
    {
        if(fadeIn)
        {
            if(_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += timeToFade * Time.deltaTime;
                if(_canvasGroup.alpha >= 1 )
                {
                    fadeIn = false;
                }
            }
        }

        if (fadeOut)
        {
            if (_canvasGroup.alpha >= 0)
            {
                _canvasGroup.alpha -= timeToFade * Time.deltaTime;
                if (_canvasGroup.alpha == 0)
                {
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
