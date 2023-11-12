using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AntivirusAppIcon : MonoBehaviour, IPointerClickHandler
{
    public AntivirusUI CurrentAntivirus { get; private set; }
    public WindowShortcut CurrentShortcut { get; private set; }

    [SerializeField] private Image iconSlot;
    [SerializeField] private CanvasGroup selectionBackgroundGroup;
    [SerializeField] private float transitionTime = 0.1f;

    private void OnEnable()
    {
        if (selectionBackgroundGroup != null)
            selectionBackgroundGroup.alpha = 0f;
    }

    public void Initialize(WindowShortcut shortcut, AntivirusUI antivirus)
    {
        if (shortcut == null || shortcut.WindowPrefab == null)
            return;

        CurrentAntivirus = antivirus;
        CurrentShortcut = shortcut;
        iconSlot.sprite = shortcut.Icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentAntivirus == null || CurrentShortcut == null)
            return;

        CurrentAntivirus.SelectApp(this);
    }
    public void SetSelected(bool selectionState)
    {
        float targetAlpha = 0f;
        if (selectionState)
            targetAlpha = 1f;

        StopAllCoroutines();
        StartCoroutine(FadeTransitionEnumerator(selectionBackgroundGroup, targetAlpha, transitionTime));
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
}
