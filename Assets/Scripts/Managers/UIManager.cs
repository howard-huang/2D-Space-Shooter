using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is Null!");
        }

        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
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
            UIGameOver();
        }
    }

    private void UIGameOver()
    {
        StartCoroutine(GameOverFlickerRoutine());
        StartCoroutine(RestartGameDisplay());
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

    private IEnumerator RestartGameDisplay()
    {
        yield return new WaitForSeconds(3.0f);
        _restartText.gameObject.SetActive(true);
        _gameManager.GMGameOver();
    }
}
