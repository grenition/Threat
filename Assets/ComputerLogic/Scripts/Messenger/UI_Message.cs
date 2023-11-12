using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct TextOptions
{
    public bool italic;
}
[System.Serializable]
public struct Message 
{
    [TextArea] public string text;
    public TextOptions textOptions;

    [Space]
    public Sprite image;
    public bool isMine;
    
}
public enum MessageSize
{
    medium,
    small, 
    large
}

[RequireComponent(typeof(RectTransform))]
public class UI_Message : MonoBehaviour
{
    public Message CurrentMessage 
    { 
        get => curMessage;
        set
        {
            curMessage = value;
            UpdateContent();
        }
    }
    [SerializeField] private Message curMessage;

    //parameters
    [Header("Base")]
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private RectTransform background;
    [SerializeField] private float maxWidth = 280f;
    [SerializeField] private Image mainImage;
    [SerializeField] private Vector2 imageSize = new Vector2(280f, 280f);
    [SerializeField] private float spacing = 10f;

    [Header("Visual")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color senderBackgroundColor;
    [SerializeField] private Color receiverBackgroundColor;

    //local
    private RectTransform rectTr;
    private RectTransform mainTextRectTr;
    private RectTransform mainImageRectTr;
    private VerticalLayoutGroup layoutGroup;
    private Vector2 screenResolutionDelta = Vector2.one;

    //local functions
    private void OnEnable()
    {
        Initialize(CurrentMessage);
        Preferences.OnPreferencesChanged += UpdateTrasfrormsOnNextFrame;
    }
    private void OnDisable()
    {
        Preferences.OnPreferencesChanged -= UpdateTrasfrormsOnNextFrame;
    }
    private void UpdateContent()
    {
        mainText.richText = true;
        if (CurrentMessage.textOptions.italic)
            mainText.ChangeText("<i>" + CurrentMessage.text + "</i>");
        else
            mainText.ChangeText(CurrentMessage.text);
        mainImage.sprite = CurrentMessage.image;

        UpdateTransforms();
    }
    private void UpdateTransforms()
    {
        mainImage.gameObject.SetActive(CurrentMessage.image != null);

        Vector2 newSizeDelta = new Vector2(Mathf.Clamp(mainText.preferredWidth, 0f, maxWidth), mainText.preferredHeight);
        if(CurrentMessage.image != null)
        {
            mainImageRectTr.sizeDelta = imageSize * screenResolutionDelta;
            newSizeDelta.y += spacing + imageSize.y;
            newSizeDelta.x = imageSize.x;
        }
        rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, newSizeDelta.y) * screenResolutionDelta;

        Vector3 position = Vector3.zero;
        if (!CurrentMessage.isMine)
            position.x -= rectTr.sizeDelta.x / 2f - newSizeDelta.x / 2f;
        else
            position.x += rectTr.sizeDelta.x / 2f - newSizeDelta.x / 2f;

        Vector3 textPosition = position;
        Vector3 imagePosition = position;
        if(CurrentMessage.image != null)
        {
            imagePosition.y += rectTr.sizeDelta.y / 2f - mainImageRectTr.sizeDelta.y / 2f;
            mainImageRectTr.localPosition = imagePosition * screenResolutionDelta;

            textPosition.y -= rectTr.sizeDelta.y / 2f - mainText.preferredHeight / 2f;
        }
        mainTextRectTr.localPosition = textPosition * screenResolutionDelta;

        if(background != null)
        {
            background.sizeDelta = newSizeDelta * screenResolutionDelta;
            background.localPosition = position * screenResolutionDelta;

            Vector3 scale = background.localScale;
            if (!CurrentMessage.isMine)
                scale.x = 1f;
            else
                scale.x = -1f;
            background.localScale = scale;
            UpdateBackgroundColor();
        }

        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
    }
    private void UpdateBackgroundColor()
    {
        if (backgroundImage == null)
            return;

        if (!CurrentMessage.isMine)
            backgroundImage.color = senderBackgroundColor;
        else
            backgroundImage.color = receiverBackgroundColor;
    }

    private void UpdateTrasfrormsOnNextFrame()
    {
        StartCoroutine(UpdateTransformsEnumerator());
    }
    private IEnumerator UpdateTransformsEnumerator()
    {
        yield return null;
        UpdateTransforms();
    }
    //public functions
    public void Initialize(Message _message)
    {
        if (mainText == null || mainImage == null)
        {
            enabled = false;
            return;
        }

        rectTr = GetComponent<RectTransform>();
        mainTextRectTr = mainText.GetComponent<RectTransform>();
        mainImageRectTr = mainImage.GetComponent<RectTransform>();
        layoutGroup = GetComponentInParent<VerticalLayoutGroup>();

        CurrentMessage = _message;
    }
}
