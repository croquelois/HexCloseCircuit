using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct BatchConfig {
    public int minSize;
    public List<float> nbByLength;
    public List<List<BlockModel>> mandatory;
}

public class PiecesLoader {
    BatchConfig batchConfig;
    List<List<BlockModel>> pieces = new List<List<BlockModel>>();
    
    public PiecesLoader(){
        switch(GameApplication.GetOptions().piecePicker){
            case "simple":
                batchConfig.minSize = 2;
                batchConfig.nbByLength = new List<float>{0.2f,4f,4f,1f};
                batchConfig.mandatory = new List<List<BlockModel>>();
                for(int i=0;i<3;i++)
                    batchConfig.mandatory.Add(new List<BlockModel>{ new BlockModel(0,0,HexDirection.E,HexDirection.W) });
                break;
            case "complex":
                batchConfig.minSize = 2;
                batchConfig.nbByLength = new List<float>{0.2f,4f,3f,2f,0.5f,0.25f,0.25f};
                batchConfig.mandatory = new List<List<BlockModel>>();
                for(int i=0;i<2;i++)
                    batchConfig.mandatory.Add(new List<BlockModel>{ new BlockModel(0,0,HexDirection.E,HexDirection.W) });
                break;
            case "dude?":
                batchConfig.minSize = 2;
                batchConfig.nbByLength = new List<float>{0.1f,2f,1f,3f,0.5f,0.25f,0.25f,0.10f,0.05f,1f};
                batchConfig.mandatory = new List<List<BlockModel>>();
                for(int i=0;i<2;i++){
                    batchConfig.mandatory.Add(new List<BlockModel>{ new BlockModel(0,0,HexDirection.E,HexDirection.W) });
                    batchConfig.mandatory.Add(new List<BlockModel>{ new BlockModel(0,0,HexDirection.E,HexDirection.NW) });
                    batchConfig.mandatory.Add(new List<BlockModel>{ new BlockModel(0,0,HexDirection.E,HexDirection.SW) });
                }
                break;
            default:
                throw new System.Exception("Unknow column picker");
        }
    }
    
    void MoveNextPos(int x, int z, HexDirection dir, out int nx, out int nz){
        int odd = z > 0 ? (z%2) : -(z%2);
        switch(dir){
            case HexDirection.E:
                nx = x + 1;
                nz = z;
                break;
            case HexDirection.W:
                nx = x - 1;
                nz = z;
                break;
            case HexDirection.NE:
                nx = x + odd;
                nz = z + 1;
                break;
            case HexDirection.NW:
                nx = x + odd - 1;
                nz = z + 1;
                break;
            case HexDirection.SE:
                nx = x + odd;
                nz = z - 1;
                break;
            case HexDirection.SW:
                nx = x + odd - 1;
                nz = z - 1;
                break;
            default:
                nx = x;
                nz = z;
                break;
        }
    }
    public List<BlockModel> Generate(int len){
        if(len == 0)
            return new List<BlockModel>();
        Grid<BlockModel> grid = new Grid<BlockModel>();
        List<BlockModel> ret = new List<BlockModel>();
        HexDirection dir1 = (HexDirection)Random.Range(0, 6);
        HexDirection dir2 = (HexDirection)Random.Range(0, 6);
        while(dir2 == dir1)
            dir2 = (HexDirection)Random.Range(0, 6);
        int x = 0;
        int z = 0;
        int nx,nz;
        BlockModel cur = new BlockModel(x,z,dir1,dir2);
        grid.Add(x,z,cur);
        MoveNextPos(x,z,dir1,out nx,out nz);
        grid.Add(nx,nz,cur); // to avoid edge case
        ret.Add(cur);
        MoveNextPos(x,z,dir2,out nx,out nz);
        for(int i=1;i<len;i++){
            x = nx;
            z = nz;
            dir1 = dir2.Opposite();
            int nbTryLeft = 25;
            do {
                dir2 = dir1;
                while(dir1 == dir2)
                    dir2 = (HexDirection)Random.Range(0, 6);
                MoveNextPos(x,z,dir2,out nx,out nz);
            }while(grid.Exist(nx,nz) && --nbTryLeft > 0);
            if(nbTryLeft == 0)
                return null;
            cur = new BlockModel(x,z,dir1,dir2);
            grid.Add(x,z,cur);
            ret.Add(cur);
        }
        return ret;
    }
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
    int RandomLength(){
        float v = Random.value;
        if(v < 0.4)
            return 1; // 40%
        if(v < 0.7)
            return 2; // 30%
        if(v < 0.9)
            return 3; // 20%
        if(v < 0.95)
            return 4; // 5%
        if(v < 0.98)
            return 5; // 3%
        return 6; // 2%
    }
    List<List<BlockModel>> PrepareBatch(){
        List<List<BlockModel>> ret = new List<List<BlockModel>>();
        for(int i=0;i<batchConfig.nbByLength.Count;i++){
            float n = batchConfig.nbByLength[i];
            while(n >= 1f){
                ret.Add(Generate(i));
                n -= 1f;
            }
            if(n > 0f && Random.value < n)
                ret.Add(Generate(i));
        }
        foreach(List<BlockModel> piece in batchConfig.mandatory)
            ret.Add(piece);
        return ret;
    }
    List<BlockModel> GetFromBatch(){
        if(pieces.Count < batchConfig.minSize)
            pieces = PrepareBatch();
        int p = Random.Range(0, pieces.Count);
        List<BlockModel> piece = pieces[p];
        pieces.RemoveAt(p);
        return piece;
        
    }
    public List<BlockModel> Get(){
        //return pieces[Random.Range(0, pieces.Count)];
        /*List<BlockModel> ret = null;
        while(ret == null)
            ret = Generate(RandomLength());
        return ret;*/
        return GetFromBatch();
    }
}
