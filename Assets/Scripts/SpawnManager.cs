using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawn;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (_stopSpawn == false)
        {
            float _xSpawnPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            transform.position = new Vector3(_xSpawnPos, 6.5f, 0f);

            GameObject _enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
            _enemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawn = true;
    }
}
