using UnityEngine;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScoreView : MonoBehaviour {
    HighScores highScores;
    bool newScore = false;
    
    void Set(List<HighScore> highScores){
        for(int i=0;i<10;i++){
            Transform row = transform.GetChild(i);
            Text pos = row.Find("Pos").GetComponent<Text>();
            Text name = row.Find("Name").GetComponent<Text>();
            Text score = row.Find("Score").GetComponent<Text>();
            pos.text = ""+i+".";
            if(highScores.Count > i){
                name.text = highScores[i].Name;
                score.text = ""+highScores[i].Score;
            }else{
                name.text = "";
                score.text = "-";
            }
        }
    }
    public void Save(){
        if(newScore)
            highScores.Save();
    }
    public void GameOver(string board, string piece, string speed, int score){
        highScores = new HighScores(Path.Combine(Application.persistentDataPath, "scores.json"));
        List<HighScore> list;
        int posNew = highScores.GetAndAdd(board,piece,speed,score,out list);
        Set(list);
        newScore = (posNew >= 0);
        if(newScore){
            Debug.Log(posNew);
            Transform name = transform.GetChild(posNew).Find("Name");
            InputField input = name.gameObject.AddComponent(typeof(InputField)) as InputField;
            input.textComponent = name.GetComponent<Text>();
            input.ActivateInputField();
        }
        
    }
    public void ViewOnly(string board, string piece, string speed){
        highScores = new HighScores(Path.Combine(Application.persistentDataPath, "scores.json"));
        List<HighScore> list = highScores.Get(board,piece,speed);
        Set(list);
    }
    
    void Start(){
    }
}
