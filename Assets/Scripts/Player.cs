using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health and Damage")]
    [SerializeField]
    private int _playerLives = 3;
    private bool _isShieldActive;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private GameObject[] _damageVisuals;
    [SerializeField]
    private GameObject _explosionPrefab;

    [Header("Score")]
    [SerializeField]
    private int _score;

    [Header("Speed")]
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private float _speedBoostCooldown = 5.0f;

    [SerializeField]
    private GameObject _thrusterVisual;
    [SerializeField]
    private float _thrusterMultiplier = 1.5f;

    [Header("Lasers")]
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1.0f;

    [SerializeField]
    private GameObject _laserContainer;
    [SerializeField]
    private GameObject _laserPrefab;
    private Vector3 _laserOffset = new Vector3(0f, 1.0f, 0f);
   
    [SerializeField]
    private GameObject _tripleShotPrefab;
    private bool _isTripleShotActive;
    [SerializeField]
    private float _tripleShotCooldown = 5.0f;

    [Header("Audio")]
    [SerializeField]
    private AK.Wwise.Event _laserAudio;
    [SerializeField]
    private AK.Wwise.Event _explosionAudio;
    [SerializeField]
    private AK.Wwise.Event _thrusterAudio;
    [SerializeField]
    private AK.Wwise.Event _stopThrusterAudio;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private void Start()
    {
        transform.position = new Vector3(0, -3, 0);

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is Null!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is Null!");
        }

        _shieldVisual.SetActive(false);
    }

    private void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

        ThrusterCheck();   
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
       
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        
        transform.Translate(direction * _speed * Time.deltaTime);     

        float _yClamp = Mathf.Clamp(transform.position.y, -4.5f, 2);                               //vertical bounds
        transform.position = new Vector3(transform.position.x, _yClamp, 0);
                                                                                                   //horizontal wrapping
        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }                                                      
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    private void ThrusterCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ThrusterOn();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ThrusterOff();
        }
    }

    private void ThrusterOn()
    {
        _speed *= _thrusterMultiplier;
        _thrusterAudio.Post(this.gameObject);
        _thrusterVisual.SetActive(true);
    }

    private void ThrusterOff()
    {
        _speed /= _thrusterMultiplier;
        _stopThrusterAudio.Post(this.gameObject);
        _thrusterVisual.SetActive(false);
    }

    public void AddScore(int _points)
    {
        _score += _points;
        _uiManager.UpdateScoreUI(_score);
    }

    private void FireLaser()                                                                       
    {
        Vector3 _laserPos = transform.position + _laserOffset;
        _canFire = Time.time + _fireRate;                                                           //Cooldown System

        if (_isTripleShotActive == true)
        {
            GameObject _tripleShot = Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            _tripleShot.transform.parent = _laserContainer.transform;                               //GameObject Nesting
        }
        else
        {
            GameObject _laser = Instantiate(_laserPrefab, _laserPos, Quaternion.identity);
            _laser.transform.parent = _laserContainer.transform;                                    //GameObject Nesting
        }

        _laserAudio.Post(this.gameObject);
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_tripleShotCooldown);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(_speedBoostCooldown);
        _speed /= _speedMultiplier;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisual.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy Laser")
        {
            Destroy(other.gameObject);
            _explosionAudio.Post(this.gameObject);
            TakeDamage();
        }
    }
    
    public void TakeDamage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            return;
        }

        _playerLives--;
        _uiManager.UpdateLivesUI(_playerLives);

        switch (_playerLives)
        {
            default:
                _damageVisuals[1].SetActive(false);
                _damageVisuals[0].SetActive(false);
                break;
            case 2:
                _damageVisuals[1].SetActive(true);
                break;
            case 1:
                _damageVisuals[0].SetActive(true);
                break;
            case 0:
                _spawnManager.OnPlayerDeath();
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
                break;
        }    
    }
}
