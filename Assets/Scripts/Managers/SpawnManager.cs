using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups; //0 = Triple Shot, 1 = Speed, 2 = Shields, 3 = Ammo, 4 = Health, 5 = SuperLaser 6 = Missiles;
    [SerializeField]
    private GameObject _powerupContainer;

    private List<GameObject> _common = new List<GameObject>(); //50 - 100
    private List<GameObject> _uncommon = new List<GameObject>(); //16 - 49
    private List<GameObject> _rare = new List<GameObject>(); //0 - 15

    private bool _stopSpawn;

    private void Start()
    {
        AssignRarity();
    }

    private void AssignRarity()
    {
        foreach (GameObject obj in _powerups)
        {
            Powerup _script = obj.GetComponent<Powerup>();
            int _rarity = _script.GetRarity();

            switch (_rarity)
            {
                case 0:
                    _common.Add(obj);
                    break;
                case 1:
                    _uncommon.Add(obj);
                    break;
                case 2:
                    _rare.Add(obj);
                    break;
                default:
                    Debug.LogError("No Rarity" + obj);
                    break;
            }          
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawn == false)
        {
            float _xSpawnPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            Vector3 _enemySpawnPos = new Vector3(_xSpawnPos, 6.5f, 0f);

            GameObject _enemy = Instantiate(_enemyPrefab, _enemySpawnPos, Quaternion.identity);
            _enemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawn == false)
        {
            float _xSpawnPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            Vector3 _powerupSpawnPos = new Vector3(_xSpawnPos, 6.5f, 0f);

            List<GameObject> _currentRarity = GetRarity();
            GameObject _randomPowerup = _currentRarity[Random.Range(0, _currentRarity.Count)];

            GameObject _powerup = Instantiate(_randomPowerup, _powerupSpawnPos, Quaternion.identity);
            _powerup.transform.parent = _powerupContainer.transform;

            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        }
    }

    private List<GameObject> GetRarity()
    {
        int _currentRarity = Random.Range(0, 100);

        if (_currentRarity > 0 && _currentRarity <= 15)
        {
            return _rare; 
        }
        else if (_currentRarity > 15 && _currentRarity <= 50)
        {
            return _uncommon;
        }
        else
        {
            return _common;
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawn = true;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies)
        {
            Destroy(item);
        }
    }
}
