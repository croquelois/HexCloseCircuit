using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    public HighScoreView view;
    public AudioSource musicGameOver;
    
    public void QuitToMenu(){
        musicGameOver.Stop();
        view.Save();
        SceneManager.LoadScene("main");
    }
    
    public void SetScore(int score){
        Options options = GameApplication.GetOptions();
        view.GameOver(options.board,options.piecePicker,options.speed,score);
    }
    
    void Start(){
        musicGameOver.Play();
    }
    
    void Update(){
        if(Input.GetButtonDown("Cancel"))
            QuitToMenu();
    }
}