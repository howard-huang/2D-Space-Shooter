using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int _enemyID;

    [Header("Speed")]
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private GameObject _thruster;

    [Header("Score")]
    [SerializeField]
    private int _points = 10;
    private bool _waveEnded;

    [Header("Laser")]
    [SerializeField]
    private GameObject _laserPrefab;
    private Vector3 _laserOffset = new Vector3(0, -1.0f, 0);
    private GameObject _laserContainer;
    private bool _canFire = true;

    [Header("Audio")]
    [SerializeField]
    private AK.Wwise.Event _explosionAudio;
    [SerializeField]
    private AK.Wwise.Event _laserAudio;

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

    public void SetID(int _ID)
    {
        _enemyID = _ID;

        switch (_enemyID) //1 = Diag Left, 2 = Diag Right
        {
            default:
                transform.rotation = Quaternion.identity;
                break;
            case 1:
                transform.rotation = Quaternion.Euler(0, 0, 75);
                break;
            case 2:
                transform.rotation = Quaternion.Euler(0, 0, -75);
                break;
        }
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //Vertical Bounds
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Explosion") || _waveEnded == true)
        {
            if (transform.position.y <= -6.0)
            {
                Destroy(this.gameObject);
            }
        }
        else if (transform.position.y < -9.0f)
        {
            float _randXPos = Mathf.Round(Random.Range(-9.0f, 9.0f) * 10) / 10;
            transform.position = new Vector3(_randXPos, 6.5f, 0f);
        }

        //Horizontal Bounds
        if (transform.position.x < -14 || transform.position.x > 14)
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator FireLaserCoroutine()
    {
        while (_canFire == true)
        {
            Vector3 _laserPos = transform.TransformPoint(_laserOffset);
            GameObject _laser = Instantiate(_laserPrefab, _laserPos, this.transform.rotation);          //Follows Rotation of Enemy
            _laserAudio.Post(this.gameObject);
            
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

    public void ClearField()
    {
        _speed *= 2;
        _canFire = false;
        _thruster.SetActive(true);
        _waveEnded = true;
    }

    private void EnemyDeath()
    {
        _canFire = false;
        _thruster.SetActive(false);
        _anim.SetTrigger("OnEnemyDeath");
        _collider2D.enabled = false;
        _explosionAudio.Post(this.gameObject);

        float _animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(this.gameObject, _animLength);
    }


}
