﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class PlayerController : MonoBehaviour
{
    [Header("Core Components")]
    private Player player;
    private Rigidbody rb;
    private Animator anim;

    [Header("Horizontal Movement")]
    private float input;
    public Joystick joystick;
    public float minX;
    public float maxX;
    public float horizontalMove = 0f;
    public float horizontalDrag = 0f;
    public float maxHorizontalVelocity;
    public float forceMultiplier;

    [Header("Misc")]
    public bool locked = false;

    [Header("Events")]
    private UnityAction returningToBaseDelegate;
    private UnityAction playerDeathDelegate;
    private UnityAction shootingDelegate;

    #region Unity Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    private void Start()
    {
        RegisterListeners();
        InvokeRepeating("RaiseMaximumVelocity", 1f, 1f);
    }

    void Update()
    {
        HandleInput();
    }
    #endregion

    #region Public Methods
    public void AddExternalForce(Vector3 direction, float forceValue)
    {
        rb.AddForce(direction * forceValue);
    }

    public void AddExternalForce(Vector3 direction, float forceValue, ForceMode forceMode)
    {
        rb.AddForce(direction * forceValue, forceMode);
    }
    #endregion

    #region Private Methods
    private void RegisterListeners()
    {
        returningToBaseDelegate = LockInput;
        EventManager.StartListening("ReturningToBase", returningToBaseDelegate);

        playerDeathDelegate = PlayerDeath;
        EventManager.StartListening("PlayerDeath", playerDeathDelegate);

        shootingDelegate = Shoot;
        EventManager.StartListening("Shoot", shootingDelegate);
    }

    /// <summary>
    /// Locks player input.
    /// </summary>
    private void LockInput()
    {
        locked = true;
    }

    private void HandleInput()
    {
        if (!locked)
        {
            HandleVelocity();

#if UNITY_ANDROID

            input = joystick.Horizontal;

#endif

#if UNITY_STANDALONE_WIN

            input = Input.GetAxis("Horizontal");

#endif
            HandleHorizontal(input);
            anim.SetFloat("Roll", input);

            if (Input.GetButtonDown("Shoot"))
            {
                Shoot();
            }

            if (Input.GetButtonDown("Cancel"))
            {
                EventManager.TriggerEvent("PauseMenu");
            }
        }
    }

    /// <summary>
    /// Manages the player's velocity.
    /// </summary>
    private void HandleVelocity()
    {
        if(Time.timeScale > 0f)
        {
            // Speed up
            if (rb.velocity.z < player.stats.currentMaximumVelocity)
            {
                rb.AddForce(Vector3.forward * player.stats.currentForwardThrust);
            }
            // Slow down
            else if (rb.velocity.z > player.stats.currentMaximumVelocity)
            {
                rb.AddForce(Vector3.forward * -0.5f, ForceMode.VelocityChange);
            }
        }  
    }

    /// <summary>
    /// Manages the player's horizontal movement.
    /// </summary>
    /// <param name="axis"></param>
    private void HandleHorizontal(float axis)
    {
        horizontalMove = axis * forceMultiplier * player.stats.currentManeuveringSpeed;
        Vector3 direction = Vector3.zero;

        if (transform.position.x >= minX && transform.position.x <= maxX)
        {
            direction = horizontalMove * Vector3.right;

            if (horizontalMove < 0f && rb.velocity.x > -maxHorizontalVelocity * player.stats.currentManeuveringSpeed)
            {
                rb.AddForce(direction, ForceMode.Impulse);
            }
            
            if (horizontalMove > 0f && rb.velocity.x < maxHorizontalVelocity * player.stats.currentManeuveringSpeed)
            {
                rb.AddForce(direction, ForceMode.Impulse);
            }
        }

        // Keep player inside the play area
        if (transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
            rb.velocity = new Vector3(0f, 0f, rb.velocity.z);
        }
        else if(transform.position.x > maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
            rb.velocity = new Vector3(0f, 0f, rb.velocity.z);
        }

        // Calculate and apply horizontal drag
        float dragLerp = Mathf.Abs(rb.velocity.x) / maxHorizontalVelocity;

        if (rb.velocity.x < 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x + (horizontalDrag * dragLerp) * Time.deltaTime, 0f, rb.velocity.z);
        }
        else if (rb.velocity.x > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x - (horizontalDrag * dragLerp) * Time.deltaTime, 0f, rb.velocity.z);
        }

    }

    /// <summary>
    /// Continually ramps up the player's maximum velocity, so they gain speed and the level becomes more challenging.
    /// </summary>
    private void RaiseMaximumVelocity()
    {
        if(player.stats.currentMaximumVelocity < player.stats.hardVelocityCap)
        {
            player.stats.maximumVelocityIncrementor++;
        }
    }

    private void PlayerDeath()
    {
        locked = true;
    }

    private void Shoot()
    {
        if (player.stats.currentBatteryLevel >= player.stats.currentBatteryDrain)
        {
            player.stats.currentBatteryLevel -= player.stats.currentBatteryDrain;

            GameObject parentGo = ObjectPooler.instance.GetPooledProjectile();
            if (parentGo)
            {
                // Get parent particle system and set the location
                Transform parentObject = parentGo.GetComponentInParent<Transform>();
                parentGo.transform.position = parentObject.position;

                // Get the children particle systems
                ParticleSystem[] children = parentGo.GetComponentsInChildren<ParticleSystem>(true);

                // Find the particle system responsible for the projectile
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i].CompareTag("Projectile"))
                    {
                        ParticleSystem projectileParticle = children[i];

                        parentGo.SetActive(true);
                        ParticleSystem.MainModule main = projectileParticle.main;
                        main.startSpeed = player.stats.currentProjectileSpeed + rb.velocity.z;

                        IEnumerator coroutine = ObjectPooler.instance.ReturnParticleToPool(parentGo, projectileParticle.main.startLifetimeMultiplier);
                        StartCoroutine(coroutine);
                        EventManager.TriggerEvent("ProjectileShot");
                        break;
                    }
                }
            }
        }
        else
        {
            EventManager.TriggerEvent("BatteryIsEmpty");
        }
    }

    #endregion

}
