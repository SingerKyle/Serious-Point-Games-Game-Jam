using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager instance;

    // Audio Source for managing background music and sound effects
    public AudioSource musicSource;
    public AudioSource sfxSource;
    //[SerializeField] private List<AudioClip> BackgroundAmbience;

    // Other global systems can be added here, for example:
    public bool isPaused = false;

    // Settings (like volume levels)
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Singleton pattern: Ensure only one GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This will make the GameManager persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate GameManagers
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize any necessary systems or settings
        UpdateVolume();
    }

    // Method to play background music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    // Method to play sound effects
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Method to update the volume of music and SFX
    public void UpdateVolume()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freezes the game time
    }

    // Unpause the game
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resumes the game time
    }

    // Toggle game pause
    public void TogglePause()
    {
        if (isPaused)
            UnpauseGame();
        else
            PauseGame();
    }

    // Method for saving game data
    public void SaveGame()
    {
        // Save game logic (e.g., PlayerPrefs, file-based save, etc.)
        Debug.Log("Game Saved!");
    }

    // Method for loading game data
    public void LoadGame()
    {
        // Load game logic
        Debug.Log("Game Loaded!");
    }

    // Additional global game systems can be added here, like:
    // - Global object pooling
    // - Level progression management
    // - Player stats or inventory
    // - Difficulty adjustments
}

