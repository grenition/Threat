using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BrowserMinitab : MonoBehaviour, IPointerClickHandler
{
    public BrowserWebsite CurrentWebsite { get; private set; }
    public BrowserUI CurrentBrowser { get; private set; }

    [SerializeField] private CanvasGroup selectionBackgroundGroup;
    [SerializeField] private Image iconSlot;
    [SerializeField] private TMP_Text label;
    [SerializeField] private float selectionFadeTime = 0.1f;
    [SerializeField] private Button closeButton;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(CloseWebsite);
    }
    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(CloseWebsite);
    }
    public void Initialize(BrowserUI browser, BrowserWebsite website)
    {
        if (label == null)
            return;

        CurrentBrowser = browser;
        CurrentWebsite = website;

        if(iconSlot != null)
            iconSlot.sprite = website.websiteIcon;

        label.ChangeText(website.websiteLabel);

        if (!website.closable)
            closeButton.gameObject.SetActive(false);
    }

    public void SetSelected(bool selectionState)
    {
        if (selectionBackgroundGroup == null)
            return;

        float targetAlpha = 0f;
        if (selectionState)
            targetAlpha = 1f;

        StopAllCoroutines();
        StartCoroutine(FadeTransitionEnumerator(selectionBackgroundGroup, targetAlpha, selectionFadeTime));
    }

    private IEnumerator FadeTransitionEnumerator(CanvasGroup group, float targetAlpha, float transitionTime)
    {
        if (transitionTime <= 0f)
            yield break;

        float t = 0f;
        float startTime = Time.time;
        float startAlpha = group.alpha;
        while (t < 1f)
        {
            t = (Time.time - startTime) / transitionTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentBrowser == null || CurrentWebsite == null)
            return;

        CurrentBrowser.SelectWebsite(CurrentWebsite);
        AudioController.PlayButtonSound();
    }

    public void CloseWebsite()
    {
        if (CurrentBrowser == null || CurrentWebsite == null)
            return;

        CurrentBrowser.ShowWindowClosingConfirmation(this);
        AudioController.PlayButtonSound();
    }
}
