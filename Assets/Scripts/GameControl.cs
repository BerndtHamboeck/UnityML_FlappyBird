using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

 	public static GameControl instance;         //A reference to our game control script so we can access it statically.
    public Text scoreText;                      //A reference to the UI text component that displays the player's score.

    //if null than no game over message and no delay to restart
    public GameObject gameOvertext;             //A reference to the object that displays the text which appears when the player dies.

    private int score = 0;                      //The player's score.
    public bool gameOver = false;               //Is the game over?
    private bool waitRequired;                  //block game over and restart
    public float scrollSpeed = -1.5f;
    public bool gotScore;   //for ML

    void Awake()
    {
        //If we don't currently have a game control...
        if (instance == null)
            //...set this one to be it...
            instance = this;
        //...otherwise...
        else if(instance != this)
            //...destroy this one because it is a duplicate.
            Destroy (gameObject);
    }

    void Update()
    {
		//exit on android
        if (Input.GetKey("escape"))
            Application.Quit();

        //If the game is over and the player has pressed some input...
        if (gameOver && Input.GetMouseButtonDown(0) ) 
        {
            if(waitRequired)
                return;
            //...reload the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }



    public void BirdScored()
    {
        //The bird can't score if the game is over.
        if (gameOver)   
            return;
        //If the game is not over, increase the score...
        score++;
        //...and adjust the score text.
        scoreText.text = "Score: " + score.ToString();

        gotScore = true;
    }

    public void BirdDied()
    {
        //Set the game to be over.
        gameOver = true;
        //Activate the game over text.
        if(gameOvertext != null) {
            gameOvertext.SetActive (true);
            waitRequired = true;
            StartCoroutine(DelayHelper(1.5f));
        }
    }

    IEnumerator DelayHelper (float seconds) {
        // Do something
        yield return new WaitForSeconds(seconds);  // Wait
        // Do something else
        waitRequired = false;
    }

}
