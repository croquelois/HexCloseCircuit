using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameCamera gameCamera;
    public GameOver gameOver;
    public GamePause gamePause;
    public Board boardView;
    public InGamePanel ingamePanel;
    BoardModel boardModel = new BoardModel();
        
    void StartGame(){
        boardView.Playing = true;
    }
    
    private void Start () {
        GameApplication.LoadOptions();
        boardView.SetBoardModel(boardModel);
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
