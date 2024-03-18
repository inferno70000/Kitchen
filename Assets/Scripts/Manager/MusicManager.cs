using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance {  get; private set; }

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";


    private float volume = 0.3f;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one MusicManager instance.");
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        audioSource.volume = volume;
    }

    /// <summary>
    /// Change soundEffect's volume
    /// </summary>
    public void ChangeVolume()
    {
        volume += 0.1f;

        if (volume > 1.01f)
        {
            volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
        audioSource.volume = volume;
    }

    /// <summary>
    /// Get soundEffect's volume
    /// </summary>
    /// <returns></returns>
    public float GetVolume()
    {
        return volume;
    }
}
