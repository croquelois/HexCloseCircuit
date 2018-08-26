﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CheckConnectionResult {
    public List<BlockModel> blocks = new List<BlockModel>();
    public bool isLoop = false;
}

class BlockWithDir {
    public BlockModel Block { get; private set; }
    public HexDirection Dir { get; private set; }
    
    public BlockWithDir(BlockModel block, HexDirection dir){
        Block = block;
        Dir = dir;
    }
}

public class ScoreEventArgs : EventArgs {
    public int Score { get; private set; }
    public ScoreEventArgs(int score){
        Score = score;
    }
}

public class LifeEventArgs : EventArgs {
    public int Life { get; private set; }
    public LifeEventArgs(int life){
        Life = life;
    }
}

public class TimeEventArgs : EventArgs {
    public float Time { get; private set; }
    public TimeEventArgs(float time){
        Time = time;
    }
}

public class ListOfBlockEventArgs : EventArgs {
    public List<BlockModel> Blocks { get; private set; }
    public ListOfBlockEventArgs(List<BlockModel> blocks){
        Blocks = blocks;
    }
}

public class BoardModel {
    PiecesLoader pieces = new PiecesLoader();
    Grid<BlockModel> grid = new Grid<BlockModel>();
    int score = 0;
    int life = 3;
    float time = 0f;
    float totalTime = 0f;
    float speed = 5f;
    
    public float Time {
        get {
            return time;
        }
        set {
            time = value;
            updateTimer(this, new TimeEventArgs(value));
        }
    }
    
    public int Life {
        get {
            return life;
        }
        set {
            life = value;
            updateLife(this, new LifeEventArgs(value));
            if(value == 0)
                gameOver(this, new EventArgs());
        }
    }
    
    public int Score {
        get {
            return score;
        }
        set {
            score = value;
            updateScore(this, new ScoreEventArgs(score));
        }
    }
    
    public event EventHandler<EventArgs> loopCompleted = delegate {};
    public event EventHandler<EventArgs> placePiece = delegate {};
    public event EventHandler<EventArgs> placeBomb = delegate {};
    public event EventHandler<ScoreEventArgs> updateScore = delegate {};
    public event EventHandler<LifeEventArgs> updateLife = delegate {};
    public event EventHandler<TimeEventArgs> updateTimer = delegate {};
    public event EventHandler<ListOfBlockEventArgs> removeBlock = delegate {};
    public event EventHandler<ListOfBlockEventArgs> newPiece = delegate {};
    public event EventHandler<EventArgs> gameOver = delegate {};
    
    void Remove(List<BlockModel> blocks){
        foreach(BlockModel block in blocks)
            grid.Remove(block.x, block.z);
        removeBlock(this, new ListOfBlockEventArgs(blocks));
    }
    
    public void Add(List<BlockModel> blocks){
        foreach(BlockModel block in blocks)
            grid.Add(block.x, block.z, block);
    }
    
    public List<BlockModel> GetConnected(int x, int z){
        return CheckConnection(x,z).blocks;
    }
    
    int nbBlockToScoreIncrease(int nb){
        float c = 1.1f;
        return Mathf.RoundToInt((1f - Mathf.Pow(c,(float)nb))/(1f - c));
    }
    
    public void Bomb(int x, int z){
        CheckConnectionResult res = CheckConnection(x,z);
        Remove(res.blocks);
        newPiece(this, new ListOfBlockEventArgs(pieces.Get()));
        Time = 0.0f;
        placeBomb(this, new EventArgs());
    }
    
    public void Push(List<BlockModel> blocks){
        Add(blocks);
        foreach(BlockModel block in blocks){
            CheckConnectionResult res = CheckConnection(block.x,block.z);
            if(res.isLoop){
                Score += nbBlockToScoreIncrease(res.blocks.Count);
                Remove(res.blocks);
                loopCompleted(this, new EventArgs());
            }
        }
        newPiece(this, new ListOfBlockEventArgs(pieces.Get()));
        Time = 0.0f;
        placePiece(this, new EventArgs());
    }
    
    public void Update(float dt){
        if(Life == 0)
            return;
        float newTime = Time + dt/speed; // Acceleration: *Mathf.Pow(0.995f,totalTime));
        totalTime += dt;
        
        if(newTime > 1f){
            Life--;
            newPiece(this, new ListOfBlockEventArgs(pieces.Get()));
            Time = 0.0f;
        }else{
            Time = newTime;
        }
    }
    
    public void Start(){
        pieces.Load();
        newPiece(this, new ListOfBlockEventArgs(pieces.Get()));
        Time = 0.0f;
        speed = GameApplication.GetOptions().SpeedFlt;
    }
    
    BlockWithDir GetNext(BlockWithDir prev){
        HexDirection dir = prev.Dir;
        BlockModel block = prev.Block;
        int x = block.x;
        int z = block.z;
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
        BlockModel next = grid.Get(x, z);
        if(next == null)
            return null;
        HexDirection opp = dir.Opposite();
        if(next.dir1 == opp)
            return new BlockWithDir(next,next.dir2);
        if(next.dir2 == opp)
            return new BlockWithDir(next,next.dir1);
        return null;
    }
    
    CheckConnectionResult CheckConnection(int x, int z){
        CheckConnectionResult res = new CheckConnectionResult();
        List<BlockModel> blocks = res.blocks;
        BlockModel block = grid.Get(x, z);
        if(block == null)
            return res; // no block in the current position
        
        blocks.Add(block);
        BlockWithDir next = new BlockWithDir(block, block.dir2);
        while((next = GetNext(next)) != null){
            if(block == next.Block)
                break;
            blocks.Add(next.Block);
        }
        if(next != null){
            res.isLoop = true;
            return res; // a loop is available
        }
        next = new BlockWithDir(block, block.dir1);
        while((next = GetNext(next)) != null)
            blocks.Add(next.Block);
        return res; // no loop available
    }
}

public class BoardModelTest {
    void Loop(){
        int nbLoop = 0;
        int blockToRemove = 0;
        BoardModel board = new BoardModel();
        
        board.loopCompleted += (sender, ev) => { nbLoop++; };
        board.removeBlock += (sender, ev) => { blockToRemove += ev.Blocks.Count; };
        
        List<BlockModel> blocks = new List<BlockModel>();
        blocks.Add(new BlockModel(4,5,HexDirection.SW,HexDirection.E));
        blocks.Add(new BlockModel(5,5,HexDirection.W,HexDirection.SE));
        blocks.Add(new BlockModel(4,4,HexDirection.SE,HexDirection.NE));
        blocks.Add(new BlockModel(6,4,HexDirection.SW,HexDirection.NW));
        blocks.Add(new BlockModel(4,3,HexDirection.NW,HexDirection.E));
        board.Add(blocks);
        
        blocks = new List<BlockModel>();
        blocks.Add(new BlockModel(5,3,HexDirection.NE,HexDirection.W));
        board.Add(blocks);
        
        blocks.Add(new BlockModel(4,5,HexDirection.SW,HexDirection.E)); // should not crash here

        Debug.Assert(nbLoop == 1, "should detect one loop");
        Debug.Assert(blockToRemove == 6, "should delete 6 blocks");
    }
    public void Main(){
        Loop();
        Debug.Log("All green");
    }
}