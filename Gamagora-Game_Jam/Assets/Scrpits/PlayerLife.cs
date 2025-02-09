using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private float timeBeforeDeath = 3f;
    [SerializeField] private Camera cam;
    [SerializeField] private LightBehaviour[] lights;
    [SerializeField] private float targetOrthographicSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isInLight = false;
        foreach (var light in lights)
        {
            isInLight |= light.isPlayerInLight;
        }

       /* if (isInLight)
            cam.orthographicSize = Mathf.Lerp();
        else*/
    }

    public IEnumerator ChangeScene()
    {
        // Valeur de d�part de la taille de la cam�ra (orthographique)
        float startOrthographicSize = cam.orthographicSize;

        // Dur�e du fade
        float fadeDuration = timeBeforeDeath;

        // Le temps �coul� pour le zoom et le fade
        float elapsedTime = 0f;

        // Zoom et fade progressifs
        while (elapsedTime < fadeDuration)
        {
            // Calcul du pourcentage du temps �coul� pour le fade
            float progress = elapsedTime / fadeDuration;

            // Applique un zoom progressif sur la taille orthographique
            cam.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, progress);

            // On attend la prochaine frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Une fois la dur�e du fade et du zoom termin�e, assure-toi que la taille finale est appliqu�e
        cam.orthographicSize = targetOrthographicSize;

        // Charger la sc�ne suivante
        SceneManager.LoadScene(nextSceneName);
    }
}
