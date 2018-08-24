using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour {
    public AudioSource audioClick;
    public AudioMixer mixer;
    public GameObject mainMenu;
    public StartMenu startMenu;
    public HighScoreMenu highScoreMenu;
    public OptionMenu optionMenu;
        
    public void GoToStartMenu(){
        startMenu.gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void GoToHighScoreMenu(){
        highScoreMenu.gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void GoToOptionMenu(){
        optionMenu.gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    private void Start(){
        GameApplication.LoadOptions();
        Options options = GameApplication.GetOptions();
        startMenu.Options = options;
        highScoreMenu.Options = options;
        optionMenu.Options = options;
        mixer.SetFloat("SoundVolume",options.SoundVolume);
        mixer.SetFloat("MusicVolume",options.MusicVolume);
        startMenu.goingBack += (o,ev) => { mainMenu.SetActive(true); };
        highScoreMenu.goingBack += (o,ev) => { mainMenu.SetActive(true); };
        optionMenu.goingBack += (o,ev) => { audioClick.Play(); mainMenu.SetActive(true); };
    }
    
    public void Quit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void Update(){
    }
}
