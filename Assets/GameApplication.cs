using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;

public class GameApplication {
    static Options options;
    
    static public void LoadOptions(){
        options = Options.Load(Path.Combine(Application.persistentDataPath, "options.json"));
    }
    
    static public Options GetOptions(){
        if(options == null){
            //return new Options();
            throw new Exception("option not initialized");
        }
        return options;
    }
}