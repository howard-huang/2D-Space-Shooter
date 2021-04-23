using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0); 
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
       
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0).normalized;             //normalized to stop axes giving a range of values

        transform.Translate(direction * _speed * Time.deltaTime);

        float _yClamp = Mathf.Clamp(transform.position.y, -5.25f, 0);                              //vertical bounds
        transform.position = new Vector3(transform.position.x, _yClamp, 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }                                                      //horizontal wrapping
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }
}
