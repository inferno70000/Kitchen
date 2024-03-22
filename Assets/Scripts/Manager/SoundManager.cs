using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const string PLAYER_PREFS_SOUND_EFFECT_VOLUME = "SoundEffectVolume";

    [SerializeField] private SoundListsSO soundListsSO;
    private float volume = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one SoundManager instance.");
        }

        if (Instance != null)
        {
            Debug.Log("There is more than one SoundManager instance.");
        }

        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, volume);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
        DeliveryManager.Instance.OnDeliveryFail += DeliveryManager_OnDeliveryFail;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        BaseCounter.OnGetKitchenObject += BaseCounter_OnGetKitchenObject;
        Player.Instance.OnPickupSomething += Player_OnPickupSomething;
        TrashCounter.OnAnyObjectTrahed += TrashCounter_OnAnyObjectTrahed;
    }

    private void TrashCounter_OnAnyObjectTrahed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = (TrashCounter)sender;
        PlayAudio(soundListsSO.trash, trashCounter.transform.position);
    }

    private void Player_OnPickupSomething(object sender, System.EventArgs e)
    {
        PlayAudio(soundListsSO.object_pickup, Player.Instance.transform.position);
    }

    private void BaseCounter_OnGetKitchenObject(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = (BaseCounter)sender;

        PlayAudio(soundListsSO.object_drop, baseCounter.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlayAudio(soundListsSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnDeliveryFail(object sender, System.EventArgs e)
    {
        PlayAudio(soundListsSO.delivery_fail, DeliveryManager.Instance.transform.position);
    }

    private void DeliveryManager_OnDeliverySuccess(object sender, System.EventArgs e)
    {
        PlayAudio(soundListsSO.delivery_success, DeliveryManager.Instance.transform.position);
    }

    private void PlayAudio(AudioClip[] clip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioClip audioClip = clip[Random.Range(0, clip.Length)];
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

    private void PlayAudio(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volumeMultiplier * volume);
    }

    /// <summary>
    /// Play footstep sound at playter position
    /// </summary>
    /// <param name="player">Player's instance</param>
    public void PlayFootStepSound(Player player)
    {
        PlayAudio(soundListsSO.footstep, player.transform.position);
    }

    /// <summary>
    /// Play countdown sound 
    /// </summary>
    public void PlayCountdownSound()
    {
        PlayAudio(soundListsSO.warning, Vector3.zero);
    }

    /// <summary>
    /// Play countdown sound 
    /// </summary>
    public void PlayWarningSound(Vector3 position)
    {
        PlayAudio(soundListsSO.warning, position);
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

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, volume);
        PlayerPrefs.Save();
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
