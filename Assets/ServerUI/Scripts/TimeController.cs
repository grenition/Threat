using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public static TimeController mainInstance { get; private set; }
    public static bool _isPaused { get; private set; }

    [Header("Elements")]
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _timeText;
    [Header("Только для главных часов")]
    [SerializeField] private bool _isMainClocks = false;

    private int time;
    private string year;
    private string month;
    private string day;
    private float timeSpeed;

    private IEnumerator _curCorutine;
    public void StartWork(string curYear, string curMonth, string curDay, int curTime, float curTimeSpeed)
    {
        time = curTime;
        year = curYear;
        month = curMonth;
        day = curDay;
        timeSpeed = curTimeSpeed;

        StartCor();
    }

    private void Awake()
    {
        if (_isMainClocks && mainInstance == null)
            mainInstance = this;
    }
    private void OnEnable()
    {
        if(_isMainClocks)
            StartCor();

        Initialization();
    }
    private void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        if (mainInstance != null)
        {
            StartWork(mainInstance.year, mainInstance.month, mainInstance.day, mainInstance.time, mainInstance.timeSpeed);
        }
    }

    public static void Pause()
    {
        _isPaused = true;
    }
    public static void UnPause()
    {
        _isPaused = false;
    }

    private void StartCor()
    {
        if (_curCorutine != null)
            StopCoroutine(_curCorutine);
        _curCorutine = Counting();
        StartCoroutine(_curCorutine);
    }
    private IEnumerator Counting()
    {
        while (ServerController._isPlaying)
        {
            while (_isPaused)
            {
                yield return null;
            }

            _timeText.ChangeText(string.Format("{0}.{1}.{2} {3:d2}:{4:d2}", day, month, year, time / 3600, (time / 60) % 60));
            yield return new WaitForSeconds(timeSpeed);
            time++;
        }
    }
}
