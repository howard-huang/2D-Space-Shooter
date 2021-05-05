using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups; //0 = Triple Shot, 1 = Speed, 2 = Shields
    [SerializeField]
    private GameObject _powerupContainer;

    private bool _stopSpawn;

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
            int randomPowerup = Random.Range(0, _powerups.Length);

            GameObject _powerup = Instantiate(_powerups[randomPowerup], _powerupSpawnPos, Quaternion.identity);
            _powerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawn = true;
    }
}
