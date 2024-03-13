using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    private CuttingCounter cuttingCounter;
    private Image bar;

    private void Start()
    {
        cuttingCounter = transform.root.GetComponent<CuttingCounter>();
        bar = transform.GetChild(1).GetComponent<Image>();
        cuttingCounter.OnCuttingProgress += CuttingCounter_OnCuttingProgress;

        Hide();
    }

    private void CuttingCounter_OnCuttingProgress(object sender, CuttingCounter.OnCuttingProgressEventAgr e)
    {
        bar.fillAmount = e.progressNomalized;

        if (e.progressNomalized == 0 || e.progressNomalized == 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
