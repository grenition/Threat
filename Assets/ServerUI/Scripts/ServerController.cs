using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerController : MonoBehaviour
{
    [HideInInspector] public static bool _isPlaying = false;
    [HideInInspector] public static int _countOfClearedComputers = 0;

    [Header("Elements of system")]
    [SerializeField] private Transform _mainWindow;
    [SerializeField] private TimerController _timer;
    [SerializeField] private TimeController _time;
    [SerializeField] private ComputerUI[] _computers;

    [Header("UI Elements")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _winPanel;

    [Header("Time Settings")]
    [Header("Дату ставим так, чтобы где-нибудь в середине месяца. Не хочу отрабатывать смену месяца")]
    [SerializeField] private string _startYear;
    [SerializeField] private string _startMonth;
    [SerializeField] private string _startDay;
    [Header("Время в секундах")]
    [Range(0f, 86400f)][SerializeField] private int _startTime;
    [Header("Скорость часов")]
    [SerializeField] private float _timeSpeed;

    private bool _isWin = false;

    private void Start()
    {
        //foreach (var obj in _computers)
        //    obj.gameObject.SetActive(false);

        Camera.main.orthographicSize *= Screen.width / 1920f;
        Vector2 scale = new Vector2(_mainWindow.localScale.x, _mainWindow.localScale.y) * Screen.width / 1920f;
        _mainWindow.localScale = new Vector3(scale.x, scale.y, 1f);

        StartGame();
    }
    public void OpenScene(int index)
    {
        if (index == 0 && MusicController.instance != null) 
            MusicController.instance.ChangeToMenuMusic();
        SceneManager.LoadScene(index);
        AudioController.Silent();
    }
    public void StartGame()
    {
        _isPlaying = true;
        _countOfClearedComputers = 0;
        _timer.StartWork();
        _time.StartWork(_startYear, _startMonth, _startDay, _startTime, _timeSpeed);
        foreach(ComputerUI computer in _computers)
        {
            computer.gameObject.SetActive(true);
            computer.Initialization();
        }
    }
    private void Update()
    {
        if(_countOfClearedComputers == _computers.Length && _isPlaying)
        {
            _isPlaying = false;
            Win();
        }

    }
    public void Restart()
    {
        OpenScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Win()
    {
        _isWin = true;
        _winPanel.SetActive(true);
        AudioController.PlayWinSound();
    }
    public void GameOver()
    {
        _isWin = false;
        _gameOverPanel.SetActive(true);
        AudioController.PlayLoseSound();
    }
    public void TimesUp()
    {
        _isPlaying = false;

        foreach (ComputerUI computer in _computers) 
        {
            if(computer.GetComponent<ComputerAnimator>().IsOpened())
                computer.gameObject.GetComponent<ComputerAnimator>().CloseWindow();
        }

        if (_isWin)
            Win();
        else
            GameOver();
    }
    #region Static_methods
    public static void ClearComputer()
    {
        _countOfClearedComputers++;
        AudioController.PlayCleanSound();
    }
    public static void PlayButtonSound()
    {
        AudioController.PlayButtonSound();
    }
    #endregion
}
