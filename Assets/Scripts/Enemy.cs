using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
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

    List<GameObject> _activeMissiles = new List<GameObject>();

    private Player _player;
    private Animator _anim;
    private Collider2D _collider2D;

    [SerializeField]
    private GameObject _shieldVisual;
    private bool _shieldActive;
    [SerializeField]
    private GameObject _explosionPrefab;

    [Header("Audio")]
    [SerializeField]
    private AK.Wwise.Event _speedBoostAudio;
    [SerializeField]
    private AK.Wwise.Event _shieldAudio, _stopShieldAudio;

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
        switch (_enemyID) //1 = Diag Left, 2 = Diag Right, 3,4,5 = Dodge 6 = Missile Enemy, 7 = Ramming Enemy, 8 = Rear Fire;
        {
            default:
                transform.rotation = Quaternion.identity;
                StartCoroutine(FireLaserCoroutine());
                AddShieldChance();
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
                transform.rotation = Quaternion.identity;
                StartCoroutine(FireLaserCoroutine());
                StartCoroutine(DetectLaserRoutine());
                break;
            case 4:
                transform.rotation = Quaternion.Euler(0, 0, 75);
                StartCoroutine(FireLaserCoroutine());
                StartCoroutine(DetectLaserRoutine());
                break;
            case 5:
                transform.rotation = Quaternion.Euler(0, 0, -75);
                StartCoroutine(FireLaserCoroutine());
                StartCoroutine(DetectLaserRoutine());
                break;
            case 6:
                StartCoroutine(MissileEnemyRoutine());
                break;
            case 7:
                StartCoroutine(RammingBehaviorRoutine());
                break;
            case 8:
                StartCoroutine(RearLaserCoroutine());
                break;
        }

        WeaponAssign();
    }

    private void Update()
    {
        switch (_enemyID)  //1 = Diag Left, 2 = Diag Right, 3,4,5 = Dodge 6 = Missile Enemy, 7 = Ramming Enemy, 8 = Rear Fire;
        {
            default: 
                StandardMovement();
                MovementBounds(true);
                break;
            case 6:
                return;
            case 7:
                StandardMovement();
                MovementBounds(false);
                break;
            case 8:
                StandardMovement();
                MovementBounds(false);
                break;
        }
    }

    private void StandardMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private IEnumerator DetectLaserRoutine()
    {
        while (_canFire == true)
        {
            bool _rayDetected = false;

            while (_rayDetected == false)
            {
                Vector3 _rayOffset = new Vector3(0, -2, 0);
                RaycastHit2D _hit = Physics2D.CircleCast((transform.position + _rayOffset), 1.2f, Vector3.down);

                if (_hit.collider != null)
                {
                    if (_hit.collider.tag == "Laser" || _hit.collider.tag == "Missile")
                    {
                        Dodge();
                        _rayDetected = true;
                    }
                }
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(3);
        }
    }

    private void Dodge()
    {
        int _rotateDirection = Random.Range(0, 2); //0 = Left, 1 = Right;

        if (_rotateDirection == 0)
        {
            StartCoroutine(BarrelRoll(0)); //Left
        }
        else if (_rotateDirection == 1)
        {
            StartCoroutine(BarrelRoll(1)); //Right
        }
    }

    private IEnumerator MissileEnemyRoutine()
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

            if (_missileFired == "Left Missile")
            {
                StartCoroutine(BarrelRoll(1)); // Right
            }
            else if (_missileFired == "Right Missile")
            {
                StartCoroutine(BarrelRoll(0)); //Left
            }
            yield return new WaitForSeconds(1.0f);
        }

        StartCoroutine(EndMissileEnemyRoutine());
    }

    private IEnumerator BarrelRoll(int _directionToMove)
    {
        Vector3 _direction = new Vector3();
       
        if (_directionToMove == 0)
        {
            _direction = Vector3.left;
            _anim.SetTrigger("Enemy_Rotate_Left");
        }
        else if (_directionToMove == 1)
        {
            _direction = Vector3.right;
            _anim.SetTrigger("Enemy_Rotate_Right");
        }

        float _animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        float _rotateTime = Time.time + _animLength;

        while (Time.time < _rotateTime)
        {
            transform.Translate(_direction * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator EndMissileEnemyRoutine()
    {
        _thruster.SetActive(true);
        _speedBoostAudio.Post(this.gameObject);

        while (transform.position.y > -6)
        {
            transform.Translate(Vector3.down * (_speed * 2.0f) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Destroy(this.gameObject);
    }

    private IEnumerator RammingBehaviorRoutine()
    {
        while (transform.position.y > _player.transform.position.y)
        {
            if (_player != null)
            {
                Vector3 _targetDirection = this.transform.position - _player.transform.position;
                Quaternion _targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
                float _rotateSpeed = _speed * Time.deltaTime;

                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotateSpeed);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void MovementBounds(bool _canRespawn)
    {
        //Vertical Bounds
        if ((_waveEnded == true || _canRespawn == false) && transform.position.y <= -6.0)
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
            if (_weapon.tag == "Enemy Missile")
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

    private IEnumerator RearLaserCoroutine()
    {
        while (transform.position.y > _player.transform.position.y)
        {
            yield return new WaitForEndOfFrame();
        }

        while (_canFire == true)
        {
            GameObject _laser = Instantiate(_weaponPrefab[1], transform.position, Quaternion.identity);
            _laser.transform.parent = _laserContainer.transform;

            yield return new WaitForSeconds(1.0f);
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
            
            if (_enemyID == 4)
            {
                Instantiate(_explosionPrefab, other.transform.position, Quaternion.identity);
                return;
            }
            else
            {
                Damage();
            }
        }
        else if (other.tag == "Laser" || other.tag == "Missile")
        {
            Destroy(other.gameObject);

            Damage();
        }
        else if (other.tag == "Super Laser")
        {
            Damage();
        }
    }

    public void ClearField()
    {
        _speed *= 2;
        _canFire = false;
        _thruster.SetActive(true);
        _stopShieldAudio.Post(this.gameObject);
        _waveEnded = true;

        foreach (Laser _laser in _laserContainer.GetComponentsInChildren<Laser>())
        {
            if (_laser.tag == "EnemyLaser")
            {
                _laser.WaveOver();
            }
        }
    }

    private void AddShieldChance()
    {
        int _chance = Random.Range(0, 3);

        if (_chance == 0)
        {
            _shieldVisual.SetActive(true);
            _shieldActive = true;
            _shieldAudio.Post(this.gameObject);
        }
    }

    private void Damage() 
    {
        if (_shieldActive == true)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _shieldVisual.SetActive(false);
            _shieldActive = false;
            _stopShieldAudio.Post(this.gameObject);
            return;
        }
        else
        {
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        _player.AddScore(_points);

        _canFire = false;
        _thruster.SetActive(false);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);     
    }
}
