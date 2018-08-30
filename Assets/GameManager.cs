using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public GameCamera gameCamera;
    public GameOver gameOver;
    public GamePause gamePause;
    public OutsideStep outside;
    public Board boardView;
    public InGamePanel ingamePanel;
    
    BoardModel boardModel = new BoardModel();
        
    void StartGame(){
        boardView.Playing = true;
    }
    
    private void Start () {
        GameApplication.LoadOptions();
        boardView.SetBoardModel(boardModel);
        
        int boardHeight = GameApplication.GetOptions().BoardHeight;
        int boardWidth = GameApplication.GetOptions().BoardWidth;
        
        boardModel.gameOver += (o, ev) => {
            gameOver.SetScore(boardModel.Score);
            gameOver.gameObject.SetActive(true); 
        };
        boardModel.updateScore += (o, ev) => { 
            ingamePanel.SetScore(ev.Score);
            boardView.Pause = ingamePanel.BlinkSpeed;
        };
        boardModel.updateLife  += (o, ev) => { 
            ingamePanel.SetLife(ev.Life);
            boardView.Pause = ingamePanel.BlinkSpeed;
        };
        boardModel.updateTimer += (o, ev) => {
            ingamePanel.SetTimer(ev.Time);
        };
        ingamePanel.SetScore(boardModel.Score, true);
        ingamePanel.SetLife(boardModel.Life, true);
        ingamePanel.SetTimer(boardModel.Time);
        gamePause.goingBack += (o,ev) => { boardView.Playing = true; };
        
        /*
        BoardModelTest test = new BoardModelTest();
        test.Main();
        return;
        */
        List<Pos> places = new List<Pos>();
        for(int x=0;x<boardWidth;x++)
            for(int z=0;z<boardHeight;z++)
                places.Add(new Pos(x,z));
        boardView.Places = places;
        
        /*
        BoardLoopFactory factory = new BoardLoopFactory(places);
        List<BlockModel> list = factory.GenerateLoop();
        boardView.CreateBlocks(list, true); // that's ugly as hell
        boardModel.Add(list);
        */
        outside.Triangulate(-5,boardWidth+5,-5,boardHeight+5);
        boardModel.Start();
        Invoke("StartGame", gameCamera.Duration);
    }
    
    void Update(){
        if(Input.GetButtonDown("Cancel") && boardView.Playing){
            boardView.Playing = false;
            gamePause.gameObject.SetActive(true);
        }
    }
}

/*
        List<BlockModel> blocks = new List<BlockModel>();
        blocks.Add(new BlockModel(4,5,HexDirection.SW,HexDirection.E));
        blocks.Add(new BlockModel(5,5,HexDirection.W,HexDirection.SE));
        blocks.Add(new BlockModel(4,4,HexDirection.SE,HexDirection.NE));
        blocks.Add(new BlockModel(6,4,HexDirection.SW,HexDirection.NW));
        blocks.Add(new BlockModel(4,3,HexDirection.NW,HexDirection.E));
        //blocks.Add(new BlockModel(5,3,HexDirection.NE,HexDirection.W));
        CreateBlocks(blocks, true);
        transforms.Get(4,5).GetComponent<BlockView>().Highlight(true);
*/
