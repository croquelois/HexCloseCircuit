using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Collections.Generic;

public class HighScore {
    public string Name { get; set; }
    public int Score { get; set; }
    
    public HighScore(string name, int score){
        Name = name;
        Score = score;
    }
}

[System.Serializable]
public class HighScores {
    string path = null;
    Dictionary<string,List<HighScore>> allHighScores;
    
    public HighScores(string path){
        try {
            allHighScores = JsonUtility.FromJson<Dictionary<string,List<HighScore>>>(File.ReadAllText(path));
        }catch(Exception ex){
            Debug.Log(ex);
            allHighScores = new Dictionary<string,List<HighScore>>();
        }
        this.path = path;
    }
    
    string constructKey(string boardSize, string piecePicker, string speed){
        return boardSize + "-" + piecePicker + "-" + speed;
    }
    public void Add(string boardSize, string piecePicker, string speed, string name, int score){
        string key = constructKey(boardSize,piecePicker,speed);
        List<HighScore> scores;
        if(!allHighScores.TryGetValue(key,out scores))
            scores = new List<HighScore>();
        int i;
        for(i=0;i<10;i++)
            if(scores[i].Score < score)
                break;
        if(i == 10)
            throw new Exception("Try to insert a score but it's not a highscore");
        for(int j=9;j>i;j--)
            scores[j] = scores[j-1];
        scores[i] = new HighScore(name, score);
        Save();
    }
    
    public bool isIn(string boardSize, string piecePicker, string speed, string name, int score){
        List<HighScore> scores;
        if(!allHighScores.TryGetValue(constructKey(boardSize,piecePicker,speed),out scores))
            return true;
        if(scores.Count < 10)
            return true;
        return (scores[9].Score < score);
    }
    
    public List<HighScore> Get(string boardSize, string piecePicker, string speed){
        List<HighScore> scores;
        if(!allHighScores.TryGetValue(constructKey(boardSize,piecePicker,speed),out scores))
            scores = new List<HighScore>();
        return scores;
    }
    
    public void Save(){
        File.WriteAllText(this.path, JsonUtility.ToJson(allHighScores));
    }
}