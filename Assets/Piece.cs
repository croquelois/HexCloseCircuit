using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PosAndObject {
    public Vector3 pos;
    public Transform obj;
    public BlockModel model;
};

public class Piece : MonoBehaviour {
    public Transform piecePrefab;
    List<PosAndObject> elem = new List<PosAndObject>();
    float r = 0f;
    
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
            HexDirection dir1 = (HexDirection)((((int)pno.model.dir1) + incR) % 6);
            HexDirection dir2 = (HexDirection)((((int)pno.model.dir2) + incR) % 6);
            list.Add(new BlockModel(x, z, dir1, dir2));
        }
        return list;
    }
    
    public void IncRotation(float delta){
        r += delta*10f*60f*0.25f;
        if(r > 360f) r -= 360f;
        if(r < 0f) r += 360f;
        transform.rotation = Quaternion.Euler(0, r, 0);
    }
    
    public void MoveTo(Vector3 pos){
        transform.localPosition = pos;
    }
    
    public void New(List<BlockModel> blocks){
        Vector3 oldPos = transform.localPosition;
        r = 0;
        transform.rotation = Quaternion.Euler(0, r, 0);
        transform.localPosition = new Vector3(0f,0f,0f);
        elem.Clear();
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
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
        transform.localPosition = oldPos;
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

