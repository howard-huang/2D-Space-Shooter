using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
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

    [SerializeField]
    private GameObject[] _weaponPrefab; //0 = Laser, 1 = Missile Left, 2 = Missile Right
    private GameObject _laserContainer;
    private bool _canFire = true;

    private GameObject _activeLaser = null;
    List<GameObject> _activeMissiles = new List<GameObject>();

    private Player _player;
    private Animator _anim;
    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private GameObject _explosionPrefab;

    [Header("Audio")]
    [SerializeField]
    private AK.Wwise.Event _speedBoostAudio;

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

        IDBehaviors();
    }

    public void SetID(int _ID)
    {
        _enemyID = _ID;
    }

    private void IDBehaviors()
    {
        switch (_enemyID) //1 = Diag Left, 2 = Diag Right, 3 = Missile Enemy;
        {
            default:
                transform.rotation = Quaternion.identity;
                StartCoroutine(FireLaserCoroutine());
                break;
            case 1:
                transform.rotation = Quaternion.Euler(0, 0, 75);
                StartCoroutine(FireLaserCoroutine());
                break;
            case 2:
                transform.rotation = Quaternion.Euler(0, 0, -75);
                StartCoroutine(FireLaserCoroutine());
                break;
            case 3:
                StartCoroutine(MissileEnemyRoutine());
                break;
        }

        WeaponAssign();
    }

    private void Update()
    {
        switch (_enemyID)  //1 = Diag Left, 2 = Diag Right, 3 = Missile Enemy;
        {
            default: 
                StandardMovement();
                MovementBounds();
                break;
            case 3:
                return;
        }
    }

    private void StandardMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private IEnumerator MissileEnemyRoutine() //Insert Code here
    {
        while(transform.position.y > 3)
        {
            yield return new WaitForEndOfFrame();
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        yield return new WaitForEndOfFrame();

        while (_activeMissiles.Count > 0)
        {
            string _missileFired = FireMissile();

            Vector3 _rotateDirection = BarrelRoll(_missileFired);

            float _animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
            float _rotateTime = Time.time + _animLength;

            while (Time.time < _rotateTime)
            {
                transform.Translate(_rotateDirection * _speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(1.0f);
        }

        StartCoroutine(EndMissileEnemyRoutine());
    }

    private Vector3 BarrelRoll(string _missileFired)
    {
        if (_missileFired == "Left Missile")
        {
            _anim.SetTrigger("Enemy_Rotate_Right");
            return Vector3.right;
        }
        else if (_missileFired == "Right Missile")
        {
            _anim.SetTrigger("Enemy_Rotate_Left");
            return Vector3.left;
        }
        else
        {
            Debug.Log("No Missile Fired");
            return Vector3.down;
        }
    }

    private IEnumerator EndMissileEnemyRoutine()
    {
        _thruster.SetActive(true);
        _speedBoostAudio.Post(this.gameObject);

        while (true)
        {
            transform.Translate(Vector3.down * (_speed * 2.0f) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
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

    private void WeaponAssign()
    {
        foreach (GameObject _weapon in _weaponPrefab)
        {
            if (_weapon.tag == "Enemy Laser")
            {
                _activeLaser = _weapon;
            }
            else if (_weapon.tag == "Enemy Missile")
            {
                _activeMissiles.Add(_weapon);
            }
        }
    }

    private IEnumerator FireLaserCoroutine()
    {
        while (_canFire == true)
        {
            Vector3 _laserOffset = new Vector3(0, -1.0f, 0);
            Vector3 _laserPos = transform.TransformPoint(_laserOffset);
            GameObject _laser = Instantiate(_weaponPrefab[0], _laserPos, this.transform.rotation);          //Follows Rotation of Enemy
            
            _laser.tag = "Enemy Laser";
            _laser.transform.parent = _laserContainer.transform;

            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
        }
    }

    private string FireMissile()
    {
        GameObject _missileToFire = _activeMissiles[Random.Range(0, _activeMissiles.Count)];
        Missile _missile = _missileToFire.GetComponent<Missile>();
        string _missileName = _missileToFire.name;
        _activeMissiles.Remove(_missileToFire);

        _missileToFire.SetActive(true);
        _missile.Fire();
        return _missileName;
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
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
