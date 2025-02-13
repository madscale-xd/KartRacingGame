using UnityEngine;
using UnityEngine.SceneManagement;  // To load the menu scene
using UnityEngine.UI;              // For UI button functionality

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;  // Reference to the Pause Menu Canvas (assign in inspector)
    [SerializeField] private Button resumeButton;         // Reference to the Resume Button (assign in inspector)
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;           // Reference to the Menu Button (assign in inspector)

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _buttonClickSound; 
    private bool isPaused = false;

    void Start()
    {
        // Initially, set the pause menu canvas to inactive
        pauseMenuCanvas.SetActive(false);

        // Set up button listeners
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        menuButton.onClick.AddListener(LoadMenu);
    }

    void Update()
    {
        // Listen for Escape key to toggle pause state
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();  // If paused, resume the game
            }
            else
            {
                PlaySound(_buttonClickSound);
                PauseGame();   // If not paused, pause the game
            }
        }
    }

    void PauseGame()
    {
        // Check if a "Player" object exists before pausing
        if (GameObject.Find("Player") == null)
        {
            Debug.LogWarning("No object named 'Player' found. Cannot pause the game.");
            return;
        }

        // Activate the pause menu canvas and make it visible
        pauseMenuCanvas.SetActive(true);

        // Pause the game
        Time.timeScale = 0;
        isPaused = true;
    }

    void ResumeGame()
    {
        PlaySound(_buttonClickSound);
        pauseMenuCanvas.SetActive(false);

        // Resume the game
        Time.timeScale = 1;
        isPaused = false;
    }

    void RestartGame()
    {
        // Resume the game time before loading the scene
        Time.timeScale = 1;

        // Load the game scene
        SceneManager.LoadScene("KartRacer");  
    }

    void LoadMenu()
    {
        // Resume the game time before loading the scene
        Time.timeScale = 1;

        // Load the menu scene
        SceneManager.LoadScene("MainMenu");
    }

    private void PlaySound(AudioClip soundEffect)
    {
        if (_audioSource != null)
        {
            if (soundEffect != null)
            {
                _audioSource.PlayOneShot(soundEffect);
            }
            else if (_buttonClickSound != null)
            {
                _audioSource.PlayOneShot(_buttonClickSound);
            }
        }
    }
}
