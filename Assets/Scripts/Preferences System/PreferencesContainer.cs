using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    english,
    russian
}

[CreateAssetMenu(fileName = "PreferencesPreset", menuName = "GameResources /PreferencesPreset")]
public class PreferencesContainer : ScriptableObject
{
    public Language language;
}
