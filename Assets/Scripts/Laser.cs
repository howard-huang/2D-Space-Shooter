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
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void Destroy()
    {
        if (transform.position.y > 8)
        {
            Destroy(this.gameObject);
        }
    }
}
