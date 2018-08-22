using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScoreView : MonoBehaviour {
    public void Set(List<HighScore> highScores){
        for(int i=0;i<10;i++){
            Transform row = transform.GetChild(i);
            Text name = row.Find("Name").GetComponent<Text>();
            Text score = row.Find("Score").GetComponent<Text>();
            if(highScores.Count > i){
                name.text = highScores[i].Name;
                score.text = ""+highScores[i].Score;
            }else{
                name.text = "";
                score.text = "-";
            }
        }
    }
}
