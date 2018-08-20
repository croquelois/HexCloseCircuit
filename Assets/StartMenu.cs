using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour {
    public Dropdown wBoard;
    public Dropdown wPieces;
    public Dropdown wSpeed;
    
    public event EventHandler<EventArgs> goingBack = delegate {};
    
    Options options;
    
    public void Back(){
        applyUI2Options();
        gameObject.SetActive(false);
        goingBack(this,new EventArgs());
    }
    
    public void Launch(){
        applyUI2Options();
        SceneManager.LoadScene("game");
    }
    
    public Options Options {
        set { 
            options = value;
            applyOption2UI();
        }
    }
        
    void applyUI2Options(){
        options.board = (new List<string>{"small","medium","large"})[wBoard.value];
        options.piecePicker = (new List<string>{"simple","complex","dude?"})[wPieces.value];
        options.speed = (new List<string>{"slow","normal","fast"})[wSpeed.value];
        options.Save();
    }
    
    void applyOption2UI(){
        wBoard.value = (new Dictionary<string,int>{{"small",0},{"medium",1},{"large",2}})[options.board];
        wPieces.value = (new Dictionary<string,int>{{"simple",0},{"complex",1},{"dude",2}})[options.piecePicker];
        wSpeed.value = (new Dictionary<string,int>{{"slow",0},{"normal",1},{"fast",2}})[options.speed];
    }
    
    void Update(){
        if(Input.GetButtonDown("Cancel"))
            Back();
    }
}
