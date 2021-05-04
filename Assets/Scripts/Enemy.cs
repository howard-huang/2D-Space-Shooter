using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    [SerializeField]
    private int _points = 10;

    private Player _player;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6.5)
        {
            float _randXPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            transform.position = new Vector3(_randXPos, 6.5f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.TakeDamage();
            }

            Destroy(this.gameObject);
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            
            if (_player != null)
            {
                _player.AddScore(_points);
            }

            Destroy(this.gameObject);
        }
    }
}
