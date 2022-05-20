using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
        
    }

    public void LoadNextScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneAmount = SceneManager.sceneCountInBuildSettings;
        if (sceneIndex + 1 == sceneAmount)
        {
            SceneManager.LoadScene(0);
            var scoreManager = GlobalScoreManager.Instance;
            if (scoreManager != null)
            {
                scoreManager.ResetScore();
            }
            return;
        }
        SceneManager.LoadScene(sceneIndex + 1);
    }

    public void ReloadScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
        Time.timeScale = 1;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        var scoreManager = GlobalScoreManager.Instance;
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        
        Application.Quit();
#endif
    }
}
