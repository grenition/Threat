using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(BrowserWebsite))]
public class BrowserPreferencesSite : MonoBehaviour
{
    [SerializeField] private CookieDeletor cookieDeletor;
    [SerializeField] private TMP_Text cookieInformation;
    [SerializeField] private Button cookieDeletionButton;
    [SerializeField] private TMP_Text trackersProtectionInformation;
    [SerializeField] private Button trackersProtectionButton;
    [SerializeField] private TMP_Text trackersProtectionButtonText;
    [SerializeField] private Color trackerProtectionButton_OffColor;
    [SerializeField] private Color trackerProtectionButton_OnColor;

    private BrowserWebsite currentWebsite;
    private BrowserUI currentBroswerUI;
    private void OnEnable()
    {
        if (currentWebsite == null)
            currentWebsite = GetComponent<BrowserWebsite>();
        currentWebsite.OnInitialization += Initialization;

        trackersProtectionButton.onClick.AddListener(OnTrackerProtectionButtonPressed);
        cookieDeletionButton.onClick.AddListener(OnCookieDeletionButtonPressed);
        cookieDeletor.OnCookieDataChanged += UpdateGameData;
    }

    private void OnDisable()
    {
        currentWebsite.OnInitialization -= Initialization;

        trackersProtectionButton.onClick.RemoveListener(OnTrackerProtectionButtonPressed);
        cookieDeletionButton.onClick.RemoveListener(OnCookieDeletionButtonPressed);
        cookieDeletor.OnCookieDataChanged -= UpdateGameData;
    }
    private void Initialization()
    {
        currentBroswerUI = currentWebsite.CurrentBrowser;

        if (currentBroswerUI == null)
            return;

        cookieDeletor.Initialize(currentBroswerUI);
        UpdateGameData();
    }
    private void OnCookieDeletionButtonPressed()
    {
        cookieDeletor.StartDeletion();
    }
    public void OnTrackerProtectionButtonPressed()
    {
        currentBroswerUI.gameData.trackerProtection = !currentBroswerUI.gameData.trackerProtection;
        UpdateGameData();
    }
    public void UpdateGameData()
    {
        string cookie = "Detected cookies";
        if (!currentBroswerUI.gameData.haveCookies)
            cookie = "Cookies not detected";
        cookieInformation.ChangeText(cookie);

        if (currentBroswerUI.gameData.trackerProtection)
        {
            trackersProtectionInformation.ChangeText("Tracker protection enabled");
            trackersProtectionInformation.color = trackerProtectionButton_OnColor;
            if (trackersProtectionButton.TryGetComponent(out Image image))
            {
                image.color = trackerProtectionButton_OffColor;
            }
            trackersProtectionButtonText.ChangeText("Turn off tracker protection");
        }
        else
        {
            trackersProtectionInformation.ChangeText("Tracker protection disabled");
            trackersProtectionInformation.color = trackerProtectionButton_OffColor;
            if (trackersProtectionButton.TryGetComponent(out Image image))
            {
                image.color = trackerProtectionButton_OnColor;
            }
            trackersProtectionButtonText.ChangeText("Enable tracker protection");
        }
    }
}
