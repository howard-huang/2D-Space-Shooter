using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 3.0f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private GameManager _gameManager;
    private Collider2D _collider2D;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _collider2D = GetComponent<Collider2D>();

        if (_gameManager == null)
        {
            Debug.Log("Spawn Manager is Null!");
        }

        if (_collider2D == null)
        {
            Debug.LogError("Collider is Null!");
        }
    }

    private void Update()
    {
        Vector3 rotation = new Vector3(0, 0, 1);
        transform.Rotate(rotation * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _collider2D.enabled = false;
            Destroy(other.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _gameManager.StartSpawning();
            Destroy(this.gameObject, 1.0f);
        }
    }
}
