using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameCamera gameCamera;
    public Board board;
    
    public void BackToMenu () {
        SceneManager.LoadScene("main");
    }
    
    void Unpause(){
        board.Unpause();
    }
    
    private void Start () {
        Invoke("Unpause", gameCamera.Duration);
    }
}
