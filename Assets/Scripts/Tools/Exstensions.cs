using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class Exstensions
{
    public static void ChangeText(this TMP_Text tmp_text, string text)
    {
        tmp_text.text = text;
        if(tmp_text.TryGetComponent(out LocalizableText locText))
        {
            locText.Localize();
        }
    }
}
