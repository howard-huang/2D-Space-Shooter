using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;

    private Vector3 _centrePos = new Vector3(0f, 12f, 0f);
    private Vector3 _centreRot = new Vector3(0f, 0f, 0f);

    private Vector3 _leftPos = new Vector3(-16f, 4f, 0f);
    private Vector3 _leftRot = new Vector3(0f, 0f, 90f);

    private Vector3 _rightPos = new Vector3(16f, 4f, 0f);
    private Vector3 _rightRot = new Vector3(0f, 0f, -90f);

    private bool _canFire;

    [SerializeField]
    private GameObject _largeTurrets;
    [SerializeField]
    private GameObject _largeLaserPrefab;
    [SerializeField]
    private WaitForSeconds _largeLaserFireRate = new WaitForSeconds(2f);

    [SerializeField]
    private GameObject _standardTurrets;
    [SerializeField]
    private GameObject _standardLaserPrefab;
    [SerializeField]
    private WaitForSeconds _standardLaserFireRate = new WaitForSeconds(5f);

    [SerializeField]
    private GameObject _mineHolders;
    [SerializeField]
    private GameObject _minePrefab;

    [SerializeField]
    private GameObject _laserContainer;

    private void Start()
    {
        StartCoroutine(FirstMovementRoutine());
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
    }

    private void StateSelector()
    {
        StopAllCoroutines();
        int _randomState = Random.Range(0, 3);
        string _sideToFire = null;

        switch (_randomState)
        {
            case 0:
                StartCoroutine(DownwardsRoutine());
                break;
            case 1:
                StartCoroutine(LeftToRightRoutine());
                _sideToFire = "left";
                break;
            case 2:
                StartCoroutine(RightToLeftRoutine());
                _sideToFire = "right";
                break;
        }

        StartCoroutine(FireLargeTurrets());
        StartCoroutine(FireStandardTurrets(_sideToFire));
    }

    private IEnumerator DownwardsRoutine()
    {
        transform.position = _centrePos;
        transform.rotation = Quaternion.Euler(_centreRot);

        _canFire = true;

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

        _canFire = false;
        StateSelector();
    }

    private IEnumerator LeftToRightRoutine()
    {
        transform.position = _leftPos;
        transform.rotation = Quaternion.Euler(_leftRot);

        _canFire = true;

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

        _canFire = false;
        StateSelector();
    }

    private IEnumerator RightToLeftRoutine()
    {
        transform.position = _rightPos;
        transform.rotation = Quaternion.Euler(_rightRot);

        _canFire = true;

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

        _canFire = false;
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

    private IEnumerator FireStandardTurrets(string _side)
    {
        GameObject _parent = null;
        switch (_side)
        {
            case "left":
                _parent = GameObject.Find("Left Turret Holder");
                break;
            case "right":
                _parent = GameObject.Find("Right Turret Holder");
                break;
            default:
                _parent = _standardTurrets;
                break;
        }

        while (_canFire == true)
        {
            Transform[] _availableTurrets = _parent.GetComponentsInChildren<Transform>();

            if (_availableTurrets != null)
            {
                foreach (Transform _turret in _availableTurrets)
                {
                    if (_turret.tag == "Turret")
                    { 
                        GameObject _laser =Instantiate(_standardLaserPrefab, _turret.position, _turret.rotation);
                        _laser.transform.parent = _laserContainer.transform;
                    }
                }
            }
            yield return _standardLaserFireRate;
        }
    }

    private IEnumerator FireLargeTurrets()
    {
        while (_canFire == true)
        {
            Transform[] _availableTurrets = _largeTurrets.GetComponentsInChildren<Transform>();

            if (_availableTurrets != null)
            {
                foreach (Transform _turret in _availableTurrets)
                {
                    if (_turret.tag == "Turret")
                    {
                        Vector3 _laserOffset = new Vector3(0, -1.75f, 0);
                        Vector3 _laserPos = _turret.transform.TransformPoint(_laserOffset);

                        GameObject _laser = Instantiate(_largeLaserPrefab, _laserPos, _turret.rotation);
                        _laser.transform.parent = _laserContainer.transform;
                    }
                }
            }
            yield return _largeLaserFireRate;
        }
    }
}
