using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    [SerializeField]
    private AK.Wwise.Event[] _laserAudio; //0 = Player Laser, 1 = Enemy Laser;

    private void Start()
    {
        if (this.tag == "Laser")
        {
            _laserAudio[0].Post(this.gameObject);
        }
        else
        {
            _laserAudio[1].Post(this.gameObject);
        }
    }

    private void Update()
    {
        Movement();
        Destroy();
    }

    private void Movement()
    {
        if (this.tag == "Laser" || this.tag == "Rear Laser")
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
    }

    public void WaveOver()
    {
        _speed *= 2;
    }

    private void Destroy()
    {
        if (transform.position.y > 12 || transform.position.y < -8)
        {
            if (transform.parent != null && transform.parent.tag != "LaserContainer")
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        if (transform.position.x > 16 || transform.position.x < -16)
        {
            if (transform.parent != null && transform.parent.tag != "LaserContainer")
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
