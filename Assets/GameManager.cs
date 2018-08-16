using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public void BackToMenu () {
        SceneManager.LoadScene("main");
    }
}
