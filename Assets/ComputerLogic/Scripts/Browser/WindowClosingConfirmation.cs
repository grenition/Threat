using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class WindowClosingConfirmation : MonoBehaviour
{
    public BrowserUI CurrentBrowser { get; private set; }
    public BrowserWebsite CurrentWebsite { get; private set; }

    [SerializeField] private float transitionTime = 0.2f;
    [SerializeField] private Transform mainPanel;

    private CanvasGroup group;
    private Vector3 startSize = Vector3.one;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        startSize = mainPanel.localScale;
        group.alpha = 0f;
    }
    public void Open(BrowserMinitab minitab)
    {
        if (mainPanel == null)
            return;

        gameObject.SetActive(true);

        mainPanel.localScale = Vector3.zero;
        if (minitab != null)
            transform.position = minitab.transform.position;

        CurrentWebsite = minitab.CurrentWebsite;

        StopAllCoroutines();
        StartCoroutine(TransitionEnumerator(1f, startSize, transitionTime));
    }
    public void CloseWebsite()
    {
        if (CurrentWebsite != null && CurrentBrowser != null)
            CurrentBrowser.CloseWebsite(CurrentWebsite);

        StopAllCoroutines();
        StartCoroutine(TransitionEnumerator(0f, Vector3.zero, transitionTime));
    }
    public void ClosePanel()
    {
        CurrentWebsite = null;

        AudioController.PlayButtonSound();
        StopAllCoroutines();
        StartCoroutine(TransitionEnumerator(0f, Vector3.zero, transitionTime));
    }
    private IEnumerator TransitionEnumerator(float targetAlpha, Vector3 targetSize, float transitionTime)
    {
        if (transitionTime <= 0f || mainPanel == null)
            yield break;

        float t = 0f;
        float startTime = Time.time;
        float startAlpha = group.alpha;
        while (t < 1f)
        {
            t = (Time.time - startTime) / transitionTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            mainPanel.localScale = Vector3.Lerp(mainPanel.localScale, targetSize, t);
            yield return null;
        }

        if (group.alpha == 0f)
            gameObject.SetActive(false);
    }
    public void Initialize(BrowserUI browser)
    {
        CurrentBrowser = browser;
    }
}
