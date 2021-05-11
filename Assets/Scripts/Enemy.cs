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
    private Animator _anim;
    private Collider2D _collider2D;
    private AudioSource _audio;

    [SerializeField]
    private AudioClip _explosionSound;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = gameObject.GetComponent<Animator>();
        _collider2D = gameObject.GetComponent<Collider2D>();
        _audio = gameObject.GetComponent<AudioSource>();

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
        if (_audio == null)
        {
            Debug.LogError("Audio Source is Null!");
        }
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Explosion"))
        {
            Destroy(this.gameObject, _audio.clip.length);
        }
        else if (transform.position.y < -6.5)
        {
            float _randXPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            transform.position = new Vector3(_randXPos, 6.5f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            _player.TakeDamage();
            _anim.SetTrigger("OnEnemyDeath");
            _collider2D.enabled = false;
            _audio.clip = _explosionSound;
            _audio.Play();

            float _audioLength = _audio.clip.length;
            
            Destroy(this.gameObject, _audioLength);
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            _player.AddScore(_points);
            _anim.SetTrigger("OnEnemyDeath");
            _collider2D.enabled = false;
            _audio.clip = _explosionSound;
            _audio.Play();

            float _audioLength = _audio.clip.length;
            
            Destroy(this.gameObject, _audioLength);
        }
    }


}
