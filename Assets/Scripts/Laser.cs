using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    private void Update()
    {
        Movement();
        Destroy();
    }

    private void Movement()
    {
        if (this.tag == "Laser")
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
        else if (this.tag == "Enemy Laser")
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
        if (transform.position.y > 8 || transform.position.y < -8)
        {
            if (transform.parent != null && transform.parent.tag == "Triple Shot")
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
