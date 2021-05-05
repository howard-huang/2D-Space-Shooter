using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator _anim;
    private float _animLength;
    private float _endTime;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator is Null!");
        }

        _animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        _endTime = Time.time + _animLength;
    }

    private void Update()
    {
        if (Time.time > _endTime)
        {
            Destroy(this.gameObject);
        }
    }
}
