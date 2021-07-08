using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]
    private int _health = 20;
    private bool _shieldDown;

    private bool _isActive = true;
    private bool _canFire;

    [SerializeField]
    private GameObject _largeLaserPrefab;
    [SerializeField]
    private WaitForSeconds _largeLaserFireRate = new WaitForSeconds(1.5f);

    [SerializeField]
    private GameObject _standardLaserPrefab;
    [SerializeField]
    private WaitForSeconds _standardLaserFireRate = new WaitForSeconds(4f);

    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _smokePrefab;
    [SerializeField]
    private GameObject _damageVisualContainer;

    private Quaternion _targetRotation;
    private float _xClamp;
    private float _yClamp;

    [SerializeField]
    private Boss _boss;

    private Player _player;
    private Transform _laserContainer;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _laserContainer = GameObject.Find("LaserContainer").GetComponent<Transform>();

        if (_player == null)
        {
            Debug.LogError("Player is Null!");
        }

        if (_laserContainer == null)
        {
            Debug.LogError("Laser Container is Null!");
        }
    }

    private void Update()
    {
        if (_player != null)
        {
            TrackTarget();
        }
    }

    public int GetHealth()
    {
        return _health;
    }

    public void SetActive()
    {
        _shieldDown = true;
    }

    public void TurretControl(bool _canFire, int _sideToFire)
    {
        if (_canFire == true)
        {
            StartCoroutine(RotateTurret());
            SideSelection(_sideToFire);
            StartCoroutine(FireLargeTurrets());
        }
        else if (_canFire == false)
        {
            StopAllCoroutines();
        }
    }

    private void TrackTarget()
    {
        Vector3 _targetDirection = _boss.transform.position - _player.transform.position;

        if (this.tag == "Large Turret")
        {
            _xClamp = Mathf.Clamp(_targetDirection.x, -1f, 1f);
            _yClamp = Mathf.Clamp(_targetDirection.y, -3f, 3f);
        }
        else
        {
            _xClamp = Mathf.Clamp(_targetDirection.x, -1f, 1f);
            _yClamp = Mathf.Clamp(_targetDirection.y, -1f, 1f);
        }

        Vector3 _clampedTarget = new Vector3(_xClamp, _yClamp, 0f);
        _targetRotation = Quaternion.LookRotation(_boss.transform.forward, _clampedTarget);
    }

    private IEnumerator RotateTurret()
    {
        while (_isActive)
        {
            float _rotateSpeed = 0.5f * Time.deltaTime;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, _rotateSpeed);

            yield return new WaitForFixedUpdate();
        }
    }

    private void SideSelection(int _side)
    {
        switch (_side)
        {
            case 1:
                if (transform.parent.name == "Left Turret Holder")
                {
                    StartCoroutine(FireStandardTurrets());
                }
                break;
            case 2:
                if (transform.parent.name == "Right Turret Holder")
                {
                    StartCoroutine(FireStandardTurrets());
                }
                break;
            default:
                StartCoroutine(FireStandardTurrets());
                break;
        }
    }

    private IEnumerator FireStandardTurrets()
    {
        if (this.tag == "Turret")
        {
            while (_isActive)
            {
                GameObject _laser = Instantiate(_standardLaserPrefab, transform.position, transform.rotation);
                _laser.transform.parent = _laserContainer;

                yield return _standardLaserFireRate;
            }
        }
    }

    private IEnumerator FireLargeTurrets()
    { 
        if (this.tag == "Large Turret")
        {
            while (_isActive)
            {
                Vector3 _laserOffset = new Vector3(0, -1.75f, 0);
                Vector3 _laserPos = transform.TransformPoint(_laserOffset);

                GameObject _laser = Instantiate(_largeLaserPrefab, _laserPos, transform.rotation);
                _laser.transform.parent = _laserContainer;

                yield return _largeLaserFireRate;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActive == true && _shieldDown == true)
        {
            if (other.tag == "Laser" || other.tag == "Missile")
            {
                Damage(10);
                Destroy(other.gameObject);
            }
            else if (other.tag == "Super Laser")
            {
                Damage(10);
            }
        }
    }

    private void Damage(int _damageTaken)
    {
        _health -= _damageTaken;
        _boss.TurretDamage(_damageTaken);

        if (_health <= 0)
        {
            DestroyedVisuals();
            _isActive = false;
            _boss.RemoveTurretFromList(this.gameObject);
        }
    }

    private void DestroyedVisuals()
    {
        GameObject _explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
       
        GameObject _smoke = Instantiate(_smokePrefab, transform.position, _boss.transform.rotation);
        _smoke.transform.parent = this.transform;
    }

    public void StopTurrets()
    {
        StopAllCoroutines();
    }
}
