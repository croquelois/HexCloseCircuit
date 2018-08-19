using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameCamera gameCamera;
    public Board board;
    
    public void BackToMenu () {
        SceneManager.LoadScene("main");
    }
    
    void StartGame(){
        board.StartGame();
    }
    
    private void Start () {
        GameApplication.LoadOptions();
        Invoke("StartGame", gameCamera.Duration);
    }
}
