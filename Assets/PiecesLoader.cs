using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesLoader {
    List<List<BlockModel>> pieces = new List<List<BlockModel>>();
    
    public void Load(){
        pieces.Add(new List<BlockModel>{
            new BlockModel(0,0,HexDirection.SW,HexDirection.E),
            new BlockModel(1,0,HexDirection.W,HexDirection.E),
            new BlockModel(2,0,HexDirection.W,HexDirection.SW)
        });
        pieces.Add(new List<BlockModel>{
            new BlockModel(0,0,HexDirection.SW,HexDirection.NE),
            new BlockModel(1,0,HexDirection.NW,HexDirection.SE),
            new BlockModel(0,1,HexDirection.SE,HexDirection.SW)
        });
        pieces.Add(new List<BlockModel>{
            new BlockModel(0,0,HexDirection.E,HexDirection.W)
        });
        pieces.Add(new List<BlockModel>{
            new BlockModel(0,0,HexDirection.E,HexDirection.SW)
        });
        pieces.Add(new List<BlockModel>{
            new BlockModel(0,0,HexDirection.E,HexDirection.SE)
        });
    }
    
    public List<BlockModel> Get(){
        return pieces[Random.Range(0, pieces.Count)];
    }
}
