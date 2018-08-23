using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour {
    public Slider wRotationSpeed;
    public Toggle wParticules;
    public Slider wMusicLevel;
    public Slider wSoundLevel;
    
    public event EventHandler<EventArgs> goingBack = delegate {};
    
    Options options;
    
    public void Back(){
        ApplyUI2Options();
        gameObject.SetActive(false);
        goingBack(this,new EventArgs());
    }
    
    public Options Options {
        set { 
            options = value;
            ApplyOption2UI();
        }
    }
        
    void ApplyUI2Options(){
        if(wRotationSpeed.value == 1f)
            options.rotationSpeed = 0f;
        else
            options.rotationSpeed = 180f + wRotationSpeed.value*360f;
        options.particules = wParticules.isOn;
        options.musicLevel = wMusicLevel.value;
        options.soundLevel = wSoundLevel.value;
        options.Save();
    }
    
    void ApplyOption2UI(){
        if(options.rotationSpeed == 0f)
            wRotationSpeed.value = 1f;
        else
            wRotationSpeed.value = (options.rotationSpeed - 180f)/360f;
        wParticules.isOn = options.particules;
        wMusicLevel.value = options.musicLevel;
        wSoundLevel.value = options.soundLevel;
    }
    
    void Update(){
        if(Input.GetButtonDown("Cancel"))
            Back();
    }
}
