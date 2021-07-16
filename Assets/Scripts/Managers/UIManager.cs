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
    private GameObject _waveDisplay;
    [SerializeField]
    private Text _waveIDText;
    [SerializeField]
    private Text _waveTimerText;
    private bool _waveEnded;

    [SerializeField]
    private GameObject _bossDisplay;
    [SerializeField]
    private Slider _bossSlider;

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
    private Slider _thrusterSlider;
    [SerializeField]
    private Slider _magnetSlider;

    private bool _engineStalled;
    [SerializeField]
    private GameObject _stallText;
    [SerializeField]
    private GameObject _engineFixedText;

    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _winText;
    [SerializeField]
    private Text _restartText;

    [SerializeField]
    private WaitForSeconds _flickerTime = new WaitForSeconds(0.5f);

    private GameManager _gameManager;
    private AudioManager _audioManager;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is Null!");
        }

        if (_audioManager == null)
        {
            Debug.LogError("AudioManager is Null!");
        }

        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
    }

    public void UpdateWaveID(int _waveID)
    {
        if (_waveID >= 11)
        {
            _waveEnded = false;
            _waveIDText.gameObject.SetActive(false);
            _waveTimerText.gameObject.SetActive(false);
            _bossDisplay.SetActive(true);
        }
        else
        {
            _waveDisplay.SetActive(true);
            _waveIDText.text = "Wave " + _waveID.ToString();
        }
    }

    public void UpdateWaveTime(float _seconds)
    {       
        float _time = Mathf.RoundToInt(_seconds);
        _waveTimerText.text = _time.ToString();

        if (_time > 0)
        {
            _waveEnded = false;
        }
        else
        {
            _waveEnded = true;
            StartCoroutine(WaveFlickerRoutine());
        }
    }

    private IEnumerator WaveFlickerRoutine()
    {
        while (_waveEnded == true)
        {
            _waveDisplay.SetActive(false);
            yield return _flickerTime;
            _waveDisplay.SetActive(true);
            yield return _flickerTime;
        }
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

    public void EngineDisplayUI(bool _hasStalled)
    {
        _engineStalled = _hasStalled;

        if (_engineStalled == true)
        {
            StartCoroutine(StallTextFlickerRoutine());
        }
        else if (_engineStalled == false)
        {
            StartCoroutine(EngineFixedFlickerRoutine());
        }
    }

    private IEnumerator StallTextFlickerRoutine()
    {
        while (_engineStalled == true)
        {
            _stallText.SetActive(true);
            yield return _flickerTime;
            _stallText.SetActive(false);
            yield return _flickerTime;
        }
    }

    private IEnumerator EngineFixedFlickerRoutine()
    {
        for (int i = 0; i < 2; i++) //Flickers Twice
        {
            _engineFixedText.SetActive(true);
            yield return _flickerTime;
            _engineFixedText.SetActive(false);
            yield return _flickerTime;
        }
    }

    public void UpdateLivesUI(int _currentLives)
    {
        _livesDisplay.sprite = _livesSprites[_currentLives];

        if (_currentLives == 0)
        {
            StopAllCoroutines();
            UIGameOver();
        }
    }

    public void UpdateThrusterUI(float _timeLeft)
    {
        _thrusterSlider.value = _timeLeft;
    }

    public void UpdateMagnetUI(float _timeLeft)
    {
        _magnetSlider.value = _timeLeft;
    }

    public void UpdateBossUI(int _bossHealth)
    {
        _bossSlider.value = _bossHealth;
    }

    private void UIGameOver()
    {
        StartCoroutine(WaveFlickerRoutine());
        StartCoroutine(GameOverFlickerRoutine());
        StartCoroutine(RestartGameDisplay());
    }

    public void UIGameWon()
    {
        _winText.gameObject.SetActive(true);
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
        _audioManager.AMGameOver();
    }
}
