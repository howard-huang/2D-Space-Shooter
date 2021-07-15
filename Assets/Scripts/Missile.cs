using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6.0f;

    private bool _missileActive;

    private GameObject _targetToTrack = null;

    private Animator _anim;
    [SerializeField]
    private GameObject _thrusterPrefab;

    [SerializeField]
    private AK.Wwise.Event _missileAudio, _stopMissileAudio;
    [SerializeField]
    private AK.Wwise.Event _explosionAudio;

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
        PickTarget();
    }

    private void PickTarget()
    {
        List<GameObject> _targets = new List<GameObject>();

        if (this.tag == "Missile")
        {
            GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject _enemy in _enemies)
            {
                if (_enemy.transform.position.y > 0)
                {
                    _targets.Add(_enemy);
                }
            }

            GameObject[] _turrets = GameObject.FindGameObjectsWithTag("Turret");
            foreach (GameObject _turret in _turrets)
            {
                int _turretHealth = _turret.GetComponent<Turret>().GetHealth();
                if (_turretHealth > 0)
                {
                    _targets.Add(_turret);
                }
            }

            GameObject _boss = GameObject.FindGameObjectWithTag("Boss");
            if (_boss == true && _boss.GetComponent<PolygonCollider2D>().isActiveAndEnabled)
            {
                _targets.Add(_boss);
            }
        }
        else if (this.tag == "Enemy Missile")
        {
            _speed /= 1.5f;
            
            GameObject _player = GameObject.FindGameObjectWithTag("Player");
            _targets.Add(_player);
        }

        if (_targets.Count > 0)
        {
            _targetToTrack = _targets[Random.Range(0, _targets.Count)];

            StartCoroutine(RotateRoutine());

            if (_targetToTrack.name == "Player" && _targetToTrack != null)
            {
                StartCoroutine(PlayerEvadeTime());
            }
        }
        else return;
    }

    private void Update()
    {
        if (_missileActive == true)
        {
            Movement();
            _anim.SetTrigger("MissileActive");
            _thrusterPrefab.SetActive(true);
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

    private IEnumerator RotateRoutine()
    {
        while (_targetToTrack != null)
        {
            Vector3 _targetDirection = _targetToTrack.transform.position - this.transform.position;
            Quaternion _targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
            float _rotateSpeed = _speed * Time.deltaTime;

            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotateSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator PlayerEvadeTime()
    {
        while (_targetToTrack != null)
        {
            yield return new WaitForSeconds(3.0f);
            _targetToTrack = null;
        }
    }

    private void TargetLost()
    {
        Destroy(this.gameObject, 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" || other.tag == "Missile" || other.tag == "Super Laser")
        {
            _explosionAudio.Post(this.gameObject);
            Destroy(this.gameObject);
        }

    }

    private void OnDestroy()
    {
        _stopMissileAudio.Post(this.gameObject);
    }
}

