using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject startMenu;
    public GameObject optionMenu;
    public Options options;
    
    // Start menu
    public Dropdown wBoard;
    public Dropdown wPieces;
    public Dropdown wSpeed;
    
    // Option menu
    public Slider wRotationSpeed;
    public Toggle wParticules;
    public Slider wMusicLevel;
    public Slider wSoundLevel;

    public void GoToStartMenu(){
        optionMenu.SetActive(false);
        startMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void GoToMainMenu(){
        applyUI2Options();
        optionMenu.SetActive(false);
        startMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    
    public void GoToOptionMenu(){
        optionMenu.SetActive(true);
        startMenu.SetActive(false);
        mainMenu.SetActive(false);
    }
        
    public void applyUI2Options(){
        options.board = (new List<string>{"small","medium","large"})[wBoard.value];
        options.piecePicker = (new List<string>{"simple","complex","dude?"})[wPieces.value];
        options.speed = (new List<string>{"slow","normal","fast"})[wSpeed.value];
        if(wRotationSpeed.value == 1f)
            options.rotationSpeed = 0f;
        else
            options.rotationSpeed = 180f + wRotationSpeed.value*360f;
        options.particules = wParticules.isOn;
        options.musicLevel = wMusicLevel.value;
        options.soundLevel = wSoundLevel.value;
        options.Save();
    }
    
    public void applyOption2UI(){
        wBoard.value = (new Dictionary<string,int>{{"small",0},{"medium",1},{"large",2}})[options.board];
        wPieces.value = (new Dictionary<string,int>{{"simple",0},{"complex",1},{"dude",2}})[options.piecePicker];
        wSpeed.value = (new Dictionary<string,int>{{"slow",0},{"normal",1},{"fast",2}})[options.speed];
        if(options.rotationSpeed == 0f)
            wRotationSpeed.value = 1f;
        else
            wRotationSpeed.value = (options.rotationSpeed - 180f)/360f;
        wParticules.isOn = options.particules;
        wMusicLevel.value = options.musicLevel;
        wSoundLevel.value = options.soundLevel;
    }
    
    public void StartGame (){
        applyUI2Options();
        SceneManager.LoadScene("game");
    }
    
    private void Start(){
        options = GameApplication.GetOptions();
        applyOption2UI();
    }
    
    
	public void Quit () 
	{
		#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
		#else
            Application.Quit();
		#endif
	}
}
