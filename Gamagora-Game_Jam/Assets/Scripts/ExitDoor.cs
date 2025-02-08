using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private FadeInOut _fade; // Fade pour la transition
    [SerializeField] private Camera _camera; // Camera principale
    [SerializeField] private float targetOrthographicSize = 5f; // Taille finale de la caméra (pour une caméra 2D)
    [SerializeField] private string nextSceneName = "Level2"; // Nom de la prochaine scène

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player)
        {
            StartCoroutine(ChangeScene());
        }
    }

    public IEnumerator ChangeScene()
    {
        // Assurez-vous que le fade commence d'abord
        _fade.FadeIn();

        // Valeur de départ de la taille de la caméra (orthographique)
        float startOrthographicSize = _camera.orthographicSize;

        // Durée du fade
        float fadeDuration = _fade.timeToFade;

        // Le temps écoulé pour le zoom et le fade
        float elapsedTime = 0f;

        // Zoom et fade progressifs
        while (elapsedTime < fadeDuration)
        {
            // Calcul du pourcentage du temps écoulé pour le fade
            float progress = elapsedTime / fadeDuration;

            // Applique un zoom progressif sur la taille orthographique
            _camera.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, progress);

            // On attend la prochaine frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Une fois la durée du fade et du zoom terminée, assure-toi que la taille finale est appliquée
        _camera.orthographicSize = targetOrthographicSize;

        // Charger la scène suivante
        SceneManager.LoadScene(nextSceneName);
    }
}
