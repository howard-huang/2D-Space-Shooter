using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator _anim;
    private float _animLength;
    private float _endTime;

    [Header("Audio")]
    [SerializeField]
    private AK.Wwise.Event _explosionAudio;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Explosion Animator is Null!");
        }

        _explosionAudio.Post(this.gameObject);

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
