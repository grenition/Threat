using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [HideInInspector] public static float _percentage;
    [HideInInspector] public static bool _isPaused;

    [Header("Important")]
    [SerializeField] private ServerController _server;

    [Header("Components of timer")]
    [SerializeField] private Image _fill;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Gradient gradient;

    [Header("Variables")]
    [SerializeField] private float _startPercentage;
    [SerializeField] private float _percentageDecrement;
    [SerializeField] private float _waitTime;

    public static void AddPercentage(float value)
    {
        _percentage = Mathf.Clamp(_percentage + value, 0, 100);
    }

    public static void SubPercentage(float value)
    {
        _percentage = Mathf.Clamp(_percentage - value, 0, 100);
    }

    public void StartWork()
    {
        _percentage = _startPercentage;
        StartCoroutine(nameof(UpdateTimer));
    }
    public static void Pause()
    {
        _isPaused = true;
    }
    public static void UnPause()
    {
        _isPaused = false;
    }
    private IEnumerator UpdateTimer()
    {
        while(_percentage < 100f && ServerController._isPlaying)
        {
            while(_isPaused)
            {
                yield return null;
            }

            _text.text = $"{Mathf.RoundToInt(_percentage)}%";
            _fill.fillAmount = _percentage / 100f;
            _fill.color = gradient.Evaluate(_percentage / 100f);
            AddPercentage(_percentageDecrement);
            yield return new WaitForSeconds(_waitTime);
        }

        _text.text = $"{Mathf.RoundToInt(_percentage)}%";
        _fill.fillAmount = _percentage / 100f;
        _fill.color = new Color(_percentage / 100f, 1 - _percentage / 100f, 0f);
        _server.TimesUp();
    }
}