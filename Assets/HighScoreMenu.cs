using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScoreMenu : MonoBehaviour {
    public Dropdown wBoard;
    public Dropdown wPieces;
    public Dropdown wSpeed;
    public HighScoreView view;
    public event EventHandler<EventArgs> goingBack = delegate {};
    
    Options options;
    
    public void Back(){
        gameObject.SetActive(false);
        goingBack(this,new EventArgs());
    }
    
    void ApplyOption2UI(){
        wBoard.value = (new Dictionary<string,int>{{"small",0},{"medium",1},{"large",2}})[options.board];
        wPieces.value = (new Dictionary<string,int>{{"simple",0},{"complex",1},{"dude",2}})[options.piecePicker];
        wSpeed.value = (new Dictionary<string,int>{{"slow",0},{"normal",1},{"fast",2}})[options.speed];
    }
    
    public void OnChange(){
        string board = (new List<string>{"small","medium","large"})[wBoard.value];
        string piecePicker = (new List<string>{"simple","complex","dude?"})[wPieces.value];
        string speed = (new List<string>{"slow","normal","fast"})[wSpeed.value];
        view.ViewOnly(board,piecePicker,speed);
    }
    
    public Options Options {
        set { 
            options = value;
            ApplyOption2UI();
            OnChange();
        }
    }
}
