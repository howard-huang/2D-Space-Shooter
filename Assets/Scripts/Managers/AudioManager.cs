using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _bgm;
    private bool _isGameOver;

    private void Update()
    {
        if (_isGameOver == true)
        {
            Destroy(_bgm);
        }
    }

    public void AMGameOver()
    {
        _isGameOver = true;
    }
}
