using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    void Update()
    {
		//exit on android
        if (Input.GetKey("escape"))
            Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

}
