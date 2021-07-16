using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver;

    [SerializeField]
    private int _waveID = 0;
    private float _waveTime = 20;

    [SerializeField]
    private WaitForSeconds _holdTime = new WaitForSeconds(5.0f);

    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is Null!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is Null!");
        }
    }

    public void StartSpawning()
    {
        _waveID++;
        _waveTime += 10;

        _uiManager.UpdateWaveID(_waveID);
        _uiManager.UpdateWaveTime(_waveTime);
        _spawnManager.StartSpawning(_waveID);

        if (_waveID <= 10)
        {
            StartCoroutine(WaveCountdown(_waveTime));
        }
    }

    private IEnumerator WaveCountdown(float _time)
    {
        while(_time > 0)
        {
            _time -= Time.deltaTime;
            _uiManager.UpdateWaveTime(_time);
            yield return new WaitForEndOfFrame();
        }
        _spawnManager.StopSpawning();

        yield return _holdTime;
        StartSpawning();
    }

    private void Update()
    {
        if (_isGameOver == true && Input.GetKeyDown(KeyCode.R))
        {
            Scene _scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(_scene.name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void GMGameOver()
    {
        StopAllCoroutines();
        _isGameOver = true;
    }

    public void GMGameWon()
    {
        StopAllCoroutines();
        _uiManager.UIGameWon();
        _spawnManager.OnWin();
        _isGameOver = true;
    }
}
