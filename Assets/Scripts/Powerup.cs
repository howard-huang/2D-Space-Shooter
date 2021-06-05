using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    private float _travelSpeed = 3.0f;

    [SerializeField]
    private int _powerupID; //0 = Triple Shot, 1 = Speed, 2 = Shields, 3 = Ammo, 4 = Health, 5 = Power Shot,

    [Header("0 = Common, 1 = Uncommon, 2 = Rare")]
    [SerializeField]
    private int _rarity;

    [Header("Powerup Specific Variables")]
    [SerializeField]
    private int _shieldStrengthCount;

    [SerializeField]
    private int _ammoPowerupCount;

    [SerializeField]
    private int _healthPowerupCount;

    [Header("Audio")]
    [SerializeField]
    private AK.Wwise.Event _powerupSound;

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        transform.Translate(Vector3.down * _travelSpeed * Time.deltaTime);

        if (transform.position.y <= -7)
        {
            Destroy(this.gameObject);
        }
    }

    public int GetRarity()
    {
        return _rarity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldStrength(_shieldStrengthCount);
                        break;
                    case 3:
                        player.AddAmmo(_ammoPowerupCount);
                        break;
                    case 4:
                        player.AddHealth(_healthPowerupCount);
                        break;
                    case 5:
                        player.SuperShotActive();
                        break;
                    case 6:
                        player.MissilesActive();
                        break;
                    default:
                        Debug.Log("Default Powerup Value");
                        break;
                }
            }

            _powerupSound.Post(this.gameObject);

            Destroy(this.gameObject);
        }
    }
}