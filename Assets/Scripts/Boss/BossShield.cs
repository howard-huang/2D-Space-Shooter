using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" || other.tag == "Missile")
        {
            Destroy(other.gameObject);
        }
    }
}
