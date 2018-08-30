using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

class StackValue {
    public Grid<BlockModel> grid;
    public Pos curPos;
    public HexDirection curDir;
    
    public StackValue(Grid<BlockModel> grid, Pos curPos, HexDirection curDir){
        this.grid = grid;
        this.curPos = curPos;
        this.curDir = curDir;
    }
}

public class BoardLoopFactory {
    BatchConfig batchConfig;
    List<Pos> validPositions;
    Grid<Pos> gridValid;
    Grid<BlockModel> grid;
    
    public BoardLoopFactory(List<Pos> validPositions){
        this.validPositions = validPositions;
        gridValid = new Grid<Pos>();
        foreach(Pos pos in validPositions)
            gridValid.Add(pos.x,pos.z,pos);
    }
    
    public List<List<BlockModel>> ChunkIt(List<BlockModel> blocks, int nbChunk){
        // TODO: Chunk a big loop in multiple pieces
        return null;
    }
    
    void Fill(){
        foreach(Pos pos in validPositions){
            if(grid.Exist(pos.x,pos.z))
                continue;
            HexDirection dir1 = HexDirectionHelper.Random();
            HexDirection dir2 = HexDirectionHelper.Random();
            while(dir2 == dir1)
                dir2 = HexDirectionHelper.Random();
            grid.Add(pos.x,pos.z,new BlockModel(pos.x,pos.z,dir1,dir2));
        }
    }
    
    void KeepOnly(int nb){
        Dictionary<int,List<List<BlockModel>>> list = new Dictionary<int,List<List<BlockModel>>>();
        Grid<BlockModel> open = new Grid<BlockModel>();
        int biggest = 0;
        foreach(Pos pos in validPositions){
            if(open.Get(pos.x,pos.z) != null)
                continue;
            List<BlockModel> blocks = CheckConnection(pos);
            foreach(BlockModel block in blocks)
                open.Add(block.x,block.z,block);
            int n = blocks.Count;
            if(n == 0)
                throw new Exception("Dev error");
            if(n > biggest)
                biggest = n;
            List<List<BlockModel>> blockss;
            if(!list.TryGetValue(n, out blockss)){
                blockss = new List<List<BlockModel>>();
                list.Add(n, blockss);
            }
            blockss.Add(blocks);
        }
        if(biggest == 0)
            throw new Exception("Dev error");
        grid.Clear();
        for(int n = biggest;n > 0;n--){
            List<List<BlockModel>> blockss;
            if(list.TryGetValue(n, out blockss)){
                foreach(List<BlockModel> blocks in blockss){
                    foreach(BlockModel block in blocks)
                        grid.Add(block.x,block.z,block);
                    if(--nb == 0)
                        return;
                }
            }
        }
    }
        
    public List<BlockModel> GenerateLoop1(){
        grid = new Grid<BlockModel>();
        for(int i=0;i<100;i++){
            Fill();
            KeepOnly(1);
        }
        return grid.GetList();
    }
    
    DateTime start;
    TimeSpan timeout = new TimeSpan(0,0,1);
    /*public List<BlockModel> GenerateLoop(){
        start = DateTime.Now;
        List<BlockModel> loop =  Generate(new List<BlockModel>(), validPositions[Random.Range(0,validPositions.Count)], HexDirectionHelper.Random());
        TimeSpan timeItTook = DateTime.Now - start;
        Debug.Log(timeItTook);
        return loop;
    }*/
    
    public List<BlockModel> GenerateLoop(){
        int nbFail = 0;
        int nbOk = 0;
        double sum = 0;
        double sumSq = 0;
        List<BlockModel> loop = new List<BlockModel>();
        int length = 5; //Mathf.RoundToInt(Mathf.Sqrt((float)validPositions.Count)*4f);
        for(int i=0;i<100;i++){
            Pos pos;
            HexDirection dir;
            while(true) {
                pos = validPositions[Random.Range(0,validPositions.Count)];
                dir = HexDirectionHelper.Random();
                Pos prev = GetNextPos(pos, dir.Opposite());
                if(gridValid.Exist(prev.x, prev.z))
                    break;
            }
            start = DateTime.Now;
            try {
                List<BlockModel> newLoop = Generate(pos, dir, length);
                if(newLoop.Count > loop.Count)
                    loop = newLoop;
            }catch(Exception ex){
                //Debug.Log(ex);
                nbFail++;
                continue;
            }
            nbOk++;
            TimeSpan timeItTook = DateTime.Now - start;
            double s = timeItTook.TotalSeconds;
            sum += s;
            sumSq += s*s;
        }
        double avg = sum/(double)nbOk;
        double stdev = Math.Sqrt(sumSq/(double)nbOk - avg*avg);
        Debug.Log("nbFail:" + nbFail);
        Debug.Log("nbOk:" + nbOk);
        Debug.Log("avg:" + avg);
        Debug.Log("stdev:" + stdev);
        return loop;
    }
    
