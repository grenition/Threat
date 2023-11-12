using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preferences : MonoBehaviour
{
    public static Preferences Instance
    {
        get
        {
            if (_instance == null)
            {
                Preferences preferencesPrefab = Resources.Load<Preferences>("Systems/Preferences");
                if (preferencesPrefab == null)
                    return FindObjectOfType<Preferences>();

                Preferences spawnedPreferences = Instantiate(preferencesPrefab);
                _instance = spawnedPreferences;
            }
            return _instance;
        }
    }
    private static Preferences _instance;

    public static event VoidEventHandler OnPreferencesChanged;

    [SerializeField] private PreferencesContainer currentPreferences;

    private Language savedLanguage;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (savedLanguage != CurrentLanguage)
        {
            print(CurrentLanguage);
            OnPreferencesChanged?.Invoke();
            savedLanguage = CurrentLanguage;
        }
    }

    #region Public values
    public static Language CurrentLanguage
    {
        get
        {
            if (Instance == null || Instance.currentPreferences == null)
                return Language.english;
            return Instance.currentPreferences.language;
        }
    }
    #endregion
}
