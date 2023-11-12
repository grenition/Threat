using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AntivirusGameData
{
    public float appCheckingTime;
}

[RequireComponent(typeof(Window))]
public class AntivirusUI : MonoBehaviour
{
    public Window CurrentWindow { get; private set; }

    [Header("Здесь изменяем игровые параметры")]
    public AntivirusGameData gameData;
    public int correctCheck_TimerBonus = 10;

    [Header("Далее локальные настройки")]
    [SerializeField] private AntivirusAppIcon appIconPrefab;
    [SerializeField] private Transform appIconsParent;
    [SerializeField] private Button checkButton;
    [SerializeField] private AntivirusProgrammChecker programmChecker;

    private List<AntivirusAppIcon> appIcons = new List<AntivirusAppIcon>();

    private AntivirusAppIcon currentAppIcon;
    private ComputerUI computer;

    private void Start()
    {
        programmChecker.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if(CurrentWindow == null)
            CurrentWindow = GetComponent<Window>();
        CurrentWindow.OnInitialize += Initialization;
        checkButton.onClick.AddListener(CheckCurrentApp);
        DeselectAllApps(true);
    }
    private void OnDisable()
    {
        checkButton.onClick.RemoveListener(CheckCurrentApp);
    }
    private void Initialization()
    {
        SpawnAppIcons();
        programmChecker.Initialize(this);
    }

    private void SpawnAppIcons()
    {
        if (CurrentWindow == null || appIconPrefab == null)
            return;

        foreach (var shortcut in CurrentWindow.CurrentComputerUI.CurrentWindowShortcuts)
        {
            bool continueFlag = false;
            foreach(var j in appIcons)
            {
                if (j.CurrentShortcut == shortcut)
                {
                    continueFlag = true;
                    break;
                }
            }
            if (continueFlag)
                continue;

            AntivirusAppIcon appIcon = Instantiate(appIconPrefab, appIconsParent);
            appIcon.Initialize(shortcut, this);
            appIcons.Add(appIcon);
        }
        
    }
    private void CheckCurrentApp()
    {
        if (currentAppIcon == null)
            return;
        programmChecker.StartChecking(currentAppIcon);
    }
    public void DeleteApp(AntivirusAppIcon appIcon)
    {
        TimerController.SubPercentage(correctCheck_TimerBonus);
        AudioController.PlayRightSound();
        CurrentWindow.CurrentComputerUI.ConfirmTask();

        CurrentWindow.CurrentComputerUI.DeleteShortcut(appIcon.CurrentShortcut);
        appIcons.Remove(appIcon);
        Destroy(appIcon.gameObject);
    }
    public void SelectApp(AntivirusAppIcon appIcon)
    {
        DeselectAllApps();
        currentAppIcon = appIcon;
        appIcon.SetSelected(true);
        checkButton.gameObject.SetActive(true);
    }
    private void DeselectAllApps(bool disableButton = false)
    {
        currentAppIcon = null;
        foreach(var app in appIcons)
        {
            app.SetSelected(false);
        }
        if(disableButton)
            checkButton.gameObject.SetActive(false);
    }
    
}
