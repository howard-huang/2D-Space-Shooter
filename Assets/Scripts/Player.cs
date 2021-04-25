﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _playerLives = 3;
    
    [SerializeField]
    private float _speed = 3.5f;

    [SerializeField]
    private GameObject _laserPrefab;
    private Vector3 _laserOffset = new Vector3(0f, 0.8f, 0f);
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1.0f;
    

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
       
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);             
                                                                                                   //more arcade like & stops faster diag movement
        transform.Translate(direction * _speed * Time.deltaTime);

        float _yClamp = Mathf.Clamp(transform.position.y, -5.25f, 0);                              //vertical bounds
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

    private void FireLaser()                                                                       //laser instantiate + cooldown
    {
        _canFire = Time.time + _fireRate;
        Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
    }

    public void TakeDamage()
    {
        _playerLives--;

        if (_playerLives == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
