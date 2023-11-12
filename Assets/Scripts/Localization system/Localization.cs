using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization : MonoBehaviour
{
    public static Localization Instance
    {
        get
        {
            if(_instance == null)
            {
                Localization localizationPrefab = Resources.Load<Localization>("Systems/Localization");
                if (localizationPrefab == null)
                    return FindObjectOfType<Localization>();

                Localization spawnedLocalization = Instantiate(localizationPrefab);
                _instance = spawnedLocalization;
            }
            return _instance;
        }
    }
    private static Localization _instance;

    [SerializeField] LocalizationDictionary localizationDictionary;

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

    public static string Translate(string text)
    {
        if (Instance == null || Instance.localizationDictionary == null)
            return text;
        return Instance.localizationDictionary.Translate(text);
    }
}
