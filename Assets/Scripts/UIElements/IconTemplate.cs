using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTemplate : MonoBehaviour
{
    private Image icon;

    private void Awake()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
    }

    /// <summary>
    /// Set sprite to this IconTemlate's Image
    /// </summary>
    /// <param name="sprite">Spite want to set</param>
    public void ChangeIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}
