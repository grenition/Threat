using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Translates
{
    [TextArea] public string englishTranslation;
    [TextArea] public string russianTranslation;

    public string GetTranslation(Language language)
    {
        switch (language)
        {
            case Language.russian:
                return russianTranslation;
            default:
                return englishTranslation;
        }
    }
    public bool IsContainsTranslation(string translation)
    {
        return translation == englishTranslation || translation == russianTranslation;
    }
} 
[CreateAssetMenu(fileName = "LocalizationDictionary", menuName = "GameResources/LocalizationDictionary")]
public class LocalizationDictionary : ScriptableObject
{
    [SerializeField] private Translates[] translates;

    public string Translate(string text)
    {
        foreach (var j in translates)
        {
            if (j.IsContainsTranslation(text))
            {
                return j.GetTranslation(Preferences.CurrentLanguage);
            } 
        }
        return text;
    }
}
