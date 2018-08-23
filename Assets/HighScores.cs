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

public class HighScores {
    string path = null;
    Dictionary<string,List<HighScore>> allHighScores;
    
    public HighScores(string path){
        List<HighScore> scores;
        allHighScores = new Dictionary<string,List<HighScore>>();
        string str;
        try {
            str = File.ReadAllText(path);
            string[] lines = str.Split(new char[]{'\n'});
            foreach(string line in lines){
                string[] part = line.Split(new char[]{' '},3);
                string key = part[0];
                int score = Int32.Parse(part[1]);
                string name = part[2];
                if(!allHighScores.TryGetValue(key, out scores)){
                    scores = new List<HighScore>();
                    allHighScores[key] = scores;
                }
                allHighScores[key].Add(new HighScore(name, score));
            }
        }catch(Exception ex){
            Debug.Log(ex);
            //allHighScores["medium-complex-normal"] = new List<HighScore>{ new HighScore("Croq",150), new HighScore("Foo",100) };
        }
        this.path = path;
    }
    
    string constructKey(string boardSize, string piecePicker, string speed){
        return boardSize + "-" + piecePicker + "-" + speed;
    }
    public int GetAndAdd(string boardSize, string piecePicker, string speed, int score, out List<HighScore> scores){
        scores = Get(boardSize,piecePicker,speed);
        int i;
        for(i=0;i<scores.Count;i++)
            if(scores[i].Score < score)
                break;
        if(i == 10)
            return -1;
        if(scores.Count < 10)
            scores.Add(null);
        for(int j=scores.Count-1;j>i;j--)
            scores[j] = scores[j-1];
        scores[i] = new HighScore("", score);
        return i;
    }
    
    public bool isIn(string boardSize, string piecePicker, string speed, string name, int score){
        List<HighScore> scores = Get(boardSize,piecePicker,speed);
        if(scores.Count < 10)
            return true;
        return (scores[9].Score < score);
    }
    
    public List<HighScore> Get(string boardSize, string piecePicker, string speed){
        List<HighScore> scores;
        string key = constructKey(boardSize,piecePicker,speed);
        if(!allHighScores.TryGetValue(key, out scores)){
            scores = new List<HighScore>();
            allHighScores[key] = scores;
        }
        return scores;
    }
    
    public void Save(){
        List<string> lines = new List<string>();
        foreach(KeyValuePair<string, List<HighScore>> entry in allHighScores){
            string key = entry.Key;
            foreach(HighScore hs in entry.Value)
                lines.Add(key + " " + hs.Score + " " + hs.Name);
        }
        File.WriteAllText(this.path, String.Join("\n",lines.ToArray()));
    }
}