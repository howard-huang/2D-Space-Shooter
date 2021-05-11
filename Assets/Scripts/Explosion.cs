using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator _anim;
    private float _animLength;
    private float _endTime;

    private AudioSource _audio;
    [SerializeField]
    private AudioClip _explosionAudio;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
        _audio = gameObject.GetComponent<AudioSource>();

        if (_anim == null)
        {
            Debug.LogError("Explosion Animator is Null!");
        }

        if (_audio == null)
        {
            Debug.LogError("Explosion Audio Source is Null!");
        }

        _audio.clip = _explosionAudio;
        _audio.Play();

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