    public List<BlockModel> Generate(Pos curPos, HexDirection curDir, int length){
        Stack<StackValue> stack = new Stack<StackValue>();
        stack.Push(new StackValue(new Grid<BlockModel>(), curPos, curDir));
        while(stack.Count > 0){
            StackValue val = stack.Pop();
            Grid<BlockModel> grid = val.grid;
            curPos = val.curPos;
            curDir = val.curDir;
            TimeSpan timeItTake = DateTime.Now - start;
            if(timeItTake > timeout)
                throw new Exception("Too slow !");
            List<HexDirection> possible = new List<HexDirection>();
            bool hasLoop = false;
            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++){
                if(curDir.Opposite() == dir)
                    continue;
                HexDirection opp = dir.Opposite();
                Pos pos = GetNextPos(curPos, dir);
                bool isOkay = true;
                bool isLoop = false;
                BlockModel block = grid.Get(pos.x, pos.z);
                if(block != null){
                    if(block.dir1 == opp || block.dir2 == opp){
                        hasLoop = isLoop = true;
                    }else{
                        isOkay = false;
                    }
                }
                if(!isOkay)
                    continue;
                isOkay = gridValid.Exist(pos.x, pos.z);
                if(!isOkay)
                    continue;
                if(isLoop){
                    List<BlockModel> newList = grid.GetList();
                    newList.Add(new BlockModel(curPos.x, curPos.z, curDir.Opposite(), dir));
                    if(newList.Count < length)
                        continue;
                    return newList;
                }
                possible.Add(dir);
            }
            if(hasLoop)
                continue;
            int n = possible.Count;
            while(n > 1){
                n--;
                int k = Random.Range(0,n);
                HexDirection value = possible[k];
                possible[k] = possible[n];
                possible[n] = value;
            }
            foreach(HexDirection dir in possible){
                Grid<BlockModel> newGrid = new Grid<BlockModel>(grid);
                newGrid.Add(curPos.x, curPos.z, new BlockModel(curPos.x, curPos.z, curDir.Opposite(), dir));
                stack.Push(new StackValue(newGrid, GetNextPos(curPos, dir), dir));
            }
        }
        return null;
    }
    
    public List<BlockModel> GenerateRec(Grid<BlockModel> grid, Pos curPos, HexDirection curDir){
        TimeSpan timeItTake = DateTime.Now - start;
        if(timeItTake > timeout)
            throw new Exception("Too slow !");
        List<HexDirection> possible = new List<HexDirection>();
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++){
            if(curDir.Opposite() == dir)
                continue;
            HexDirection opp = dir.Opposite();
            Pos pos = GetNextPos(curPos, dir);
            bool isOkay = true;
            bool isLoop = false;
            BlockModel block = grid.Get(pos.x, pos.z);
            if(block != null){
                if(block.dir1 == opp || block.dir2 == opp){
                    isLoop = true;
                }else{
                    isOkay = false;
                }
            }
            if(!isOkay)
                continue;
            isOkay = gridValid.Exist(pos.x, pos.z);
            if(!isOkay)
                continue;
            if(isLoop){
                List<BlockModel> newList = grid.GetList();
                newList.Add(new BlockModel(curPos.x, curPos.z, curDir.Opposite(), dir));
                return newList;
            }
            possible.Add(dir);
        }
        int n = possible.Count;
        while(n > 1){
            n--;
            int k = Random.Range(0,n);
            HexDirection value = possible[k];
            possible[k] = possible[n];
            possible[n] = value;
        }
        foreach(HexDirection dir in possible){
            Grid<BlockModel> newGrid = new Grid<BlockModel>(grid);
            newGrid.Add(curPos.x, curPos.z, new BlockModel(curPos.x, curPos.z, curDir.Opposite(), dir));
            List<BlockModel> retList = GenerateRec(newGrid, GetNextPos(curPos, dir), dir);
            if(retList != null)
                return retList;
        }
        return null;
    }
    
    public List<BlockModel> Generate(List<BlockModel> list, Pos curPos, HexDirection curDir){
        TimeSpan timeItTake = DateTime.Now - start;
        if(timeItTake > timeout)
            throw new Exception("Too slow !");
        List<HexDirection> possible = new List<HexDirection>();
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++){
            if(curDir.Opposite() == dir)
                continue;
            HexDirection opp = dir.Opposite();
            Pos pos = GetNextPos(curPos, dir);
            bool isOkay = true;
            bool isLoop = false;
            foreach(BlockModel block in list){
                if(block.x == pos.x && block.z == pos.z){
                    if(block.dir1 == opp || block.dir2 == opp){
                        isLoop = true;
                        break;
                    }
                    isOkay = false;
                    break;
                }
            }
            if(!isOkay)
                continue;
            isOkay = gridValid.Exist(pos.x, pos.z);
            /*isOkay = false;
            foreach(Pos validPos in validPositions){
                if(validPos.x == pos.x && validPos.z == pos.z)
                    isOkay = true;
            }*/
            if(!isOkay)
                continue;
            if(isLoop){
                List<BlockModel> newList = new List<BlockModel>(list);
                newList.Add(new BlockModel(curPos.x, curPos.z, curDir.Opposite(), dir));
                return newList;
            }
            possible.Add(dir);
        }
        int n = possible.Count;
        while(n > 1){
            n--;
            int k = Random.Range(0,n);
            HexDirection value = possible[k];
            possible[k] = possible[n];
            possible[n] = value;
        }
        foreach(HexDirection dir in possible){
            List<BlockModel> newList = new List<BlockModel>(list);
            newList.Add(new BlockModel(curPos.x, curPos.z, curDir.Opposite(), dir));
            List<BlockModel> retList = Generate(newList, GetNextPos(curPos, dir), dir);
            if(retList != null)
                return retList;
        }
        return null;
    }
        
    List<BlockModel> CheckConnection(Pos pos){
        List<BlockModel> blocks = new List<BlockModel>();;
        BlockModel block = grid.Get(pos.x, pos.z);
        if(block == null)
            return blocks;
        
        blocks.Add(block);
        BlockWithDir next = new BlockWithDir(block, block.dir2);
        while((next = GetNext(next)) != null){
            if(block == next.Block)
                break;
            blocks.Add(next.Block);
        }
        if(next != null)
            return blocks;
        next = new BlockWithDir(block, block.dir1);
        while((next = GetNext(next)) != null)
            blocks.Add(next.Block);
        return blocks;
    }
    Pos GetNextPos(Pos pos, HexDirection dir){
        int x = pos.x;
        int z = pos.z;
        if(dir == HexDirection.E)
            x += 1;
        if(dir == HexDirection.W)
            x -= 1;
        if(dir == HexDirection.NE){
            x += (z%2);
            z += 1;
        }
        if(dir == HexDirection.NW){
            x += (z%2) - 1;
            z += 1;
        }
        if(dir == HexDirection.SE){
            x += (z%2);
            z -= 1;
        }
        if(dir == HexDirection.SW){
            x += (z%2) - 1;
            z -= 1;
        }
        return new Pos(x,z);
    }
    
    BlockWithDir GetNext(BlockWithDir prev){
        HexDirection dir = prev.Dir;
        Pos pos = GetNextPos(new Pos(prev.Block.x, prev.Block.z), dir);
        BlockModel next = grid.Get(pos.x, pos.z);
        if(next == null)
            return null;
        HexDirection opp = dir.Opposite();
        if(next.dir1 == opp)
            return new BlockWithDir(next,next.dir2);
        if(next.dir2 == opp)
            return new BlockWithDir(next,next.dir1);
        return null;
    }
}
