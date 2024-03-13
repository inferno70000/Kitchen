using System;
using System.Collections;
using System.Collections.Generic;
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
        spawnTimer += Time.deltaTime;

        if (spawnTimer > spawnTimeMax)
        {
            spawnTimer = 0;

            if (spawnCount < spawnCountMax)
            {
                OnPlateAdded?.Invoke(this, EventArgs.Empty);

                spawnCount++;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (spawnCount > 0f)
            {
                KitchenObject.Spawn(kitchenObjectSO, player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);

                spawnCount--;
            }
        }
    }
}
