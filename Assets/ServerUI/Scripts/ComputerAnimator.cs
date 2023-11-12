using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(ComputerUI))]
public class ComputerAnimator : MonoBehaviour
{
    public static bool _isSomeWindowAnimating = false;
    public static bool _isSomeWindowOpened = false;

    [Header("Some Values")]
    [SerializeField] private float _openTime = 1f;

    [Header("Some objects")]
    [SerializeField] private GameObject _closePanel;
    [SerializeField] private GameObject _overlayPanel;
    [SerializeField] private Transform _mainWindow;
    [SerializeField] private ComputerUI[] _computers;

    private Vector3 _startPosition;
    private Vector3 _startScale;
    private Vector3 _endPosition = Vector3.zero;
    private Vector3 _endScale = Vector3.one;

    private RectTransform _rectTransform;
    private ComputerUI _computerUI;
    private IEnumerator _currentRoutine;

    private bool _isOpened = false;

    private void Awake()
    {
        _isSomeWindowAnimating = false;
        _isSomeWindowOpened = false;

        _rectTransform = GetComponent<RectTransform>();
        _startScale = _rectTransform.localScale;
        _startPosition = _rectTransform.localPosition;

        _computerUI = GetComponent<ComputerUI>();

        if (!_isOpened)
        {
            _overlayPanel.SetActive(true);
        }
    }

    public void OpenWindow()
    {
        if (_isSomeWindowAnimating || _computerUI.IsCleared())
            return;

        if (!_isSomeWindowOpened)
        {
            AudioController.PlayOpenSound();
            _isOpened = true;
            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);
            _currentRoutine = Transition(_endPosition, _endScale, _openTime, true);
            StartCoroutine(_currentRoutine);
        }
    }

    public void CloseWindow()
    {
        if (_isSomeWindowAnimating)
            return;

        if (_isSomeWindowOpened)
        {
            AudioController.PlayCloseSound();
            _isOpened = false;
            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);
            _currentRoutine = Transition(_startPosition, _startScale, _openTime, false);
            StartCoroutine(_currentRoutine);
        }
    }

    public bool IsOpened()
    {
        return _isOpened;
    }

    private IEnumerator Transition(Vector3 targetPosition, Vector3 targetScale, float transitionTime, bool isOpening)
    {
        if (transitionTime <= 0f || _isSomeWindowAnimating)
            yield break;

        _isSomeWindowAnimating = true;

        if (!isOpening)
        {
            _overlayPanel.SetActive(true);
            foreach (ComputerUI computer in _computers)
                if (computer != GetComponent<ComputerUI>())
                {
                    Vector3 pos = new Vector3(computer.gameObject.GetComponent<RectTransform>().localPosition.x, computer.gameObject.GetComponent<RectTransform>().localPosition.y, 0f);
                    computer.gameObject.GetComponent<RectTransform>().localPosition = pos;
                }
        }

        float t = 0f;
        float startTime = Time.time;
        Vector3 savedPosition = _rectTransform.localPosition;
        Vector3 savedScale = _rectTransform.localScale;

        while (t < 1f)
        {
            t = (Time.time - startTime) / transitionTime;
            _rectTransform.localPosition = Vector3.Lerp(savedPosition, targetPosition, t);
            _rectTransform.localScale = Vector3.Lerp(savedScale, targetScale, t);
            yield return null;
        }

        if (isOpening)
        {
            _overlayPanel.SetActive(false);
            foreach (ComputerUI computer in _computers)
                if (computer != GetComponent<ComputerUI>())
                {
                    RectTransform _compTr = computer.gameObject.GetComponent<RectTransform>();
                    Vector3 pos = new Vector3(_compTr.localPosition.x, _compTr.localPosition.y, 10f);
                    _compTr.localPosition = pos;
                }
        }



        _closePanel.SetActive(!isOpening);
        _isSomeWindowOpened = isOpening;
        _isSomeWindowAnimating = false;
    }
}
