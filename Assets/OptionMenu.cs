using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionMenu : MonoBehaviour {
    public AudioSource audioClick;
    public AudioMixer mixer;
    public Slider wRotationSpeed;
    public Toggle wParticules;
    public Slider wMusicLevel;
    public Slider wSoundLevel;
    
    public event EventHandler<EventArgs> goingBack = delegate {};
    
    Options options;
    float soundLevel;
    
    public void Back(){
        ApplyUI2Options();
        gameObject.SetActive(false);
        goingBack(this,new EventArgs());
    }
    
    public Options Options {
        set {
            if(options != null)
                throw new Exception("options has already been assigned");
            options = value;
            ApplyOption2UI();
        }
    }
    
    public void OnSoundVolumeChange(){
        options.soundLevel = wSoundLevel.value;
        mixer.SetFloat("SoundVolume",options.SoundVolume);
    }
    
    public void OnMusicVolumeChange(){
        options.musicLevel = wMusicLevel.value;
        mixer.SetFloat("MusicVolume",options.MusicVolume);
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
        soundLevel = options.soundLevel;
    }
    
    void Update(){
        if(Input.GetButtonDown("Cancel"))
            Back();
        
        if(Input.GetMouseButtonDown(0))
            soundLevel = wSoundLevel.value;
        if(Input.GetMouseButtonUp(0) && soundLevel != wSoundLevel.value)
            audioClick.Play();
    }
}
