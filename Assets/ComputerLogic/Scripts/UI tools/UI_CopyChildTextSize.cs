using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class UI_CopyChildTextSize : MonoBehaviour
{
    [SerializeField] private TMP_Text childText;

    private RectTransform tr;
    private RectTransform childRectTr;

    private string savedText;

    private void OnEnable()
    {
        tr = GetComponent<RectTransform>();
        if (childText == null) {
            enabled = false;
            return; 
        }
        childRectTr = childText.GetComponent<RectTransform>();
    }
    private void Update()
    {
        if(savedText != childText.text)
            tr.sizeDelta = new Vector2(childText.preferredWidth, tr.sizeDelta.y);
        savedText = childText.text;
    }
}
