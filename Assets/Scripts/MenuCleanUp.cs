using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (GameNetworkManager.Instance != null)
        {
            Destroy(GameNetworkManager.Instance.gameObject);
        }
    }
}
