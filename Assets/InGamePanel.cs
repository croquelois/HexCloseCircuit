using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : MonoBehaviour {
    public Text score;
    public Text life;
    public Slider timer;
    
    float blinkSpeed = 1f;
    
    float scoreBlink = 0f;
    int scoreValue = 0;
    
    float lifeBlink = 0f;
    int lifeValue = 0;
    
    public float BlinkSpeed {
        get { return blinkSpeed; } 
    }
    
    public void SetScore(int v, bool noBlink=false){
        scoreBlink = noBlink ? 0f : 1f;
        scoreValue = v;
        if(noBlink)
            score.text = "Score: " + scoreValue;
    }
    
    public void SetLife(int v, bool noBlink=false){
        lifeBlink = noBlink ? 0f : 1f;
        lifeValue = v;
        if(noBlink)
            life.text = "Life: " + lifeValue;
    }
    
    public void SetTimer(float v){
        timer.value = 1f-v;
    }
    
    void Start () {
        score.text = "Score: " + 0;
        life.text = "Life: " + 0;
        timer.value = 1f;
    }

    void Update () {
        if(scoreBlink > 0f){
            float oldScoreBlink = scoreBlink;
            scoreBlink = Mathf.Max(0f,scoreBlink - Time.deltaTime*blinkSpeed);
            if(oldScoreBlink >= 0.5f && scoreBlink < 0.5f)
                score.text = "Score: " + scoreValue;
            Color c = score.color;
            c.a = 0.5f*(Mathf.Sin(Mathf.PI * (0.5f - 2f*scoreBlink)) + 1f);
            score.color = c;
        }
        if(lifeBlink > 0f){
            float oldLifeBlink = lifeBlink;
            lifeBlink = Mathf.Max(0f,lifeBlink - Time.deltaTime*blinkSpeed);
            if(oldLifeBlink >= 0.5f && lifeBlink < 0.5f)
                life.text = "Life: " + lifeValue;
            Color c = life.color;
            c.a = 0.5f*(Mathf.Sin(Mathf.PI * (0.5f - 2f*lifeBlink)) + 1f);
            life.color = c;
        }
    }
}
