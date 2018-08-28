using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLoopFactory {
    BatchConfig batchConfig;
    Grid<System.Object> validPositions;
    
    public BoardLoopFactory(Grid<System.Object> validPositions){
        this.validPositions = validPositions;
    }
    
    public List<List<BlockModel>> ChunkIt(List<BlockModel> blocks, int nbChunk){
        // TODO: Chunk a big loop in multiple pieces
        return null;
    }
    
    public List<BlockModel> GenerateLoop(int len){
        if(len == 0)
            throw new Exception("Length should be at least 4");
        Grid<BlockModel> grid = new Grid<BlockModel>();
        List<BlockModel> ret = new List<BlockModel>();
        HexDirection dir1 = RandomDirection();
        HexDirection dir2 = RandomDirection();
        while(dir2 == dir1)
            dir2 = RandomDirection();
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
                    dir2 = RandomDirection();
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
    
    HexDirection RandomDirection(){
        return (HexDirection)UnityEngine.Random.Range(0, 6);
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
}
