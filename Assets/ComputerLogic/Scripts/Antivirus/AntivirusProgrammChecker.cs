using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class AntivirusProgrammChecker : MonoBehaviour
{
    public AntivirusUI CurrentAntivirus { get; private set; }
    public bool IsChecking { get => isChecking; }
    public AntivirusAppIcon CheckingApp { get; private set; }

    [SerializeField] private Image progressBar;
    [SerializeField] private Gradient progressGradient;
    [SerializeField] private GameObject succesfulDeletionPanel;
    [SerializeField] private GameObject failurDeletionPanel;

    private bool isChecking = false;
    private float startTime = 0f;
    private void OnEnable()
    {
        //if (!isChecking)
        //{
        //    CloseAllPanels();
        //}

        //if(isChecking)
        //    CurrentAntivirus.CurrentWindow.CurrentComputerUI.SetLockPanelActive(true);
        if (CurrentAntivirus != null && CurrentAntivirus.CurrentWindow != null)
            CurrentAntivirus.CurrentWindow.IsClosable = !isChecking;
    }
    private void OnDisable()
    {
        //if(CurrentAntivirus != null && CurrentAntivirus.CurrentWindow != null && CurrentAntivirus.CurrentWindow.CurrentComputerUI != null)
        //    CurrentAntivirus.CurrentWindow.CurrentComputerUI.SetLockPanelActive(false);
        if (CurrentAntivirus != null && CurrentAntivirus.CurrentWindow != null)
            CurrentAntivirus.CurrentWindow.IsClosable = true;
    }
    private void Update()
    {
        if (isChecking)
        {

            CloseAllPanels();

            float t = (Time.time - startTime) / CurrentAntivirus.gameData.appCheckingTime;
            progressBar.fillAmount = t;
            progressBar.color = progressGradient.Evaluate(t);

            if (t >= 1f)
            {
                ViewCheckResult();
            }
        }
    }
    public void StartChecking(AntivirusAppIcon app)
    {
        CheckingApp = app;
        gameObject.SetActive(true);

        isChecking = true;
        startTime = Time.time;

        if (CurrentAntivirus != null && CurrentAntivirus.CurrentWindow != null)
            CurrentAntivirus.CurrentWindow.IsClosable = false;

        //CurrentAntivirus.CurrentWindow.CurrentComputerUI.SetLockPanelActive(true);
    }
    public void StopDeletion()
    {
        CheckingApp = null;
        isChecking = false;

        if (CurrentAntivirus != null && CurrentAntivirus.CurrentWindow != null)
            CurrentAntivirus.CurrentWindow.IsClosable = true;
        //CurrentAntivirus.CurrentWindow.CurrentComputerUI.SetLockPanelActive(false);
    }
    public void Initialize(AntivirusUI antivirus)
    {
        CurrentAntivirus = antivirus;
    }
    private void ViewCheckResult()
    {
        CloseAllPanels();
        if (CheckingApp.CurrentShortcut.IsWrongApp)
        {
            succesfulDeletionPanel.SetActive(true);
        }
        else
        {
            failurDeletionPanel.SetActive(true);
        }
        //CurrentAntivirus.CurrentWindow.CurrentComputerUI.SetLockPanelActive(false);
        isChecking = false;
        if (CurrentAntivirus != null && CurrentAntivirus.CurrentWindow != null)
            CurrentAntivirus.CurrentWindow.IsClosable = true;
    }
    public void DeleteVirus()
    {
        CurrentAntivirus.DeleteApp(CheckingApp);
    }
    private void CloseAllPanels()
    {
        succesfulDeletionPanel.SetActive(false);
        failurDeletionPanel.SetActive(false);
    }
}
