using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WindowShortcut : MonoBehaviour, IPointerClickHandler
{
    public Window CurrentWindow { get; private set; }
    public ComputerUI CurrentComputerUI { get; set; }
    public Window WindowPrefab { get => windowPrefab; }
    public bool IsWrongApp { get => isWrongApp; }
    public Sprite Icon
    {
        get
        {
            if (overridedSprite != null)
                return overridedSprite;
            else if (windowPrefab != null && windowPrefab.icon != null)
                return windowPrefab.icon;
            return null;
        }
    }

    [Header("Важное")]
    [SerializeField] private Window windowPrefab;
    [SerializeField] private WindowShortcut virusShortcut;
    [SerializeField] private bool isWrongApp = false;

    [Header("Если окно этого ярлыка открыто в начале сцены")]
    [SerializeField] private Window startWindow;

    [Header("остальное")]
    [SerializeField] private Sprite overridedSprite;
    [SerializeField] private CanvasGroup selectionBackgroundGroup;
    [SerializeField] private Image icon;
    [SerializeField] private float fadeTransitionTime = 0.1f;
    [SerializeField] private float doubleClickInterval = 0.8f;

    private float lastClickTime = 0f;
    private void OnEnable()
    {
        if(selectionBackgroundGroup != null)
            selectionBackgroundGroup.alpha = 0f;

        icon.sprite = Icon;

        if (startWindow != null)
            CurrentWindow = startWindow;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
    public void OnClick()
    {
        if(CurrentComputerUI != null)
        {
            CurrentComputerUI.SetShortcutSelected(this);


            if(Time.time - lastClickTime < doubleClickInterval)
            {
                //double click here...

                if (virusShortcut != null)
                    Notifications.Notify("Access blocked");

                if (windowPrefab != null && virusShortcut == null)
                {
                    if (CurrentWindow == null)
                    {
                        CurrentWindow = CurrentComputerUI.SpawnWindow(windowPrefab);
                    }
                    else
                    {
                        CurrentWindow.Open();
                        CurrentComputerUI.InitializeWindows();
                    }

                    CurrentComputerUI.SetWindowHigher(CurrentWindow);
                    SetSelected(false);
                    lastClickTime = 0f;
                }
            }
            else
                lastClickTime = Time.time;
        }

    }
    public void SetSelected(bool activeSelf)
    {
        if (selectionBackgroundGroup == null)
            return;

        StopAllCoroutines();
        float targetAlpha = 0f;
        if (activeSelf)
            targetAlpha = 1f;

        if (targetAlpha != selectionBackgroundGroup.alpha)
            StartCoroutine(FadeTransitionEnumerator(selectionBackgroundGroup, targetAlpha, fadeTransitionTime));
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
    public void Initialize(ComputerUI computerUI)
    {
        CurrentComputerUI = computerUI;
    }
}
