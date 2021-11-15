using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret_controller : MonoBehaviour
{
    //variables
    [Header("Turret Info:")]
    public float fire_rate;
    public int max_ammo;
    public float range;
    public float fire_power;
    public float reload_time;
    public GameObject head;
    public GameObject bullet_prefab;
    public Transform muzzle_transform;
    public Vector2 offset;
    public float death_time, death_angle;

    private GameObject target;
    private bool fire_ready, died, death_counted;
    
    //get components
    void Start()
    {
        fire_ready = true;
        target = GameObject.FindGameObjectWithTag("Player");
        head.GetComponent<GameObject>();
    }
    
    //if not dead and player within distance, look at the toward the direction of the player, if ready to fire then run coroutine shoot bullet
    void Update()
    {
        if(Vector2.Distance(target.transform.position,transform.position) < range && !died)
        {

            Vector2 instanced_offset = offset;

            if (target.transform.position.x < transform.position.x)
            {
                head.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                head.transform.localScale = new Vector3(1, 1, 1);
                instanced_offset = offset * -1;
            }

            Vector2 dir = target.transform.position - transform.position;
            dir = dir + instanced_offset;
            dir.Normalize();

            head.transform.up = dir;

            if (fire_ready)
            {
                fire_ready = false;
                StartCoroutine(shoot_bullets());
            }
        }

        death();
    }
    
    //shoot three bullets toward the direction of the player the player. wait until reload time is finished, then fire is ready
    IEnumerator shoot_bullets()
    {
        int ammo = max_ammo;

        for (int i = 0; i < ammo; i++)
        {
            yield return new WaitForSeconds(fire_rate);
            GameObject bullet = Instantiate(bullet_prefab, muzzle_transform.position, head.transform.rotation);
            bullet.transform.localScale = new Vector3(head.transform.localScale.x, 1, 1);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            Vector2 dir = target.transform.position - transform.position;
            dir.Normalize();
            rb.AddForce(dir * fire_power);
        }

        yield return new WaitForSeconds(reload_time);
        fire_ready = true;
    }

    //if dead move the head of the turret's z rotation down
    private void death()
    {
        if (died)
        {
            float rot_z = Mathf.Lerp(head.transform.rotation.z, death_angle, death_time);
            head.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot_z));
        }
    }

    //if hit by the wheel, die and decrease the # of enemies in the game manager
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wheel")
        {
            died = true;
            if (!death_counted)
            {
                level_manager manager = FindObjectOfType<level_manager>();
                manager.number_of_enemies -= 1;
                death_counted = true;
            }
        }
    }
}
