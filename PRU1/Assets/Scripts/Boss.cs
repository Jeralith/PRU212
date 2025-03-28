﻿using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    public int health;
    public int damage;
    private float timeBtwDamage = 1.5f;
    public GameObject heart;

    //public Animator camAnim;
    public Slider healthBar;
    private Animator anim;
    private CircleCollider2D _collider;
    public bool isDead;

    private void Start()
    {
        
        anim = GetComponent<Animator>();
        _collider = GetComponent<CircleCollider2D>();
        heart.SetActive(false);
    }

    public void OnEnable()
    {
        health = 50;
        healthBar.value = health;
        anim.SetTrigger("stageOne");
    }

    private void Update()
    {
        

        if (health <= 25) {
            anim.SetTrigger("stageTwo");
        }

        if (health <= 0) {
            anim.SetTrigger("death");
            _collider.enabled = false;
            heart.SetActive(true);
        }

        // give the player some time to recover before taking more damage !
        if (timeBtwDamage > 0) {
            timeBtwDamage -= Time.deltaTime;
        }

        healthBar.value = health;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // deal the player damage ! 
        
        if (other.CompareTag("Torch"))
        {
            health -= damage;
        }
    }

}
