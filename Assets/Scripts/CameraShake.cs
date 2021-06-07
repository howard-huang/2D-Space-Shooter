using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
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
    }

    private float RandomNumber(float _min, float _max)
    {
        return Random.Range(_min, _max);
    }
}
