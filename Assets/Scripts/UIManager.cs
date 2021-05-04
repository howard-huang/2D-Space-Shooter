using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Image _livesDisplay;
    [SerializeField]
    private Sprite[] _livesSprites;

    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    private bool _gameOver;

    private void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
    }

    private void Update()
    {
        if (_gameOver == true && Input.GetKeyDown(KeyCode.R))
        {
            Scene _scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(_scene.name);
        }
    }

    public void UpdateScoreUI(int _score)
    {
        _scoreText.text = "Score: " + _score.ToString();
    }

    public void UpdateLivesUI(int _currentLives)
    {
        _livesDisplay.sprite = _livesSprites[_currentLives];

        if (_currentLives == 0)
        {
            _gameOver = true;
            StartCoroutine(GameOverFlickerRoutine());
            _restartText.gameObject.SetActive(true);
        }
    }

    private IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
