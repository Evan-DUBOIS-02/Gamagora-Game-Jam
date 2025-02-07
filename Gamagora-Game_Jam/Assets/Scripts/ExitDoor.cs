using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private FadeInOut _fade;

    public IEnumerator ChangeScene()
    {
        _fade.FadeIn();
        yield return new WaitForSeconds(_fade.timeToFade);
        SceneManager.LoadScene("Level2");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == _player)
        {
            StartCoroutine(ChangeScene());
        }
    }
}
