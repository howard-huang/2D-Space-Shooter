using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefabs; //1 = Diag Left, 2 = Diag Right, 3,4,5 = Dodge 6 = Missile Enemy, 7 = Ramming Enemy, 8 = Rear Fire;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups; //0 = Triple Shot, 1 = Speed, 2 = Shields, 3 = Ammo, 4 = Health, 5 = SuperLaser, 6 = Missiles, 7 = Stall;
    [SerializeField]
    private GameObject _powerupContainer;

    private List<GameObject> _commonPowerup = new List<GameObject>(); //50 - 100
    private List<GameObject> _uncommonPowerup = new List<GameObject>(); //16 - 49
    private List<GameObject> _rarePowerup = new List<GameObject>(); //0 - 15

    private bool _stopSpawn;

    [SerializeField]
    private AK.Wwise.Event _clearEnemyAudio;

    private void Start()
    {
        AssignPowerupRarity();
    }

    private void AssignPowerupRarity()
    {
        foreach (GameObject obj in _powerups)
        {
            Powerup _script = obj.GetComponent<Powerup>();
            
            if (_script != null)
            {
                int _rarity = _script.GetRarity();

                switch (_rarity)
                {
                    case 0:
                        _commonPowerup.Add(obj);
                        break;
                    case 1:
                        _uncommonPowerup.Add(obj);
                        break;
                    case 2:
                        _rarePowerup.Add(obj);
                        break;
                    default:
                        Debug.LogError("No Rarity" + obj);
                        break;
                }
            }
        }
    }

    public void StartSpawning(int _waveID)
    {
        _stopSpawn = false;
        GetWaveInfo(_waveID);
    }

    public void StopSpawning()
    {
        _stopSpawn = true;
        ClearEnemies();
    }

    private void GetWaveInfo(int _waveID)
    {
        WaitForSeconds _respawnTime = new WaitForSeconds(10);
        int _minEnemyPool = 0;
        int _maxEnemyPool = _enemyPrefabs.Length; //Int = Exclusive

        switch (_waveID)
        {
            case 1:
                _minEnemyPool = 0;
                _maxEnemyPool = 1;
                _respawnTime = new WaitForSeconds(5);   //Standard Enemy every 5 sec
                break;
            case 2:
                _minEnemyPool = 0;
                _maxEnemyPool = 3;
                _respawnTime = new WaitForSeconds(5);   //Standard + Rotation every 5 sec
                break;
            case 3:
                _minEnemyPool = 1;
                _maxEnemyPool = 4;
                _respawnTime = new WaitForSeconds(5);   //St Rotate + Dodge every 5 sec
                break;
            case 4:
                _minEnemyPool = 0;
                _maxEnemyPool = 6;
                _respawnTime = new WaitForSeconds(5);   //Standard + Dodge variations every 5 sec
                break;
            case 5:
                _minEnemyPool = 3;
                _maxEnemyPool = 7;
                _respawnTime = new WaitForSeconds(5);   //Dodgers + Missile every 5 sec
                break;
            case 6:
                _minEnemyPool = 0;
                _maxEnemyPool = 6;
                _respawnTime = new WaitForSeconds(3);   //Standard + Dodge every 3 sec
                break;
            case 7:
                _minEnemyPool = 3;
                _maxEnemyPool = 8;
                _respawnTime = new WaitForSeconds(5);   //Dodge thru Rammer every 5 sec
                break;
            case 8:
                _minEnemyPool = 6;
                _maxEnemyPool = 9;
                _respawnTime = new WaitForSeconds(5);   //Missile Ram and RearFire every 5 sec
                break;
            case 9:
                _minEnemyPool = 0;
                _maxEnemyPool = 6;
                _respawnTime = new WaitForSeconds(3);   //Standard + Dodge every 3 sec
                break;
            case 10:
                _minEnemyPool = 0;
                _maxEnemyPool = 9;
                _respawnTime = new WaitForSeconds(5);   //All of them every 5 sec
                break;
            case 11:
                SpawnBoss();    
                break;
        }

        if (_waveID <= 10)
        {
            StartCoroutine(SpawnEnemyRoutine(_respawnTime, _minEnemyPool, _maxEnemyPool));
        }

        StartCoroutine(SpawnPowerupRoutine());
    }

    private IEnumerator SpawnEnemyRoutine(WaitForSeconds _respawnTime, int _minEnemyPool, int _maxEnemyPool)
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawn == false)
        {
            int _randomEnemy = Random.Range(_minEnemyPool, _maxEnemyPool);
            Vector3 _enemySpawnPos = GetEnemySpawnPos(_randomEnemy);

            GameObject _enemy = Instantiate(_enemyPrefabs[_randomEnemy], _enemySpawnPos, Quaternion.identity);

            Enemy _enemyScript = _enemy.GetComponent<Enemy>();
            _enemyScript.SetID(_randomEnemy);
            _enemy.transform.parent = _enemyContainer.transform;

            yield return _respawnTime;
        }
    }

    private Vector3 GetEnemySpawnPos(int _enemyID)
    {
        float _xSpawnPos;
        float _ySpawnPos;
        Vector3 _enemySpawnPos;

        switch (_enemyID) //1 & 4  = Diagonal Left // 2 & 5 = Diagonal Right
        {
            case 1:
            case 4:
                _xSpawnPos = Random.Range(-13, -10);
                _ySpawnPos = Random.Range(4, 8);
                _enemySpawnPos = new Vector3(_xSpawnPos, _ySpawnPos, 0f);
                break;

            case 2:
            case 5:
                _xSpawnPos = Random.Range(13, 10);
                _ySpawnPos = Random.Range(4, 8);
                _enemySpawnPos = new Vector3(_xSpawnPos, _ySpawnPos, 0f);
                break;

            default:
                _xSpawnPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
                _enemySpawnPos = new Vector3(_xSpawnPos, 6.5f, 0f);
                break;
        }
        return _enemySpawnPos;
    }

    private void SpawnBoss()
    {
        Instantiate(_bossPrefab, transform.position, Quaternion.identity);
    }

    private void StopBoss()
    {
        if (GameObject.FindGameObjectWithTag("Boss") != null)
        {
            Boss _boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
            _boss.Clear();
        }
    }

    private void ClearEnemies()
    {
        Enemy[] _activeEnemies = _enemyContainer.GetComponentsInChildren<Enemy>();

        if (_activeEnemies != null)
        {
            _clearEnemyAudio.Post(this.gameObject);

            foreach (Enemy _enemy in _activeEnemies)
            {
                _enemy.ClearField();
            }
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawn == false)
        {
            float _xSpawnPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            Vector3 _powerupSpawnPos = new Vector3(_xSpawnPos, 6.5f, 0f);

            List<GameObject> _currentRarity = GetPowerupRarity();
            GameObject _randomPowerup = _currentRarity[Random.Range(0, _currentRarity.Count)];

            GameObject _powerup = Instantiate(_randomPowerup, _powerupSpawnPos, Quaternion.identity);
            _powerup.transform.parent = _powerupContainer.transform;

            yield return new WaitForSeconds(Random.Range(4.0f, 7.0f));
        }
    }

    private List<GameObject> GetPowerupRarity()
    {
        int _currentRarity = Random.Range(0, 100);

        if (_currentRarity > 0 && _currentRarity <= 15)
        {
            return _rarePowerup; 
        }
        else if (_currentRarity > 15 && _currentRarity <= 50)
        {
            return _uncommonPowerup;
        }
        else
        {
            return _commonPowerup;
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawn = true;

        ClearEnemies();
        StopBoss();
    }

    public void OnWin()
    {
        _stopSpawn = true;
    }
}
