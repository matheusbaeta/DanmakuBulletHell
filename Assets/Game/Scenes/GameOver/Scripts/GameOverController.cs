using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    private GameObject _gameOver;
    [SerializeField] private float _waitingTime = 0.5f;
    private string _constantGameOverObject = "GameOver";
    private void Start()
    {
        _gameOver = GameObject.Find(_constantGameOverObject);
        StartCoroutine(GameOverBlinkText());
    }

    private void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            SceneManager.LoadScene("MainMenu");
        }

        if(Mouse.current != null && Mouse.current.press.wasPressedThisFrame)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator GameOverBlinkText()
    {
        while (_gameOver)
        {
            _gameOver.SetActive(!_gameOver.activeInHierarchy);
            yield return new WaitForSeconds(_waitingTime);
        }
        yield break;
    }
}
