using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public GameObject mainMenu;
    public StartMenu startMenu;
    public OptionMenu optionMenu;
        
    public void GoToStartMenu(){
        startMenu.gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void GoToOptionMenu(){
        optionMenu.gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    private void Start(){
        GameApplication.LoadOptions();
        Options options = GameApplication.GetOptions();
        optionMenu.Options = options;
        startMenu.Options = options;
        optionMenu.goingBack += (o,ev) => { mainMenu.SetActive(true); };
        startMenu.goingBack += (o,ev) => { mainMenu.SetActive(true); };
    }
    
    public void Quit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
