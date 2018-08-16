using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject startMenu;
    public GameObject optionMenu;
    

    public void GoToStartMenu(){
        optionMenu.SetActive(false);
        startMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void GoToMainMenu(){
        optionMenu.SetActive(false);
        startMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    
    public void GoToOptionMenu(){
        optionMenu.SetActive(true);
        startMenu.SetActive(false);
        mainMenu.SetActive(false);
    }
    
    public void SetBoardSize(int i){
        switch(i){
            case 0:
                Options.boardWidth = 7;
                Options.boardHeight = 5;
                break;
            case 1:
                Options.boardWidth = 15;
                Options.boardHeight = 10;
                break;
            case 2:
                Options.boardWidth = 20;
                Options.boardHeight = 15;
                break;
            default:
                throw new Exception("unavailable option");
        }
    }
    
    public void SetPiecePicker(int i){
        switch(i){
            case 0:
                Options.piecePicker = "simple";
                break;
            case 1:
                Options.piecePicker = "complex";
                break;
            case 2:
                Options.piecePicker = "dude?";
                break;
            default:
                throw new Exception("unavailable option");
        }
    }
    
    public void SetSpeed(int i){
        switch(i){
            case 0:
                Options.speed = 8f;
                break;
            case 1:
                Options.speed = 5f;
                break;
            case 2:
                Options.speed = 3f;
                break;
            default:
                throw new Exception("unavailable option");
        }
    }
    
    public void StartGame (){
        SceneManager.LoadScene("game");
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
