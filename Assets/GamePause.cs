using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePause : MonoBehaviour {
    public GameObject mainMenu;
    public OptionMenu optionMenu;
    public AudioSource musicPause;
    
    public event EventHandler<EventArgs> goingBack = delegate {};
            
    public void GoToOptionMenu(){
        optionMenu.gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void BackToGame(){
        musicPause.Stop();
        gameObject.SetActive(false);
        goingBack(this,new EventArgs());
    }
    
    public void QuitToMenu(){
        musicPause.Stop();
        SceneManager.LoadScene("main");
    }
    
    void Start(){
        optionMenu.Options = GameApplication.GetOptions();
        optionMenu.goingBack += (o,ev) => { mainMenu.SetActive(true); };
    }
    
    void OnEnable(){
        musicPause.Play();
    }
    
    void Update(){
        if(Input.GetButtonDown("Cancel") && mainMenu.activeSelf)
            BackToGame();
    }
}