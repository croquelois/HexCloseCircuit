using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockView : MonoBehaviour {
    public BlockViewSolid solid;
    public BlockViewLiquid liquid;
    public HexDirection dir1;
    public HexDirection dir2;

    private void Start () {
        Triangulate();
    }
    public void Triangulate(){
        solid.Triangulate(dir1, dir2);
        liquid.Triangulate(dir1, dir2);
    }
    void Awake () {
    }
}
