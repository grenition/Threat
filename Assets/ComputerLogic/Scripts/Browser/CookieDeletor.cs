using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CookieDeletor : MonoBehaviour
{
    public bool IsDeleting { get => isDeleting; }
    public BrowserUI CurrentBrowser { get; private set; }
    [SerializeField] private float cookieDeletionTime = 20f;
    [SerializeField] private Image progressBar;
    [SerializeField] private Gradient progressGradient;
    [SerializeField] private GameObject succesfulDeletionPanel;
    [SerializeField] private GameObject failurDeletionPanel;
    [SerializeField] private float timeToTransition = 0.2f;

    private bool isDeleting = false;
    private float startTime = 0f;
    private CanvasGroup group;
    private Vector3 savedScale = Vector3.one;

    public event VoidEventHandler OnCookieDataChanged;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        savedScale = transform.localScale;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        StartScenario();
    }
    private void Update()
    {
        if (isDeleting)
        {
            float t = (Time.time - startTime) / cookieDeletionTime;
            progressBar.fillAmount = t;
            progressBar.color = progressGradient.Evaluate(t);

            if(t >= 1f)
            {
                DeleteCookies();
            }
        }
    }
    private void StartScenario()
    {
        if (group == null)
            return;
        if (!isDeleting)
        {
            succesfulDeletionPanel.SetActive(false);
            failurDeletionPanel.SetActive(false);
        }

        transform.localScale = Vector3.zero;
        group.alpha = 0f;
        StopAllCoroutines();
        StartCoroutine(TransitionEnumerator(1f, savedScale, timeToTransition));
    }
    public void StartDeletion()
    {
        gameObject.SetActive(true);

        isDeleting = true;
        startTime = Time.time;
    }
    private void StopDeletion()
    {
        isDeleting = false;
    }
    private void DeleteCookies()
    {
        isDeleting = false;


        if (CurrentBrowser == null)
            return;
        if (CurrentBrowser.gameData.haveCookies)
        {
            succesfulDeletionPanel.SetActive(true);
            CurrentBrowser.gameData.haveCookies = false;
            OnCookieDataChanged?.Invoke();
            AudioController.PlayRightSound();
            TimerController.SubPercentage(CurrentBrowser.levels.succesfulAnswer_TimerBonus);
            CurrentBrowser.CurrentComputer.ConfirmTask();
        }
        else
        {
            failurDeletionPanel.SetActive(true);
            AudioController.PlayWrongSound();
            TimerController.AddPercentage(CurrentBrowser.levels.wrongAnswer_TimerFine);
        }
    }
    public void Close()
    {
        StopDeletion();
        StopAllCoroutines();
        StartCoroutine(TransitionEnumerator(0f, Vector3.zero, timeToTransition));
    }
    public void Initialize(BrowserUI browser)
    {
        CurrentBrowser = browser;
    }
    private IEnumerator TransitionEnumerator(float targetAlpha, Vector3 targetSize, float transitionTime)
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
            transform.localScale = Vector3.Lerp(transform.localScale, targetSize, t);
            yield return null;
        }

        if (group.alpha == 0f)
            gameObject.SetActive(false);
    }
}
