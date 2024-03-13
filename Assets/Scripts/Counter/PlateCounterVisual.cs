using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private PlateCounter plateCounter;
    [SerializeField] private Transform plateVisual;
    [SerializeField] private Transform counterTopPoint;

    private List<Transform> plateVisualList = new();

    private void Start()
    {
        plateCounter = transform.root.GetComponent<PlateCounter>();
        plateCounter.OnPlateAdded += PlateCounter_OnPlateAdded;
        plateCounter.OnPlateRemoved += PlateCounter_OnPlateRemoved;
    }

    private void PlateCounter_OnPlateAdded(object sender, System.EventArgs e)
    {
        float plateOffset = 0.1f;
        Transform newPlateVisual = Instantiate(plateVisual, counterTopPoint);

        newPlateVisual.localPosition = new(0, plateOffset * plateVisualList.Count, 0);

        plateVisualList.Add(newPlateVisual);
    }

    private void PlateCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        GameObject plateGameObject = plateVisualList[plateVisualList.Count - 1].gameObject;

        plateVisualList.Remove(plateGameObject.transform);

        Destroy(plateGameObject);
    }

}
