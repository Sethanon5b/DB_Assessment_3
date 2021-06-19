using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private float turnSpeed = 10f;
    //private float currentSpeed = 3;
    //private float acceleration = .2f;
    //private float maxSpeed = 25;

    public static Vector3 shipPosHoz;
    public static Vector3 shipPosVer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //GetComponent<Rigidbody>().angularVelocity = new Vector3(10, 0, 0);
    }


    void Update()
    {

        GetComponent<Rigidbody>().angularVelocity = new Vector3(10, 0, 0);
        shipPosHoz = transform.position;

        float horizontalInput = Input.GetAxis("Horizontal");

        transform.position = transform.position + new Vector3(horizontalInput * turnSpeed * Time.deltaTime, 0);

        var pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, -3.0f, 3.0f);       
        transform.position = pos;


        //if (currentSpeed < maxSpeed)
        //{
        //    currentSpeed += Time.deltaTime * acceleration;

        //    rb.velocity = transform.forward * currentSpeed;
        //}
        //else
        //{
        //    currentSpeed = maxSpeed;
        //}
    }
}

// for bulding the tilemaps : https://www.youtube.com/watch?v=xFhScBZdXxg