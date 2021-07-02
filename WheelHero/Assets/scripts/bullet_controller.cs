using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_controller : MonoBehaviour
{
    [Header("Bullet Info")]
    public SpriteRenderer sprite_renderer;
    public float time;

    private bool is_hit;
    private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (is_hit)
        {
            float alpha = Mathf.Lerp(sprite_renderer.color.a, 0, time);
            sprite_renderer.color = new Color(255, 255, 255, alpha);
        }
        if( sprite_renderer.color.a < .01f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        is_hit = true;

        if (collision.collider.tag == "Wheel" || collision.collider.tag == "Ground")
        {
            tag = "Dead_Bullet";
        }

        if(collision.gameObject.tag == "Player" && tag == "Bullet")
        {
            player_controller controller = collision.gameObject.GetComponent<player_controller>();
            controller.death();
        }
    }

}
