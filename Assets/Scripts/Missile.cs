using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6.0f;

    private bool _missileActive;
    private bool _targetFound;

    List<GameObject> _targets = new List<GameObject>();
    private GameObject _targetEnemy = null;

    private Animator _anim;
    [SerializeField]
    private GameObject _thrusterPrefab;

    [SerializeField]
    private AK.Wwise.Event _missileAudio, _stopMissileAudio;

    private void Start()
    {
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.Log("Animator is Null!");
        }
    }

    public void Fire()
    {
        this.gameObject.transform.parent = null;
        _missileAudio.Post(this.gameObject);
        _missileActive = true;
    }

    private void Update()
    {
        if (_missileActive == true)
        {
            Movement();
            _anim.SetTrigger("MissileActive");
            _thrusterPrefab.SetActive(true);

            if (_targetFound == false)
            {
                PickTarget();
            }
            else if (_targetFound == true)
            {
                Rotate();
            }
        }
    }

    private void PickTarget()
    {
        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> _targets = new List<GameObject>();

        foreach (GameObject _enemy in _enemies)
        {
            if (_enemy.transform.position.y > 0)
            {
                _targets.Add(_enemy);
            }
        }

        if (_targets.Count > 0)
        {
            _targetEnemy = _targets[Random.Range(0, _targets.Count)];
            _targetFound = true;
        }
    }

    private void Movement()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.x < -12 || transform.position.x > 12)
        {
            TargetLost();
        }
        
        if (transform.position.y < -6 || transform.position.y > 6)
        {
            TargetLost();
        }
    }

    private void Rotate()
    {
        if (_targetEnemy == null)
        {
            _targetFound = false;
        }
        else
        {
            Vector3 _targetDirection = _targetEnemy.transform.position - this.transform.position;
            Quaternion _targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
            float _rotateSpeed = _speed * Time.deltaTime;

            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotateSpeed);
        }
    }

    private void TargetLost()
    {
        Destroy(this.gameObject, 1.0f);
    }

    private void OnDestroy()
    {
        _stopMissileAudio.Post(this.gameObject);
    }
}

