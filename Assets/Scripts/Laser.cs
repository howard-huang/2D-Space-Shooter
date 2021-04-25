using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //speed variable of 8
    [SerializeField]
    private float _speed = 8.0f;

    void Update()
    {
        Movement();
        Destroy();
    }

    void Movement()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    void Destroy()
    {
        if (transform.position.y > 8)
        {
            Destroy(this.gameObject);
        }
    }
}
