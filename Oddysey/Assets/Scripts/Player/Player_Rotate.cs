using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Rotate : MonoBehaviour
{
    public float xSpeed = 5f;
    public float ySpeed = 5f;
    public float zSpeed = 5f;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(
        xSpeed * Time.deltaTime,
        ySpeed * Time.deltaTime,
        zSpeed * Time.deltaTime
        );
    }
}
