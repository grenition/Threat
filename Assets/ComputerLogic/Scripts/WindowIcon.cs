using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WindowIcon : MonoBehaviour
{
    public Window CurrentWindow { get; private set; }

    [SerializeField] private Button button;
    [SerializeField] private Image iconSlot;

    [Header("Animations")]
    [SerializeField] private CanvasGroup selectionBackgroundGroup;
    [SerializeField] private float fadeTransitionTime = 0.4f;

    private IEnumerator currentTransitionRoutine;
    private bool _isAnimating = false;

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(OpenWindow);

        if (selectionBackgroundGroup != null)
            selectionBackgroundGroup.alpha = 0f;
    }
    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(OpenWindow);
        if (CurrentWindow != null)
            CurrentWindow.OnWindowActiveStateChanged -= ChangeSelectionBackgroundVisibillity;
    }
    public void Initialize(Window window)
    {
        if (window == null)
            return;
        CurrentWindow = window;
        if (CurrentWindow != null)
            CurrentWindow.OnWindowActiveStateChanged += ChangeSelectionBackgroundVisibillity;

        if(iconSlot != null && window.icon != null)
        {
            iconSlot.sprite = window.icon;
        }
    }
    public void OpenWindow()
    {
        if (CurrentWindow == null)
            return;

        if (!CurrentWindow.gameObject.activeSelf)
            CurrentWindow.Open();
        else
        {
            if(CurrentWindow.isMainWindow)
                CurrentWindow.Hide();
            else
                CurrentWindow.GoHigher();
        }
    }

    public void ChangeSelectionBackgroundVisibillity(bool value)
    {
        if (CurrentWindow == null || selectionBackgroundGroup == null)
            return;

        float targetAlpha = 0f;
        if (value && CurrentWindow.isMainWindow)
            targetAlpha = 1f;

        StartCoroutine(FadeTransitionEnumerator(selectionBackgroundGroup, targetAlpha, fadeTransitionTime));
    }

    private IEnumerator FadeTransitionEnumerator(CanvasGroup group, float targetAlpha, float transitionTime)
    {
        if (transitionTime <= 0f || _isAnimating)
            yield break;

        _isAnimating = true;

        float t = 0f;
        float startTime = Time.time;
        float startAlpha = group.alpha;
        while (t < 1f)
        {
            t = (Time.time - startTime) / transitionTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        _isAnimating = false;
    }
}
