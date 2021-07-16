using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.5f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private Boss _boss;

    private void Start()
    {
        _boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();

        if (_boss == null)
        {
            Debug.LogError("Boss is Null!");
        }

        StartCoroutine(Disperse());
    }

    private void Update()
    {
        Destroy();
    }

    private IEnumerator Disperse()
    {
        float _cooldownTime = Time.time + Random.Range(0.5f, 2.5f);
        Vector3 _direction = transform.position - _boss.transform.position;

        while (Time.time < _cooldownTime)
        {
            transform.Translate(_direction * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        while (true)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" || other.tag == "Super Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }
    }

    private void Destroy()
    {
        if (transform.position.y > 12 || transform.position.y < -8)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x > 16 || transform.position.x < -16)
        {
            Destroy(this.gameObject);
        }
    }
}
