using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level_manager : MonoBehaviour
{
    //variables 
    public int number_of_enemies;
    public int next_level;
    SpriteRenderer sprite_renderer;

    //get sprite renderer
    private void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }
    
    //if the # of enemies equals 0 then turn the panel green
    void Update()
    {
        if(number_of_enemies == 0)
        {
            sprite_renderer.color = Color.green;
        }
    }
    
    /*if the # of enemies equals 0 and the player is staying on
    the panel, then load the next level*/
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (number_of_enemies == 0 && collision.gameObject.tag == "Player")
        {
            load_level(next_level);
        }
    }
    
    //function for loading the next lexel
    private void load_level(int level)
    {
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
}
