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

    [SerializeField] private AudioSource loopSourceSlow; // For slow looping sounds
    [SerializeField] private AudioSource loopSourceHum;  // For hum sound loop
    [SerializeField] private SoundEffect[] soundEffects; // Array of sound effects

    private string currentlyLoopingSlow = null;
    private string currentlyLoopingHum = null;

    private Dictionary<string, AudioSource> oneShotSources = new Dictionary<string, AudioSource>(); // Tracks active one-shot sounds

    [Range(0f, 1f)] public float oneShotVolume = 1.0f; // Volume control for one-shot sounds
    private SFXManager sfxman;

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

    // Play a sound once, ensuring the latest sound overrides the previous one
    public void PlaySound(string soundName)
    {
        foreach (SoundEffect sound in soundEffects)
        {
            if (sound.name == soundName)
            {
                // If sound is already playing, stop it before playing the new one
                if (oneShotSources.ContainsKey(soundName))
                {
                    oneShotSources[soundName].Stop();
                }
                else
                {
                    // Create a new AudioSource for this sound if it doesn't exist
                    AudioSource newSource = gameObject.AddComponent<AudioSource>();
                    newSource.playOnAwake = false;
                    newSource.spatialBlend = 0f; // Non-3D sound by default
                    oneShotSources[soundName] = newSource;
                }

                // Assign the new clip and play it
                oneShotSources[soundName].clip = sound.clip;
                oneShotSources[soundName].volume = oneShotVolume; // Apply volume
                oneShotSources[soundName].Play();

                return;
            }
        }
        Debug.LogWarning("Sound not found: " + soundName);
    }

    // Adjust volume for one-shot sounds
    public void SetOneShotVolume(float value)
    {
        oneShotVolume = Mathf.Clamp(value, 0f, 1f); // Clamp volume between 0 and 1

        // Update volume for all existing one-shot AudioSources
        foreach (var source in oneShotSources.Values)
        {
            source.volume = oneShotVolume;
        }
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
