using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public Transform camera;
    public Transform board;
    
    public void BackToMenu () {
        SceneManager.LoadScene("main");
    }
    
    private void Start () {
        float x = ((float)Options.boardWidth-1f)*HexMetrics.innerRadius;
        float dy = 23f + 12.5f * Options.boardHeight;
        float dz = 0.67f*dy;
        float z = ((float)Options.boardHeight-1f)*0.5f*(HexMetrics.innerRadius+HexMetrics.outerRadius)-dz;
        camera.localPosition = new Vector3(x,dy,z);
    }
}
