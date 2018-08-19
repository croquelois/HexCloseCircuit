using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameCamera gameCamera;
    public GameObject gameOver;
    public GameObject gamePause;
    public Board board;
    
    public void BackToMenu () {
        SceneManager.LoadScene("main");
    }
    
    void StartGame(){
        board.Playing = true;
    }
    
    private void Start () {
        GameApplication.LoadOptions();
        Invoke("StartGame", gameCamera.Duration);
    }
    
    void Update(){
        if(Input.GetButtonDown("Cancel")){
            board.Playing = !board.Playing;
            gamePause.SetActive(!board.Playing);
        }
    }
}
