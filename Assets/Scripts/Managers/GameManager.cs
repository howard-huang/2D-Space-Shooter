using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver;

    private void Update()
    {
        if (_isGameOver == true && Input.GetKeyDown(KeyCode.R))
        {
            Scene _scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(_scene.name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void GMGameOver()
    {
        _isGameOver = true;
    }
}
