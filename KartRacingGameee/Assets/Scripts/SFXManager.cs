using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private AudioSource sfxSource;      // For one-shot sounds
    [SerializeField] private AudioSource loopSourceSlow; // For slow looping sounds
    [SerializeField] private AudioSource loopSourceHum;  // For hum sound loop
    [SerializeField] private SoundEffect[] soundEffects; // Array of sound effects

    private string currentlyLoopingSlow = null;
    private string currentlyLoopingHum = null;

    private HashSet<string> activeOneShots = new HashSet<string>(); // Tracks active one-shot sounds

    private void Awake()
    {
        // Singleton pattern to ensure only one instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play a sound once, ensuring no duplicate play
    public void PlaySound(string soundName)
    {
        if (activeOneShots.Contains(soundName)) return; // Prevent duplicate play

        foreach (SoundEffect sound in soundEffects)
        {
            if (sound.name == soundName)
            {
                sfxSource.PlayOneShot(sound.clip);
                activeOneShots.Add(soundName);
                StartCoroutine(RemoveFromActiveOneShots(sound.clip.length, soundName)); // Remove after duration
                return;
            }
        }
        Debug.LogWarning("Sound not found: " + soundName);
    }

    // Coroutine to remove the sound from active list after it finishes playing
    private System.Collections.IEnumerator RemoveFromActiveOneShots(float duration, string soundName)
    {
        yield return new WaitForSeconds(duration);
        activeOneShots.Remove(soundName);
    }

    // Start looping a sound on the slow loop source
    public void StartLoopingSlow(string soundName)
    {
        if (currentlyLoopingSlow == soundName) return; // Avoid restarting the same sound

        foreach (SoundEffect sound in soundEffects)
        {
            if (sound.name == soundName)
            {
                loopSourceSlow.clip = sound.clip;
                loopSourceSlow.loop = true;
                loopSourceSlow.Play();
                currentlyLoopingSlow = soundName;
                return;
            }
        }
        Debug.LogWarning("Loop sound not found: " + soundName);
    }

    // Start looping a sound on the hum loop source
    public void StartLoopingHum(string soundName)
    {
        if (currentlyLoopingHum == soundName) return; // Avoid restarting the same sound

        foreach (SoundEffect sound in soundEffects)
        {
            if (sound.name == soundName)
            {
                loopSourceHum.clip = sound.clip;
                loopSourceHum.loop = true;
                loopSourceHum.Play();
                currentlyLoopingHum = soundName;
                return;
            }
        }
        Debug.LogWarning("Loop sound not found: " + soundName);
    }

    // Stop looping a specific slow sound
    public void StopLoopingSlow(string soundName)
    {
        if (currentlyLoopingSlow == soundName && loopSourceSlow.isPlaying)
        {
            loopSourceSlow.Stop();
            currentlyLoopingSlow = null;
        }
        else
        {
            Debug.LogWarning("Tried to stop a looping sound that isn't playing: " + soundName);
        }
    }

    // Stop looping a specific hum sound
    public void StopLoopingHum(string soundName)
    {
        if (currentlyLoopingHum == soundName && loopSourceHum.isPlaying)
        {
            loopSourceHum.Stop();
            currentlyLoopingHum = null;
        }
        else
        {
            Debug.LogWarning("Tried to stop a looping sound that isn't playing: " + soundName);
        }
    }
}
