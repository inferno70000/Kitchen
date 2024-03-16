using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundListsSO soundListsSO;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("There is more than one SoundManager instance.");
        }

        Instance = this;
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

    private void PlayAudio(AudioClip[] clip, Vector3 position, float volume = 1f)
    {
        AudioClip audioClip = clip[Random.Range(0, clip.Length)];
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void PlayAudio(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlayFootStepSound(Player player)
    {
        PlayAudio(soundListsSO.footstep, player.transform.position);
    }
}
