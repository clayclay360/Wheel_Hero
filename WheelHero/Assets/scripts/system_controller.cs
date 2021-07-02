using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class system_controller : MonoBehaviour
{
    public GameObject door;
    public float y_target_pos;
    public float time;
    public bool door_open;

    private SpriteRenderer sprite_renderer;
    private float reference;
    private float current_y;
    void Start()
    {
        current_y = door.transform.position.y;
        sprite_renderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (door_open)
        {
            float y_pos = Mathf.SmoothDamp(door.transform.position.y, y_target_pos, ref reference, time);
            door.transform.position = new Vector2(door.transform.position.x, y_pos);
            sprite_renderer.color = Color.black;
        }
        else
        {
            float y_pos = Mathf.SmoothDamp(door.transform.position.y, current_y, ref reference, time);
            door.transform.position = new Vector2(door.transform.position.x, y_pos);
            sprite_renderer.color = Color.red;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wheel")
        {
            door_open = !door_open;
        }
    }
}
