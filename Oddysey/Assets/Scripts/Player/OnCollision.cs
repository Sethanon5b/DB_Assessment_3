using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    /// <summary>
    /// When the player collides with the obstacle, it will call the Die methods from the 
    /// Player_Movement
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") 
        {
            other.GetComponent<Player_Movement>().Die();
        }
    }
}
