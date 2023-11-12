using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrowserWebsite : MonoBehaviour
{
    public BrowserMinitab CurrentMinitab { get; private set; }
    public BrowserUI CurrentBrowser { get; private set; }

    public Sprite websiteIcon;
    public string websiteLabel;
    public bool isWrongWebsite;

    public bool closable = true;

    public event VoidEventHandler OnInitialization;

    public void Initialize(BrowserUI browser, BrowserMinitab minitab)
    {
        CurrentBrowser = browser;
        CurrentMinitab = minitab;

        OnInitialization?.Invoke();
    }
    public void Open()
    {
        gameObject.SetActive(true);
        if (CurrentMinitab != null)
            CurrentMinitab.SetSelected(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
        if (CurrentMinitab != null)
            CurrentMinitab.SetSelected(false);
    }
}
