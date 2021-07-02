using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private int _health;
    [SerializeField]
    private int _baseHealth = 60;

    [SerializeField]
    private float _speed = 2.0f;

    private Vector3 _centrePos = new Vector3(0f, 12f, 0f);
    private Vector3 _centreRot = new Vector3(0f, 0f, 0f);

    private Vector3 _leftPos = new Vector3(-16f, 4f, 0f);
    private Vector3 _leftRot = new Vector3(0f, 0f, 90f);

    private Vector3 _rightPos = new Vector3(16f, 4f, 0f);
    private Vector3 _rightRot = new Vector3(0f, 0f, -90f);

    [SerializeField]
    private GameObject _mineHolders;
    [SerializeField]
    private GameObject _minePrefab;

    [SerializeField]
    private GameObject _laserContainer;

    private List<Turret> _allTurrets = new List<Turret>();

    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is Null!");
        }

        _health += _baseHealth;
        GetTurretScripts();
        StartCoroutine(FirstMovementRoutine());
    }

    private void GetTurretScripts()
    {
        foreach (Transform _transform in transform.GetComponentsInChildren<Transform>())
        {
            if (_transform.tag == "Turret" || _transform.tag == "Large Turret")
            {
                Turret _script = _transform.GetComponent<Turret>();

                if (_script != null)
                {
                    _allTurrets.Add(_script);
                }
            }
        }

        GetTurretHealth();
    }

    private IEnumerator FirstMovementRoutine()
    {
        transform.position = _centrePos;
        transform.rotation = Quaternion.Euler(_centreRot);

        while (transform.position.y > 4f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3f);
        FireMines("both");

        while (transform.position.y < 12f)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        StateSelector();

        foreach (Turret _turret in _allTurrets)
        {
            _turret.SetActive();
        }
    }

    private void StateSelector()
    {
        StopTurrets();
        int _randomState = Random.Range(0, 3);
        int _sideToFire = 0;

        switch (_randomState)
        {
            case 0:
                StartCoroutine(DownwardsRoutine());
                break;
            case 1:
                StartCoroutine(LeftToRightRoutine());
                _sideToFire = 1;
                break;
            case 2:
                StartCoroutine(RightToLeftRoutine());
                _sideToFire = 2;
                break;
        }
        StartTurrets(_sideToFire);
    }

    private IEnumerator DownwardsRoutine()
    {
        transform.position = _centrePos;
        transform.rotation = Quaternion.Euler(_centreRot);

        while (transform.position.y > 4f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3f);
        FireMines("both");

        while (transform.position.y < 12)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        StateSelector();
    }

    private IEnumerator LeftToRightRoutine()
    {
        transform.position = _leftPos;
        transform.rotation = Quaternion.Euler(_leftRot);

        float _mineTime = Random.Range(Time.time, Time.time + 5f);

        while (transform.position.x < 0)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        FireMines("left");

        while (transform.position.x < _rightPos.x)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        StateSelector();
    }

    private IEnumerator RightToLeftRoutine()
    {
        transform.position = _rightPos;
        transform.rotation = Quaternion.Euler(_rightRot);

        while (transform.position.x > 0)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        FireMines("right");

        while (transform.position.x > _leftPos.x)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        StateSelector();
    }

    private void FireMines(string _side)
    {
        GameObject _parent = null;
        switch (_side)
        {
            case "left":
                _parent = GameObject.Find("Left Mine Holders");
                break;
            case "right":
                _parent = GameObject.Find("Right Mine Holders");
                break;
            default:
                _parent = _mineHolders;
                break;
        }

        Transform[] _availableMines = _parent.GetComponentsInChildren<Transform>();

        if (_availableMines != null)
        {
            foreach (Transform _mineHolder in _availableMines)
            {
                if (_mineHolder.tag == "Mine Holder")
                {
                    GameObject _mine = Instantiate(_minePrefab, _mineHolder.position, Quaternion.identity);
                    _mine.transform.parent = _laserContainer.transform;
                }
            }
        }
    }

    private void StartTurrets(int _sideToFire)
    {
        foreach (Turret _turret in _allTurrets)
        {
            _turret.TurretControl(true, _sideToFire);
        }
    }
    
    private void StopTurrets()
    {
        foreach (Turret _turret in _allTurrets)
        {
            _turret.TurretControl(false, 0);
        }
    }

    private void GetTurretHealth()
    {
        foreach (Turret _turret in _allTurrets)
        {
            int _turretHealth = _turret.GetHealth();
            _health += _turretHealth;
        }
    }

    public void TurretDamage(int _damageTaken)
    {
        UpdateHealth(_damageTaken);
    }

    private void UpdateHealth(int _damageTaken)
    {
        _health -= _damageTaken;
        _uiManager.UpdateBossUI(_health);

        if (_health == 0)
        {
            StopAllCoroutines();
            Debug.Log("Boss Defeated");
        }
    }
}
