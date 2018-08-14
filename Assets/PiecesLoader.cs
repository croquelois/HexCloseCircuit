using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesLoader {
    List<List<BlockModel>> pieces = new List<List<BlockModel>>();
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
        if(v < 0.3)
            return 1; // 30%
        if(v < 0.6)
            return 2; // 30%
        if(v < 0.8)
            return 3; // 20%
        if(v < 0.9)
            return 4; // 10%
        if(v < 0.9)
            return 5; // 5%
        return 6; // 5%
    }
    public List<BlockModel> Get(){
        //return pieces[Random.Range(0, pieces.Count)];
        List<BlockModel> ret = null;
        while(ret == null)
            ret = Generate(RandomLength());
        return ret;
    }
}
