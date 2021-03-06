using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_Movement : MonoBehaviour
{
    private float turnSpeed = 12f;
    private float currentSpeed = 3;
    private float acceleration = .2f;
    private float maxSpeed = 28;
    private int score = 0;
    private float timer;

    string username = "";
    string alphabet = "abcdefghijklmnopqrstuvwxyz";

    public static Vector3 playerPos;
    public Text usernameText;
    public Text scoreText;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * currentSpeed;
        for (int i = 0; i < Random.Range(2, 5); i++)
        {
            username += alphabet[Random.Range(0, alphabet.Length)];
        }
        usernameText.text = "Username : " + username;
        scoreText.text = "Score : " + score.ToString();       
    }
    /// <summary>
    /// Handles horizontal movement, as well as the increasing the speed of the player overtime. 
    /// </summary>
    private void Update()
    {
        playerPos = transform.position;

        float horizontalInput = Input.GetAxis("Horizontal");

        // Horizontal Movement
        transform.position = transform.position + new Vector3(horizontalInput * turnSpeed * Time.deltaTime, 0);
        var pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, -4f, 4f);
        transform.position = pos;

        // Forward Speed
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += Time.deltaTime * acceleration;

            rb.velocity = transform.forward * currentSpeed;
        }
        else
        {
            currentSpeed = maxSpeed;
        }

        timer += Time.deltaTime;

        // Score Timer
        if (timer > 5f)
        {
            score +=  5;
          
            scoreText.text = "Score : " + score.ToString();

            timer = 0;
        }
    }
    // Send username / score to highscores when this is called 
    public void Die() 
    {         
        HighScores.AddNewHighScore(username, score);
        SceneManager.LoadScene("Death_Scene");
    }
}