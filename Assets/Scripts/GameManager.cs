using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameEvent startingEvent;
    
    public GameObject deathScreen;
    public GameObject pauseMenu;
    
    public bool isPaused = false;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        //deathScreen.SetActive(false);
        if(startingEvent != null) startingEvent.TriggerEvent();
    }

    void Start()
    {
        Debug.Log("Starting the Level");
        //Cursor.visible = false;
        
    }

    public void ShowDeathScreen()
    {
        Pause();
        //Add fade scene transition
        deathScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseMenu()
    {
        Pause();
        pauseMenu.SetActive(true);
        isPaused = true;
    }
    
    public void LoadNextScene()
    {
        int x = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(x + 1);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
