using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void VoidEventHandler();
public delegate void BoolEventHandler(bool value);

//добавил структуру
[System.Serializable]
public struct WindowGameData
{
    public bool isWrongWindow;
}

[RequireComponent(typeof(RectTransform))]
public class Window : MonoBehaviour
{
    [Header("Здесь изменяем игровые параметры")]
    public WindowGameData gameData;

    [Header("Important")]
    public Sprite icon;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private Button hideButton;

    [Header("Other")]
    [SerializeField] private UI_ClickableObject[] draggingPanels;
    [SerializeField] private RectTransform bottomOfUpperPanel;
    [SerializeField] private float openTime = 0.4f;

    public ComputerUI currentComputerUI;
    public WindowIcon currentWindowIcon;

    private RectTransform rectTr;
    private Vector3 savedSize = Vector3.one;
    private Vector3 savedPosition = Vector3.zero;
    private IEnumerator currentTransitionRoutine;

    private bool _isAnimating = false;
    private bool isClosable = true;

    public BoolEventHandler OnWindowActiveStateChanged;
    public event VoidEventHandler OnInitialize;
    public bool isMainWindow { get; set; }
    public WindowIcon CurrentWindowIcon { get => currentWindowIcon; }
    public ComputerUI CurrentComputerUI { get => currentComputerUI; }
    public bool IsClosable { get => isClosable; set { isClosable = value; } }

    public bool IsClosed { get; private set; }


    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
        savedSize = rectTr.localScale;
        savedPosition = rectTr.position;
    }

    private void OnEnable()
    {
        if (rectTr == null)
            rectTr = GetComponent<RectTransform>();

        foreach (var draggingPanel in draggingPanels)
        {
            if (draggingPanel.isDraggable)
                draggingPanel.onDrag += OnPanelDragging;
            draggingPanel.onPointerDown += OnPanelPointerDown;
            draggingPanel.onPointerUp += OnPanelPointerUp;
        }

        if (hideButton != null)
            hideButton.onClick.AddListener(Hide);
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }
    private void OnDisable()
    {
        foreach (var draggingPanel in draggingPanels)
        {
            if (draggingPanel.isDraggable)
                draggingPanel.onDrag -= OnPanelDragging;
            draggingPanel.onPointerDown -= OnPanelPointerDown;
            draggingPanel.onPointerUp -= OnPanelPointerUp;
        }
        if (hideButton != null)
            hideButton.onClick.RemoveListener(Hide);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }
    private void Update()
    {
        //if (!rectTr.hasChanged)
        //    return;
        //if (currentComputerUI != null && upperPanel != null)
        //{
        //    Vector3 position = rectTr.localPosition;
        //    position.y = Mathf.Clamp(position.y, transform.parent.InverseTransformPoint(currentComputerUI.GetPeakOfLowerBar()).y - outline.sizeDelta.y / 2 + upperPanel.sizeDelta.y, (mainWindow.sizeDelta.y - outline.sizeDelta.y) / 2);
        //    rectTr.localPosition = position;
        //}
    }
    public void OnPanelDragging(PointerEventData eventData)
    {
        rectTr.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0f);

        if (CurrentComputerUI == null || bottomOfUpperPanel == null)
            return;

        Vector3 clampedPosition = rectTr.transform.position;
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0f, Screen.height);
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, 0f, Screen.width);
        rectTr.position = clampedPosition;
    }
    public void OnPanelPointerDown(PointerEventData eventData)
    {
        Cursor.lockState = CursorLockMode.Confined;
        GoHigher();
    }
    public void OnPanelPointerUp(PointerEventData eventData)
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void Open()
    {
        if (gameObject.activeSelf || _isAnimating)
            return;

        IsClosed = false;

        gameObject.SetActive(true);
        OnWindowActiveStateChanged?.Invoke(gameObject.activeSelf);

        if (currentTransitionRoutine != null)
            StopCoroutine(currentTransitionRoutine);
        currentTransitionRoutine = TransitionEnumerator(savedPosition, savedSize, openTime, true);
        StartCoroutine(currentTransitionRoutine);

        GoHigher();
    }
    public void Hide()
    {
        if (_isAnimating || !isClosable)
            return;

        if (currentWindowIcon == null)
        {
            gameObject.SetActive(false);
            return;
        }

        OnWindowActiveStateChanged?.Invoke(false);

        savedPosition = rectTr.position;
        savedSize = rectTr.localScale;

        if (currentTransitionRoutine != null)
            StopCoroutine(currentTransitionRoutine);
        currentTransitionRoutine = TransitionEnumerator(currentWindowIcon.transform.position,
            Vector3.zero, openTime, false);
        StartCoroutine(currentTransitionRoutine);
    }

    public void SpawnAnimation()
    {
        savedPosition = transform.position;
        savedSize = transform.localScale;
        transform.localScale = Vector3.zero;


        if (currentTransitionRoutine != null)
            StopCoroutine(currentTransitionRoutine);
        currentTransitionRoutine = TransitionEnumerator(savedPosition, savedSize, openTime, true);
        StartCoroutine(currentTransitionRoutine);
    }
    public void Close()
    {
        if (_isAnimating || !isClosable)
            return;

        IsClosed = true;

        savedPosition = transform.position;
        savedSize = transform.localScale;

        if (currentTransitionRoutine != null)
            StopCoroutine(currentTransitionRoutine);
        currentTransitionRoutine = TransitionEnumerator(savedPosition, Vector3.zero, openTime, false);
        StartCoroutine(currentTransitionRoutine);

        if (currentWindowIcon != null)
            Destroy(currentWindowIcon.gameObject);
    }

    public void GoHigher()
    {
        if (currentComputerUI != null)
        {
            currentComputerUI.SetWindowHigher(this);
        }
    }
    public void Initialize(ComputerUI currentComputer)
    {
        if (currentComputer == currentComputerUI)
            return;

        currentComputerUI = currentComputer;

        OnInitialize?.Invoke();
    }
    public void Initialize(ComputerUI currentComputer, WindowIcon icon)
    {
        currentWindowIcon = icon;
        Initialize(currentComputer);
    }

    private IEnumerator TransitionEnumerator(Vector3 targetPosition, Vector3 targetScale, float transitionTime, bool endActiveSelf)
    {
        if (transitionTime <= 0f || _isAnimating)
            yield break;

        _isAnimating = true;

        float t = 0f;
        float startTime = Time.time;
        while (t < 1f)
        {
            t = (Time.time - startTime) / transitionTime;
            rectTr.position = Vector3.Lerp(rectTr.position, targetPosition, t);
            rectTr.localScale = Vector3.Lerp(rectTr.localScale, targetScale, t);
            yield return null;
        }

        if (gameObject.activeSelf != endActiveSelf)
        {
            OnWindowActiveStateChanged?.Invoke(endActiveSelf);
        }

        gameObject.SetActive(endActiveSelf);
        _isAnimating = false;
    }
}
