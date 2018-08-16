using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {

	public void StartGame () 
	{
        SceneManager.LoadScene("game", LoadSceneMode.Single);
	}
    
	public void Options () 
	{
	}
    
	public void BackToMenu () 
	{
        SceneManager.LoadScene("main", LoadSceneMode.Single);
	}
    
	public void Quit () 
	{
		#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
		#else
            Application.Quit();
		#endif
	}
}
