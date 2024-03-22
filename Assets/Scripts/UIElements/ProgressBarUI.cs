using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject objectHasProgressBar;
    private IHasProgressBar iHasProgressBar;
    private Image bar;

    private void Awake()
    {
        iHasProgressBar = objectHasProgressBar.GetComponent<IHasProgressBar>();
        bar = transform.GetChild(1).GetComponent<Image>();
    }

    private void Start()
    {
        iHasProgressBar.OnProgressChanged += IHasProgressBar_OnProgressChangedEventAgr;

        Hide();
    }

    private void IHasProgressBar_OnProgressChangedEventAgr(object sender, IHasProgressBar.OnProgressChangedEventAgr e)
    {
        //Changing progress
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
