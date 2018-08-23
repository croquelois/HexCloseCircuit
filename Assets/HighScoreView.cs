using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScoreView : MonoBehaviour {
    HighScores highScores = null;
    InputField input = null;
    HighScore current = null;
    bool firstFrame = true;
    
    void Set(List<HighScore> highScores){
        for(int i=0;i<10;i++){
            Transform row = transform.GetChild(i);
            Text pos = row.Find("Pos").GetComponent<Text>();
            Text name = row.Find("Name").GetComponent<Text>();
            Text score = row.Find("Score").GetComponent<Text>();
            pos.text = ""+(i+1)+".";
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
        if(input != null && current != null){
            current.Name = input.text;
            highScores.Save();
        }
    }
    public void GameOver(string board, string piece, string speed, int score){
        highScores = new HighScores(Path.Combine(Application.persistentDataPath, "scores.json"));
        List<HighScore> list;
        int posNew = highScores.GetAndAdd(board,piece,speed,score,out list);
        Set(list);
        if(posNew >= 0){
            Transform row = transform.GetChild(posNew);
            Image rowBackground = row.GetComponent<Image>();
            Transform name = row.Find("Name");
            input = name.gameObject.AddComponent(typeof(InputField)) as InputField;
            current = list[posNew];
            input.textComponent = name.GetComponent<Text>();
            Color color = rowBackground.color;
            color.a = 0.25f;
            rowBackground.color = color;
        }
        
    }
    public void ViewOnly(string board, string piece, string speed){
        highScores = new HighScores(Path.Combine(Application.persistentDataPath, "scores.json"));
        List<HighScore> list = highScores.Get(board,piece,speed);
        Set(list);
    }
    
    void Update(){
        if(input != null && firstFrame == true){
            firstFrame = false;
            EventSystem.current.SetSelectedGameObject(input.gameObject);
            input.ActivateInputField();
        }
    }
}
