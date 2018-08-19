using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockView : MonoBehaviour {
    public BlockViewSolid solid;
    public BlockViewLiquid liquid;
    public ParticleSystem explosion;
    public HexDirection dir1;
    public HexDirection dir2;
    
    public Material normalMat;
    public Material highlightMat;
    bool exploded = false;
    

    private void Start () {
        Triangulate();
    }
    public void Triangulate(){
        solid.Triangulate(dir1, dir2);
        liquid.Triangulate(dir1, dir2);
    }
    public float ExplosionDuration {
        get {
            return explosion.main.duration;
        }
    }
    public void Explode(){
        exploded = true;
        Destroy(solid.gameObject);
        Destroy(liquid.gameObject);
        if(GameApplication.GetOptions().particules)
            explosion.Play();
        Destroy(gameObject, ExplosionDuration);
    }
    public void Highlight(bool val){
        if(exploded)
            return;
        Material[] mats = solid.GetComponent<MeshRenderer>().materials;
        mats[0] = (val ? highlightMat : normalMat);
        solid.GetComponent<MeshRenderer>().materials = mats;
    }
    void Awake () {
    }
}
