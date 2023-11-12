using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Variables")]
    [SerializeField] private int _countOfTasks;

    [Header("Transforms")]
    [SerializeField] private RectTransform peakOfLowerBar;
    [SerializeField] private GameObject parentOfWindows;
    [SerializeField] private GameObject parentOfWindowIcons;
    [SerializeField] private RectTransform higherSiblingForWindows;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject blockComputerPanel;

    [Header("Prefabs")]
    [SerializeField] private WindowIcon windowIconPrefab;

    [SerializeField] private List<Window> currentWindows = new List<Window>();
    [SerializeField] private List<WindowIcon> currentWindowIcons = new List<WindowIcon>();
    [SerializeField] private List<WindowShortcut> currentWindowsShortcuts = new List<WindowShortcut>();

    public List<WindowShortcut> CurrentWindowShortcuts { get => currentWindowsShortcuts; }
    public List<Window> CurrentWindows { get => currentWindows; }

    private bool _isCleared = false;
    private int _confirmedTasks = 0;
    public void Initialization()
    {
        foreach (var win in GetComponentsInChildren<Window>())
            currentWindows.Add(win);
        foreach (var shortcut in GetComponentsInChildren<WindowShortcut>())
        {
            shortcut.Initialize(this);
            currentWindowsShortcuts.Add(shortcut);
        }

        InitializeWindows();

        //SetLockPanelActive(false);
    }
    public List<Window> GetCurrentWindows()
    {
        return currentWindows;
    }
    public void InitializeWindows()
    {

        List<WindowIcon> icons = new List<WindowIcon>(currentWindowIcons);

        foreach (var j in currentWindowIcons)
        {
            if (j == null)
                icons.Remove(j);
        }
        currentWindowIcons = icons;

        foreach (var win in currentWindows)
        {
            //создаю иконку
            bool isFindIcon = false;
            foreach (var _icon in currentWindowIcons)
            {
                if (_icon.CurrentWindow == win)
                {
                    isFindIcon = true;
                    break;
                }
            }
            WindowIcon icon = null;
            if (!isFindIcon && !win.IsClosed)
            {
                icon = InitializeIcon(win);
            }

            //инциализация окна
            if (icon != null)
                win.Initialize(this, icon);
            else
                win.Initialize(this);
        }
    }
    private WindowIcon InitializeIcon(Window window)
    {
        if (windowIconPrefab == null || parentOfWindowIcons == null)
        {
            Debug.LogWarning("components is not assigned");
            return null;
        }

        WindowIcon icon = Instantiate(windowIconPrefab, parentOfWindowIcons.transform);
        icon.Initialize(window);
        currentWindowIcons.Add(icon);
        return icon;
    }

    #region Public func
    public void SetWindowHigher(Window window)
    {
        if (higherSiblingForWindows == null || window == null)
            return;

        window.transform.SetAsLastSibling();

        window.isMainWindow = true;
        foreach (var win in currentWindows)
        {
            if (win != window)
                win.isMainWindow = false;

            if (win.CurrentWindowIcon != null)
                win.CurrentWindowIcon.ChangeSelectionBackgroundVisibillity(window.isMainWindow == true);
        }
    }
    public Vector3 GetPeakOfLowerBar()
    {
        if (peakOfLowerBar != null)
            return peakOfLowerBar.position;

        return new Vector3();
    }
    public void SetShortcutSelected(WindowShortcut shortcut)
    {
        foreach (var _short in currentWindowsShortcuts)
        {
            _short.SetSelected(_short == shortcut);
        }
    }
    public Window SpawnWindow(Window prefab)
    {
        Window window = Instantiate(prefab, parentOfWindows.transform);
        currentWindows.Add(window);

        InitializeWindows();
        window.SpawnAnimation();
        SetWindowHigher(window);

        return window;
    }
    public void CloseWindow(Window window)
    {
        window.Close();
    }

    public void Clear()
    {
        _isCleared = true;
        ServerController.ClearComputer();
        GetComponent<ComputerAnimator>().CloseWindow();
        clearPanel.SetActive(true);
    }

    public void PlayButtonSound()
    {
        AudioController.PlayButtonSound();
    }
    public void ConfirmTask()
    {
        _confirmedTasks++;
        if (_confirmedTasks == _countOfTasks)
            Clear();
    }

    public bool IsCleared()
    {
        return _isCleared;
    }
    public void DeleteShortcut(WindowShortcut shortcut)
    {
        if (!currentWindowsShortcuts.Contains(shortcut))
            currentWindowsShortcuts.Remove(shortcut);
        if(shortcut.CurrentWindow != null && shortcut.CurrentWindow.gameObject.activeSelf)
            CloseWindow(shortcut.CurrentWindow);
        Destroy(shortcut.gameObject);
    }
    public void SetLockPanelActive(bool activeState)
    {
        if (blockComputerPanel == null)
            return;

        blockComputerPanel.SetActive(activeState);
    }
    #endregion
    public void OnPointerClick(PointerEventData eventData)
    {
        SetShortcutSelected(null);
    }
}

