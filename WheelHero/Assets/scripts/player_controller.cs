using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour
{
    //variables
    [Header("Player Info:")]
    public float gravity;
    public float run_speed;
    public float walk_speed;
    public float jump_power;
    public float multiplier;
    public Transform line_cast_start, line_cast_end;
    public Collider2D unshielded_col, shielded_col;

    [Header("Wheel Info")]
    public GameObject wheel_prefab;
    public GameObject wheel_stack_prefab;
    public Transform wheel_spawn_transform, wheel_stack_spawn_transform;
    public float throwing_force, drop_force, drop_time, destory_time;
    public int wheels_stored;
    public Collider2D wheel_col;
    private Vector2 dir;

    [Header("Look At Mouse Info:")]
    public GameObject target;
    public GameObject target2;
    public Vector3 target_offset, target_offset_2;

    private Animator animator;
    private Rigidbody2D rb;
    private bool wheel_equipped, unequipped, shielding, grounded, drop_available, died;
    private float taps;
    
    //get components
    void Start()
    {
        wheel_col.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    //if not died, allow the player to keep moving
    void FixedUpdate()
    {
        if(!died)
        movement();
    }

    private void Update()
    {
        weapon();
        check_ground();
        get_input();
        animator.SetBool("unequipped", unequipped);
        animator.SetBool("shielding", shielding);
    }
    
    /*depending on the keys the player presses, they will be allowed to equip and unequip their wheel (E), 
    jump (W), sheild themselves (right mouse), throw the wheel (left mouse)*/
    private void get_input()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            if (drop_available && wheels_stored > 0 && !grounded)
            {
                drop_wheels();
                wheels_stored--;
            }

            if (grounded)
            {
                animator.SetTrigger("jump");
                rb.AddForce(new Vector2(0, jump_power));
                grounded = false;
                drop_available = true;
                StartCoroutine(drop_wheels_timer());
            }
        }

        if (wheel_equipped)
        {
            if (Input.GetMouseButton(0) && !shielding)
            {
                look_at_mouse();
            }
            if (Input.GetMouseButtonUp(0) && !shielding)
            {
                Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dir = mouse_pos - wheel_spawn_transform.position;
                dir.Normalize();

                if (dir.x > 0 && transform.localScale.x > 0 || dir.x < 0 && transform.localScale.x < 0)
                {
                    animator.SetTrigger("wheel_throw");
                    unequipped = true;
                    wheel_equipped = false;
                }
            }

            if (Input.GetMouseButton(1))
            {
                unshielded_col.enabled = false;
                shielded_col.enabled = true;
                wheel_col.enabled = true;
                shielding = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                unshielded_col.enabled = true;
                shielded_col.enabled = false;
                wheel_col.enabled = false;
                shielding = false;
            }
        }
    }

    /*allow the player to move left and right across the screen by the horizontal and vertical axis input. 
    change the players scale depending on which direction their facing*/
    private void movement()
    {
        float x_input = Input.GetAxisRaw("Horizontal");
        float speed;

        if (x_input != 0 && grounded)
        {
            animator.SetBool("run", true);
            speed = run_speed;
        }
        else
        {
            animator.SetBool("run", false);
            speed = walk_speed;
        }

        if (x_input != 0)
        {
            transform.localScale = new Vector2(0.5f * x_input, .5f);
        }

        speed *= x_input;
        rb.velocity = new Vector2(speed, gravity);
    }

    //play the equip animation based on whether the variable wheel equipped is true or false
    private void weapon()
    {
        animator.SetBool("wheel_equipped", wheel_equipped);

        if (Input.GetKeyDown(KeyCode.E) && !unequipped)
        {
            wheel_equipped = !wheel_equipped;
        }
    }

    //instantiate a wheel, throwing/forcing it in the direction of the player's mouse
    public void throw_wheel()
    {
        GameObject wheel = Instantiate(wheel_prefab, wheel_spawn_transform.position, Quaternion.identity);
        Rigidbody2D rb = wheel.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * throwing_force);
    }
    
    //instatiate the stack of wheel under the player, destroy it in a certain amount of time
    private void drop_wheels()
    {
        GameObject wheel_stack = Instantiate(wheel_stack_prefab, wheel_stack_spawn_transform.position, Quaternion.identity);
        Rigidbody2D rb = wheel_stack.GetComponent<Rigidbody2D>();
        rb.AddForce(-transform.up * drop_force);
        Destroy(wheel_stack, destory_time);
        Invoke("add_wheel", destory_time);
    }
    
    //add one to wheels stored variable
    private void add_wheel()
    {
        wheels_stored += 1;
    }

    //wait for a specific amount of time, then drop available equalls false
    private IEnumerator drop_wheels_timer()
    {
        yield return new WaitForSeconds(drop_time);
        drop_available = false;
    }

    //on the player, make a raycast facing down, if the ray hits a collider tagged "Ground" then grounded, else not grounded
    private void check_ground()
    {
        Debug.DrawRay(line_cast_start.position, line_cast_end.position, Color.yellow);
        RaycastHit2D hit = Physics2D.Linecast(line_cast_start.position, line_cast_end.position);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Ground")
            {
                grounded = true;
            }
        }
        else
        {
            grounded = false;
        }
    }

    //get the direction of the mouse and have the player look toward it
    private void look_at_mouse()
    {
        Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_pos += target_offset;

        Vector2 dir = new Vector2(transform.position.x - mouse_pos.x, mouse_pos.y - transform.position.y);
        target.transform.up = -dir;
    }
    
    //if dead, play the death animation and freeze the player's rigidbody
    public void death()
    {
        died = true;
        for (int i = 1; i < 4; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
        animator.SetLayerWeight(4, 1);
        animator.SetBool("died", true);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    /*if player hit another collider tagged "Wheel" and is unequpped, then equip the player with a whell and destory 
    the wheel that collided with the player, play the cathing animation. if collider tagged with "WheelStack" add upward force to the player*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wheel" && unequipped)
        {
            unequipped = false;
            wheel_equipped = true;
            animator.SetTrigger("catching");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "WheelStack")
        {
            rb.AddForce(new Vector2(0, jump_power) * multiplier);
        }
    }
}
