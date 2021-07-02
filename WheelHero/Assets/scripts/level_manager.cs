using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level_manager : MonoBehaviour
{
    public int number_of_enemies;
    public int next_level;
    SpriteRenderer sprite_renderer;

    private void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(number_of_enemies == 0)
        {
            sprite_renderer.color = Color.green;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (number_of_enemies == 0 && collision.gameObject.tag == "Player")
        {
            load_level(next_level);
        }
    }

    private void load_level(int level)
    {
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
}
