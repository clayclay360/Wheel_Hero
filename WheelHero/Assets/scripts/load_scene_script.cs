using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class load_scene_script : MonoBehaviour
{

    //if the player presses the r key then reload the current scene
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }
    
    //this function loads the scene depending on the index number
    public void load_scene(int number)
    {
        SceneManager.LoadScene(number, LoadSceneMode.Single);
    }

}
