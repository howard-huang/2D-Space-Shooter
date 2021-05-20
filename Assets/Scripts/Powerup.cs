using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _travelSpeed = 3.0f;

    [SerializeField]
    private int _powerupID; //0 = Triple Shot, 1 = Speed, 2 = Shields

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
                        player.ShieldActive();
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