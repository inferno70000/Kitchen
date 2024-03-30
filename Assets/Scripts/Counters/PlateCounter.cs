using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateAdded;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private float spawnTimer = 0;
    private float spawnTimeMax = 4f;
    private int spawnCount = 0;
    private int spawnCountMax = 4;

    private void Update()
    {

        if (GameManager.Instance.IsGamePlaying())
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer > spawnTimeMax)
            {
                spawnTimer = 0;

                if (spawnCount < spawnCountMax)
                {
                    AddPlateServerRpc();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlateServerRpc()
    {
        AddPlateClientRpc();
    }

    [ClientRpc]
    private void AddPlateClientRpc()
    {
        spawnCount++;
        OnPlateAdded?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (spawnCount > 0f)
            {
                KitchenObject.Spawn(kitchenObjectSO, player);

                RemovePlateServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlateServerRpc()
    {
        RemovePlateClientRpc();
    }

    [ClientRpc]
    private void RemovePlateClientRpc()
    {
        spawnCount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
