using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ChatMinitab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MessengerUI CurrentMessenger { get; private set; }
    public Chat CurrentChat { get; private set; }

    [SerializeField] private TMP_Text label;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite defaultIcon;

    [SerializeField] private Image background;
    [SerializeField] private Color selectionColor;
    [SerializeField] private Color previewColor;
    [SerializeField] private float colorTransitionTime = 0.1f;
    private Color defaultColor;
    private bool selected = false;

    private void Awake()
    {
        if(background != null)
        {
            defaultColor = background.color;
        }
    }
    public void Initialize(MessengerUI messenger, Chat currentChat)
    {
        CurrentMessenger = messenger;
        CurrentChat = currentChat;

        label.ChangeText(CurrentChat.CurrentChatData.name);
        if (CurrentChat.CurrentChatData.icon != null) 
        {
            icon.sprite = CurrentChat.CurrentChatData.icon;
        }
        else
        {
            icon.sprite = defaultIcon;
        }
    }


    public void SetSelcted(bool selectionState)
    {
        selected = selectionState;
        StopAllCoroutines();
        if (selected)
        {
            StartCoroutine(ColorTransition(selectionColor));
        }
        else
        {
            StartCoroutine(ColorTransition(defaultColor));
        }
    }

    #region animations
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selected)
            return;

        StopAllCoroutines();
        StartCoroutine(ColorTransition(previewColor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selected)
            return;

        StopAllCoroutines();
        StartCoroutine(ColorTransition(defaultColor));
    }
    private IEnumerator ColorTransition(Color targetColor)
    {
        if (background == null)
            yield break;

        float startTime = Time.time;
        float t = 0f;
        Color startColor = background.color;
        while (t < 1)
        {
            t = (Time.time - startTime) / colorTransitionTime;
            background.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        #endregion
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentMessenger == null)
            return;

        CurrentMessenger.SelectChat(CurrentChat);
    }
}
