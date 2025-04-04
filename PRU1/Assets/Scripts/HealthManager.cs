﻿using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Transform player;
    public float detectionRadius = 10f;
    public Image healthBar;
    public float healthAmount = 0;
    public event Action OnPlayerDie;

    private GameObject[] torches;
    public GameObject _conBoss;
    public Slider _bossHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torches = GameObject.FindGameObjectsWithTag("Torch");
        InvokeRepeating("TakeDamage", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

        

        if(healthAmount >= 100)
        {
            playerMovement.Die();
            _bossHealth.value = 50;
            _conBoss.transform.position = new Vector3(45f, -6f, 0f);
            _conBoss.SetActive(false);
            healthAmount = 0;
        }
        //bool nearTorch = true;
        foreach (GameObject torch in torches)
        {
            float distance = Vector2.Distance(player.position, torch.transform.position);

            if (distance <= 1f)
            {
                resetWarm();
                //CancelInvoke("TakeDamage");
                //nearTorch = true;
                break; 
            }
            /*else if(nearTorch)
            {
                nearTorch = false;
                InvokeRepeating("TakeDamage", 1f, 1f);
            }*/
                
                
        }

    }

    public void resetWarm()
    {
        healthAmount = 0;
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void TakeDamage()
    {
        healthAmount += 20;
        healthBar.fillAmount = healthAmount / 100f;
    }
}
