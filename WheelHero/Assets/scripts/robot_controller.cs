﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robot_controller : MonoBehaviour
{
    [Header("Robot Info:")]
    public float reload_time;
    public int max_ammo;
    public float fire_force;
    public float range;
    public GameObject bullet_prefab;
    public Transform muzzle;
    public Vector2 offset;
    public float death_time, fade_time;
    public SpriteRenderer[] sprite_renderer;
    public Transform linecast_start, linecast_end;
    private GameObject target;
    private Animator animator;
    private Rigidbody2D rb;
    private bool fire_ready, died, death_counted, player_insight;


    void Start()
    {
        fire_ready = true;
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        foreach (SpriteRenderer s in sprite_renderer)
        {
            s.GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        death();
        check_weapon();
    }

    private void check_weapon()
    {
        if (Vector2.Distance(target.transform.position, transform.position) < range && fire_ready && player_insight)
        {
            animator.SetTrigger("fire");
            StartCoroutine(reload_timer());
            fire_ready = false;
        }
    }

    public void shoot_bullets()
    {
        Vector2 direction = target.transform.position - muzzle.transform.position;
        GameObject bullet = Instantiate(bullet_prefab, muzzle.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce((direction + offset) * fire_force);
    }

    private void death()
    {
        if (died)
        {
            float rot_z = Mathf.Lerp(transform.rotation.z, -90, death_time);
            for (int i = 0; i < sprite_renderer.Length; i++)
            {
                float alpha = Mathf.Lerp(sprite_renderer[i].color.a, 0, fade_time);
                sprite_renderer[i].color = new Color(255, 255, 255, alpha);

                if (sprite_renderer[i].color.a < .25f)
                {
                    Destroy(gameObject);
                }
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot_z));
        }
    }

    IEnumerator reload_timer()
    {
        yield return new WaitForSeconds(reload_time);
        fire_ready = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wheel")
        {
            player_insight = true;
        }
        else
        {
            player_insight = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wheel")
        {
            died = true;
            animator.enabled = false;
            rb.gravityScale = 1;

            Vector2 dir = collision.transform.position - transform.position;
            dir.Normalize();
            if (!death_counted)
            {
                level_manager manager = FindObjectOfType<level_manager>();
                manager.number_of_enemies -= 1;
                death_counted = true;
            }
        }
    }
}
