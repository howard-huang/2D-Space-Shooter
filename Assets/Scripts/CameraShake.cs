using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
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
    }

    private float RandomNumber(float _min, float _max)
    {
        float _randomNumber = Random.Range(_min, _max);
        return _randomNumber;
    }
}
