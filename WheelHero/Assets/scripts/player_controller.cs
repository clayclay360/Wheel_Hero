using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour
{

    [Header("Player Info:")]
    public float gravity;
    public float run_speed;
    public float walk_speed;
    public float jump_power;

    [Header("Wheel Info")]
    public GameObject wheel_prefab;
    public Transform spawn_transform;
    public float throwing_force;
    private Vector2 dir;

    [Header("Look At Mouse Info:")]
    public GameObject target;
    public GameObject target2;
    public Vector3 target_offset, target_offset_2;


    private bool grounded;


    private Animator animator;
    private Rigidbody2D rb;
    private bool wheel_equipped, unequipped;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //look_at_mouse();
        movement();
    }

    private void Update()
    {
        get_input();
        weapon();
        animator.SetBool("unequipped", unequipped);
    }

    private void get_input()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                animator.SetTrigger("jump");
                rb.AddForce(new Vector2(0, jump_power));
                grounded = false;
            }
        }

        if (wheel_equipped)
        {
            if (Input.GetMouseButton(0))
            {
                look_at_mouse();
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dir = mouse_pos - spawn_transform.position;
                dir.Normalize();
                animator.SetTrigger("wheel_throw");
                unequipped = true;
                wheel_equipped = false;
            }
        }
    }

    private void movement()
    {
        float x_input = Input.GetAxisRaw("Horizontal");
        float speed;

        if(x_input != 0 && grounded)
        {
            animator.SetBool("run", true);
            speed = run_speed;
        }
        else
        {
            animator.SetBool("run", false);
            speed = walk_speed;
        }

        speed *= x_input;
        rb.velocity = new Vector2(speed, gravity);
    }

    private void weapon()
    {
        animator.SetBool("wheel_equipped", wheel_equipped);

        if (Input.GetKeyDown(KeyCode.E) && !unequipped)
        {
            wheel_equipped = !wheel_equipped;
        }
    }

    public void throw_wheel()
    {
        GameObject wheel = Instantiate(wheel_prefab, spawn_transform.position, Quaternion.identity);
        Rigidbody2D rb = wheel.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * throwing_force);
    }

    private void look_at_mouse()
    {
        Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_pos += target_offset;
       
        Vector2 dir = new Vector2(transform.position.x - mouse_pos.x, mouse_pos.y - transform.position.y);
        target.transform.up = -dir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            grounded = true;
        }

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wheel" && unequipped)
        {
            unequipped = false;
            wheel_equipped = true;
            animator.SetTrigger("catching");
            Destroy(collision.gameObject);
        }
    }
}
