using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.5f;

    private Boss _boss;

    private void Start()
    {
        _boss = GameObject.Find("Boss").GetComponent<Boss>();

        if (_boss == null)
        {
            Debug.LogError("Boss is Null!");
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }
}
