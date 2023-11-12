using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizableText : MonoBehaviour
{
    private TMP_Text textObject;
    private void OnEnable()
    {
        textObject = GetComponent<TMP_Text>();
        Localize();

        Preferences.OnPreferencesChanged += Localize;
    }
    private void OnDisable()
    {
        Preferences.OnPreferencesChanged -= Localize;
    }
    public void Localize()
    {
        if (textObject == null)
            return;
        textObject.text = Localization.Translate(textObject.text);
    }
}
