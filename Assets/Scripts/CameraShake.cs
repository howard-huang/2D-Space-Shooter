using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using TMPro;
>>>>>>> b469d0b0ee986f502caa46f5c1bc10c92dc944c9
using UnityEngine;

public class CameraShake : MonoBehaviour
{
<<<<<<< HEAD
    [SerializeField]
    private float _shakeLength = 0.3f;

    public void TakeDamage()
    {
        StartCoroutine(CameraShakeRoutine());
    }

    private IEnumerator CameraShakeRoutine()
    {
        Vector3 _defaultCameraPos = this.transform.position;
        float _shakeTime = Time.time + _shakeLength;

        while(Time.time < _shakeTime)
        {
            float _randomX = RandomNumber(-0.5f, 0.5f);
            float _randomY = RandomNumber(-0.5f, 0.5f);
            this.transform.position = new Vector3(_randomX, _randomY, transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        this.transform.position = _defaultCameraPos;
=======
    public void Damage()
    {
        StartCoroutine(StartShake());
    }

    private IEnumerator StartShake()
    {
        Vector3 _defaultCameraPos = this.transform.position;
        float _shakeTime = Time.time;
        float _shakeLength = _shakeTime + 0.3f;

        while (_shakeTime < _shakeLength)
        {
            _shakeTime += Time.deltaTime;
            float _xShake = RandomNumber(-0.5f, 0.5f);
            float _yShake = RandomNumber(-0.5f, 0.5f);
            Vector3 _shakePos = new Vector3(_xShake, _yShake, transform.position.z);
            transform.position = _shakePos;
            yield return new WaitForEndOfFrame();
        }
>>>>>>> b469d0b0ee986f502caa46f5c1bc10c92dc944c9
    }

    private float RandomNumber(float _min, float _max)
    {
<<<<<<< HEAD
        return Random.Range(_min, _max);
=======
        float _randomNumber = Random.Range(_min, _max);
        return _randomNumber;
>>>>>>> b469d0b0ee986f502caa46f5c1bc10c92dc944c9
    }
}
