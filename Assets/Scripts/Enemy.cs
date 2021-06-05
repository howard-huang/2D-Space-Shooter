using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    private float _speed = 4.0f;

    [Header("Score")]
    [SerializeField]
    private int _points = 10;

    [Header("Laser")]
    [SerializeField]
    private GameObject _laserPrefab;
    private Vector3 _laserOffset = new Vector3(0, -1.0f, 0);
    private GameObject _laserContainer;

    [Header("Audio")]
    [SerializeField]
    private AK.Wwise.Event _explosionSound;
    [SerializeField]
    private AK.Wwise.Event _laserSound;

    private Player _player;
    private Animator _anim;
    private Collider2D _collider2D;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = gameObject.GetComponent<Animator>();
        _collider2D = gameObject.GetComponent<Collider2D>();
        _laserContainer = GameObject.Find("LaserContainer");

        if (_player == null)
        {
            Debug.LogError("Player is Null!");
        }        
        if (_anim == null)
        {
            Debug.LogError("Enemy Animator is Null!");
        }
        if (_collider2D == null)
        {
            Debug.LogError("Enemy Collider2D is Null!");
        }
        if (_laserContainer == null)
        {
            Debug.LogError("Laser Container is Null!");
        }

        StartCoroutine(FireLaserCoroutine());
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Explosion") && transform.position.y <= -6.0)
        {
            Destroy(this.gameObject);
        }
        else if (transform.position.y < -8.0f)
        {
            float _randXPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            transform.position = new Vector3(_randXPos, 6.5f, 0f);
        }
    }

    private IEnumerator FireLaserCoroutine()
    {
        while (true)
        {
            Vector3 _laserPos = transform.position + _laserOffset;
            GameObject _laser = Instantiate(_laserPrefab, _laserPos, Quaternion.identity);
            _laserSound.Post(this.gameObject);
            
            _laser.tag = "Enemy Laser";
            _laser.transform.parent = _laserContainer.transform;

            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            _player.TakeDamage();
            EnemyDeath();
        }
        else if (other.tag == "Laser" || other.tag == "Missile")
        {
            Destroy(other.gameObject);

            _player.AddScore(_points);
            EnemyDeath();
        }
        else if (other.tag == "Super Laser")
        {
            _player.AddScore(_points);
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _collider2D.enabled = false;
        _explosionSound.Post(this.gameObject);

        float _animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(this.gameObject, _animLength);
    }


}
