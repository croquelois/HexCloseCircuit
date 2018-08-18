using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameCamera : MonoBehaviour {
    Transform swivel;
    Transform stick;
    float stickMinZoom = 0.0f;
    float stickMaxZoom = 100.0f;
    float swivelMinZoom = 0.0f;
    float swivelMaxZoom = 70.0f;
    float current = 0f;
    float target = 1f;
    float speed = 0.5f;
    
    public float Duration {
        get {
            return 1f / speed;
        }
    }
    
    void Update(){
        if(current == target)
            return;
        float delta = Mathf.Sign(target - current)*speed;
        current = Mathf.Clamp01(current + delta*Time.deltaTime);
        
        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, current);
        stick.localPosition = new Vector3(0f, 0f, distance);
        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, current);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
    
    void Awake(){
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }
    
    void Start () {
        float width = (float)GameApplication.GetOptions().BoardWidth;
        float height = (float)GameApplication.GetOptions().BoardHeight;
        float x = (width-1f)*HexMetrics.innerRadius;
        float z = (height-1f)*0.5f*(HexMetrics.innerRadius+HexMetrics.outerRadius);
        float dz = 40f - height * 5f;
        transform.localPosition = new Vector3(x,23f,z + dz);
        stickMaxZoom = -12.5f*height;
    }
}
