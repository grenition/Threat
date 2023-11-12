using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public struct BrowserGameData
{
    public bool haveCookies;
    public bool trackerProtection;
}

[System.Serializable]
public struct BrowserLevels
{
    public float succesfulAnswer_TimerBonus;
    public float wrongAnswer_TimerFine;
}
[RequireComponent(typeof(Window))]
public class BrowserUI : MonoBehaviour, IPointerClickHandler
{
    public ComputerUI CurrentComputer { get => CurrentWindow.CurrentComputerUI; }
    public Window CurrentWindow { get; private set; }

    [Header("Здесь изменяем игровые параметры")]
    public BrowserGameData gameData;
    public BrowserLevels levels;

    [Header("Main")]
    [SerializeField] private GameObject wrongSiteClosedNotification;

    [Header("Optional")]
    [SerializeField] private Transform minitabsParent;
    [SerializeField] private BrowserMinitab minitabPrefab;
    [SerializeField] private WindowClosingConfirmation windowClosingConfirmation;
    [SerializeField] private bool openFirstPageOnAwake = true;

    private List<BrowserWebsite> websites = new List<BrowserWebsite>();
    private List<BrowserMinitab> minitabs = new List<BrowserMinitab>();

    private BrowserWebsite lastOpenedWebsite;
    private bool isFirstTimeStart = true;
    private void OnEnable()
    {
        if (CurrentWindow == null)
            CurrentWindow = GetComponent<Window>();
        CurrentWindow.OnInitialize += Initialization;
    }
    private void OnDisable()
    {

    }
    private void Initialization()
    {
        websites = ListTools.ClearNulls(websites);

        foreach (var j in transform.GetComponentsInChildren<BrowserWebsite>(true))
        {
            if (!websites.Contains(j))
                websites.Add(j);
        }
        InitializeWebsites();

        if (windowClosingConfirmation != null)
            windowClosingConfirmation.Initialize(this);
        if (wrongSiteClosedNotification != null)
            wrongSiteClosedNotification.SetActive(false);


        if (isFirstTimeStart && openFirstPageOnAwake)
        {
            if (websites.Count != 0)
                SelectWebsite(websites[0]);
            isFirstTimeStart = false;
        }
    }
    private void InitializeWebsites()
    {
        foreach (var web in websites)
        {
            BrowserMinitab minitab = null;

            foreach (var tab in minitabs)
            {
                if (tab.CurrentWebsite == web)
                {
                    minitab = tab;
                    break;
                }
            }

            if (minitab == null)
            {
                minitab = InitializeMinitab(web);
                minitabs.Add(minitab);
            }

            web.Initialize(this, minitab);

            if(web != lastOpenedWebsite)
                web.Close();
        }
    }
    private BrowserMinitab InitializeMinitab(BrowserWebsite website)
    {
        BrowserMinitab minitab = Instantiate(minitabPrefab, minitabsParent);
        minitab.Initialize(this, website);
        return minitab;
    }

    #region Public functions
    public void SelectWebsite(BrowserWebsite website)
    {
        foreach (var web in websites)
        {
            if (web == website)
            {
                web.Open();
                lastOpenedWebsite = website;
            }
            else
                web.Close();
        }
    }
    public void CloseWebsite(BrowserWebsite website)
    {
        if (!website.isWrongWebsite)
        {
            wrongSiteClosedNotification.SetActive(true);
            TimerController.AddPercentage(levels.wrongAnswer_TimerFine);
            AudioController.PlayWrongSound();
        }
        else
        {
            TimerController.SubPercentage(levels.succesfulAnswer_TimerBonus);
            AudioController.PlayRightSound();
            CurrentComputer.ConfirmTask();
        }

        if (websites.Contains(website))
            websites.Remove(website);
        if (minitabs.Contains(website.CurrentMinitab))
            minitabs.Remove(website.CurrentMinitab);
        Destroy(website.CurrentMinitab.gameObject);
        Destroy(website.gameObject);

    }
    public void ShowWindowClosingConfirmation(BrowserMinitab minitab)
    {
        if (windowClosingConfirmation == null || minitab == null)
            return;
        windowClosingConfirmation.Open(minitab);
    }
  
    public void OnPointerClick(PointerEventData eventData)
    {
        if(wrongSiteClosedNotification.activeSelf == true)
        {
            if(TryGetComponent(out UI_ResizingAnimationOnAwake anim))
            {
                anim.CloseWithAnimation();
            }
        }
    }
    #endregion
}
