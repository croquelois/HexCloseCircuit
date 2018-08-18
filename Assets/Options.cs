using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class Options {
    public string board = "medium";
    public string piecePicker = "complex";
    public string speed = "normal";
    
    public bool particules = true;
    public float rotationSpeed = 360f;
    public float soundLevel = 0.5f;
    public float musicLevel = 0.5f;
    
    public float SpeedFlt {
        get {
            return (new Dictionary<string, float> {
                {"slow", 8f},
                {"normal", 5f},
                {"fast", 3f}
            })[speed];
        }
    }
    
    public int BoardWidth {
        get {
            return (new Dictionary<string, int> {
                {"small", 5},
                {"medium", 15},
                {"large", 20}
            })[board];
        }
    }
    
    public int BoardHeight {
        get {
            return (new Dictionary<string, int> {
                {"small", 7},
                {"medium", 10},
                {"large", 15}
            })[board];
        }
    }
    
    public void Save(){
        string path = Path.Combine(Application.persistentDataPath, "options.json");
        File.WriteAllText(path, JsonUtility.ToJson(this));
    }
    
    static public Options Load(){
        string path = Path.Combine(Application.persistentDataPath, "options.json");
        try {
            return JsonUtility.FromJson<Options>(File.ReadAllText(path));
        }catch(Exception ex){
            Debug.Log(ex);
            return new Options();
        }
    }
}