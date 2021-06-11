using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    //Weapons
    [SerializeField]
    private GameObject _weaponPrefab;
    private Vector3 _laserOffset = new Vector3(0, -1.0f, 0);
    private GameObject _laserContainer;
    private bool _canFire = true;

    [SerializeField]
    private AK.Wwise.Event _weaponAudio;

    private Player _player;
    private Animator _anim;
    private Collider2D _collider2D;

    [SerializeField]
    private GameObject _explosionPrefab;

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

        WeaponSelect();
    }

    public void SetID(int _ID)
    {
        _enemyID = _ID;

        switch (_enemyID) //1 = Diag Left, 2 = Diag Right, 3 = Missile Enemy;
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
        switch (_enemyID)  //1 = Diag Left, 2 = Diag Right, 3 = Missile Enemy;
        {
            default: 
                StandardMovement();
                break;
            case 3:
                StartCoroutine(MissileEnemyRoutine());
                break;
        }

        MovementBounds();
    }

    private void StandardMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private IEnumerator MissileEnemyRoutine() //Insert Code here
    {
        yield return new WaitForEndOfFrame();
    }

    private void MovementBounds()
    {
        //Vertical Bounds
        if (_waveEnded == true && transform.position.y <= -6.0)
        {
                Destroy(this.gameObject);
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

    private void WeaponSelect()
    {
        switch (_enemyID)  //1 = Diag Left, 2 = Diag Right, 3 = Missile Enemy;
        {
            default:
                StartCoroutine(FireLaserCoroutine());
                break;
            case 3: //Missile Firing will correlate with movement;
                return;                
        }
    }

    private IEnumerator FireLaserCoroutine()
    {
        while (_canFire == true)
        {
            Vector3 _laserPos = transform.TransformPoint(_laserOffset);
            GameObject _laser = Instantiate(_weaponPrefab, _laserPos, this.transform.rotation);          //Follows Rotation of Enemy
            _weaponAudio.Post(this.gameObject);
            
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

        foreach (Laser _laser in _laserContainer.GetComponentsInChildren<Laser>())
        {
            if (_laser.tag == "EnemyLaser")
            {
                _laser.WaveOver();
            }
        }
    }

    private void EnemyDeath() 
    {
        _canFire = false;
        _thruster.SetActive(false);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
