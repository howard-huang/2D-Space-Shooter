using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _ammoText;
    private int _ammoTotal;
    [SerializeField]
    private GameObject _ammoDisplay;

    [SerializeField]
    private Image _livesDisplay;
    [SerializeField]
    private Sprite[] _livesSprites;

    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;

    [SerializeField]
    private WaitForSeconds _flickerTime = new WaitForSeconds(0.5f);

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
    }

    public void UpdateScoreUI(int _score)
    {
        _scoreText.text = "Score: " + _score.ToString();
    }

    public void UpdateAmmoUI(int _ammoCount, int _maxAmmo)
    {
        _ammoTotal = _ammoCount;
        _ammoText.text = "Ammo: " + _ammoTotal.ToString() + " / " + _maxAmmo.ToString();

        if (_ammoTotal == 0)
        {
            StartCoroutine(NoAmmoRoutine());
        }
    }

    private IEnumerator NoAmmoRoutine()
    {
        while (_ammoTotal == 0)
        {
            _ammoDisplay.SetActive(false);
            yield return _flickerTime;
            _ammoDisplay.SetActive(true);
            yield return _flickerTime;
        }
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
            yield return _flickerTime;
            _gameOverText.gameObject.SetActive(false);
            yield return _flickerTime;
        }
    }

    private IEnumerator RestartGameDisplay()
    {
        yield return new WaitForSeconds(3.0f);
        _restartText.gameObject.SetActive(true);
        _gameManager.GMGameOver();
    }
}
