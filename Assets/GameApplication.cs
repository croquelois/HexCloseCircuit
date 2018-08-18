using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class GameApplication {
    static Options options;
    
    static public Options GetOptions(){
        if(options == null)
            options = Options.Load();
        return options;
    }
}