﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PosAndObject {
    public Vector3 pos;
    public Transform obj;
    public BlockModel model;
};

public class Piece : MonoBehaviour {
    public Transform piecePrefab;
    public Transform bombPrefab;
    List<PosAndObject> elem = new List<PosAndObject>();
    float r = 0f;
    float curR = 0f;
    bool isBomb = false;
    
    public bool IsBomb {
        get { return isBomb; }
    }
    
    public List<BlockModel> GetShadow(){
        List<BlockModel> list = new List<BlockModel>();
        int incR = Mathf.RoundToInt(r * 6f / 360f);
        foreach(PosAndObject pno in elem){
            Vector3 pos = (Quaternion.Euler(0, incR*360f/6f, 0)*pno.obj.localPosition + transform.localPosition);
            float X = pos.x;
            float Z = pos.z;
            float ro = HexMetrics.outerRadius;
            float ri = HexMetrics.innerRadius;
            int z = Mathf.RoundToInt(Z/(1.5f*ro));
            int x = Mathf.RoundToInt(X/(2f*ri) - (z%2==0?0.0f:0.5f));
            HexDirection dir1 = HexDirection.E;
            HexDirection dir2 = HexDirection.E;
            if(pno.model != null){
                dir1 = (HexDirection)((((int)pno.model.dir1) + incR) % 6);
                dir2 = (HexDirection)((((int)pno.model.dir2) + incR) % 6);
            }
            list.Add(new BlockModel(x, z, dir1, dir2));
        }
        return list;
    }
    
    public void IncRotation(float delta){
        r += delta*10f*60f;//*0.25f;
        if(r > 360f) r -= 360f;
        if(r < 0f) r += 360f;
    }
    
    void Update()
    {
        if(IsBomb){
            curR += Time.deltaTime*45f;
            transform.rotation = Quaternion.Euler(0, curR, 0);
            return;
        }
        if(curR == r)
            return;
        float rotationSpeed = GameApplication.GetOptions().rotationSpeed;
        if(rotationSpeed == 0f){
            curR = r;
        }else{
            float d = Time.deltaTime*rotationSpeed;
            float a = Mathf.Abs(r - curR);
            float w = 1f;
            if(a > 180f){
                w = -1f;
                a -= 180f;
            }
            if(a < d)
                curR = r;
            else{
                curR += d*w*Mathf.Sign(r - curR);
                if(curR > 360f) curR -= 360f;
                if(curR < 0f) curR += 360f; 
            }
        }
        transform.rotation = Quaternion.Euler(0, curR, 0);
    }
    
    public void MoveTo(Vector3 pos){
        transform.localPosition = pos;
    }
    
    public void New(List<BlockModel> blocks){
        isBomb = (blocks.Count == 0);
        Vector3 oldPos = transform.localPosition;
        curR = r = 0;
        transform.rotation = Quaternion.Euler(0, r, 0);
        transform.localPosition = new Vector3(0f,0f,0f);
        elem.Clear();
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
        if(IsBomb){
            SetBomb();
        }else{
            foreach(BlockModel block in blocks)
                Add(block);
            Vector3 center = new Vector3();
            foreach(PosAndObject pno in elem)
                center += pno.pos;
            center /= elem.Count;
            foreach(PosAndObject pno in elem){
                pno.pos -= center;
                pno.obj.localPosition = pno.pos;
            }
        }
        transform.localPosition = oldPos;
        curR = r = Random.Range(0,6)*60f;
        transform.rotation = Quaternion.Euler(0, r, 0);
    }
    
    Transform SetBomb(){
        Transform instance = Instantiate(bombPrefab);
        PosAndObject pno = new PosAndObject();
        pno.pos = instance.localPosition = new Vector3(0f,10f,0f);
        pno.obj = instance;
        instance.SetParent(transform);
        elem.Add(pno);
        return pno.obj;
    }
    
    Transform Add(BlockModel model){
        Transform instance = Instantiate(piecePrefab);
        BlockView block = instance.gameObject.GetComponent<BlockView>();
        block.dir1 = model.dir1;
        block.dir2 = model.dir2;
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        PosAndObject pno = new PosAndObject();
        pno.pos = instance.localPosition = new Vector3(2f*ri*(((float)model.x)+(model.z%2==0?0.0f:0.5f)),0.5f,1.5f*ro*(float)model.z);
        pno.obj = instance;
        pno.model = model;
        instance.SetParent(transform);
        elem.Add(pno);
        return pno.obj;
    }
}

