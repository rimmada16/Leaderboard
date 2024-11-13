using Leaderboard.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the game state.
/// </summary>
public class DemoGameManager : MonoBehaviour
{
    [SerializeField] private KeyCode resetKey = KeyCode.R;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    public bool inLeaderboard;
    public bool inputFieldActive;
    public bool gameIsPaused;
    public static DemoGameManager Instance;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Canvas inputFieldCanvas;
    [SerializeField] private Canvas pauseMenuCanvas;
    
    /// <summary>
    /// Sets the instance of the GameManager.
    /// </summary>
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    /// <summary>
    /// Lock the cursor on start
    /// </summary>
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }
        
        CursorHandler.Instance.SetCursorState(true);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Handle game events
    /// </summary>
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }

        if (Input.GetKeyDown(pauseKey))
        {
            gameIsPaused = !gameIsPaused;

            if (gameIsPaused)
            {
                pauseMenuCanvas.gameObject.SetActive(true);
                PauseDemoGame();
            }
            else if (!gameIsPaused)
            {
                pauseMenuCanvas.gameObject.SetActive(false);
                ResumeDemoGame();
            }
        }

        if (Input.GetKeyDown(resetKey) && !nameInputField.IsActive())
        {
            ResetLevel();
        }
        
        if (inputFieldCanvas.isActiveAndEnabled)
        {
            inputFieldActive = true;
            CursorHandler.Instance.SetCursorState(false);
        }
        else
        {
            inputFieldActive = false;
        }
        
        if (LeaderboardSaveData.Instance.leaderboardIsActive != inLeaderboard)
        {
            inLeaderboard = LeaderboardSaveData.Instance.leaderboardIsActive;
        }
        
        
    }

    /// <summary>
    /// Resets the level
    /// </summary>
    public void ResetLevel()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Loads the main menu
    /// </summary>
    public void LoadDemoMainMenu()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
    
    /// <summary>
    /// Loads the first demo level
    /// </summary>
    public void LoadDemoLevelOne()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }
    
    /// <summary>
    /// Loads the second demo level
    /// </summary>
    public void LoadDemoLevelTwo()
    {
        SceneManager.LoadScene(sceneBuildIndex: 2);
    }
    
    /// <summary>
    /// Loads the third demo level
    /// </summary>
    public void LoadDemoLevelThree()
    {
        SceneManager.LoadScene(sceneBuildIndex: 3);
    }
    
    /// <summary>
    /// Quits the demo game
    /// </summary>
    public void QuitDemoGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /// <summary>
    /// Pauses the demo game
    /// </summary>
    private void PauseDemoGame()
    {
        CursorHandler.Instance.SetCursorState(false);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
    
    /// <summary>
    /// Resumes the demo game
    /// </summary>
    public void ResumeDemoGame()
    {
        pauseMenuCanvas.gameObject.SetActive(false);
        CursorHandler.Instance.SetCursorState(true);
        gameIsPaused = false;
        Time.timeScale = 1f;
    }
}
