using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ChatData
{
    public string name;
    public bool isWrongChat;
    public List<Message> messages;
    public Sprite icon;
}
public class Chat : MonoBehaviour
{
    public MessengerUI CurrentMessenger { get; private set; }
    public ChatMinitab CurrentChatMinitab { get; private set; }
    public ChatData CurrentChatData { get; set; }

    [SerializeField] private Transform messagesParent;
    [SerializeField] private UI_Message messagePrefab;

    private List<UI_Message> messages = new List<UI_Message>();

    public void Initialize(MessengerUI messenger, ChatMinitab chatMinitab, ChatData chatData)
    {
        CurrentMessenger = messenger;
        CurrentChatMinitab = chatMinitab;
        CurrentChatData = chatData;
        InstantiateMessages();
    }
    private void InstantiateMessages()
    {
        for (int i = 0; i < CurrentChatData.messages.Count; i++)
        {
            while(messages.Count < CurrentChatData.messages.Count)
            {
               messages.Add(Instantiate(messagePrefab, messagesParent));
            }

            messages[i].Initialize(CurrentChatData.messages[i]);
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Delete()
    {
        if (CurrentChatMinitab != null)
            Destroy(CurrentChatMinitab.gameObject);
        Destroy(gameObject);
    }
}
