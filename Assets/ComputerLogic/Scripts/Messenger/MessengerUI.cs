using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class ListTools
{
    public static List<T> ClearNulls<T>(List<T> list)
    {
        List<T> newList = new List<T>(list);
        foreach(var j in newList)
        {
            if (j == null)
                newList.Remove(j);
        }
        return newList;
    }
}

[System.Serializable]
public struct MessengerGameData
{
    public string[] wrongChats;
}
[System.Serializable]
public struct MessengerLevels 
{
    public int succesfulAnswer_TimerBonus;
    public int wrongAnswer_TimerFine;
}

[RequireComponent(typeof(Window))]
public class MessengerUI : MonoBehaviour
{
    [Header("Здесь изменяем игровые параметры")]
    public MessengerGameData gameData;
    public MessengerLevels levels;
    public List<ChatData> chatDatas = new List<ChatData>();

    [Header("Notifications")]
    [SerializeField] private GameObject succesfulDeleteNotificationBase;
    [SerializeField] private TMP_Text succesfulDeleteNotificationBonusText;

    [SerializeField] private GameObject failureDeleteNotificationBase;
    [SerializeField] private TMP_Text failureDeleteNotificationBonusText;

    [Header("Other")]
    [SerializeField] private Chat chatPrefab;
    [SerializeField] private Transform chatParent;
    [SerializeField] private GameObject[] activeWithChatObjects;
    [SerializeField] private GameObject[] deactivateWhileChatChanges;
    [SerializeField] private GameObject[] deactivateOnEnable;
    [SerializeField] private Transform minitabsParent;
    [SerializeField] private ChatMinitab minitabPrefab;

    public List<Chat> chats = new List<Chat>();
    private List<ChatMinitab> minitabs = new List<ChatMinitab>();
    private Chat lastOpenedChat;

    private ComputerUI CurrentComputer
    {
        get
        {
            if (comp != null)
                return null;
            else if (TryGetComponent(out Window win))
                return win.CurrentComputerUI;
            else
                return null;
        }
    }
    private ComputerUI comp;

    private void OnEnable()
    {
        foreach (var obj in deactivateOnEnable)
            obj.gameObject.SetActive(false);


        InitializeChats();
        DisableNotifications();
    }
    private void InitializeChats()
    {
        RefreshLists();

        for (int i = 0; i < chatDatas.Count; i++)
        {
            while (chats.Count < chatDatas.Count)
            {
                chats.Add(Instantiate(chatPrefab, chatParent));
            }

            Chat chat = chats[i];

            ChatMinitab minitab = null;
            foreach (var tab in minitabs)
            {
                if (tab.CurrentChat == chat)
                {
                    minitab = tab;
                    break;
                }
            }

            chat.CurrentChatData = chatDatas[i];

            if (minitab == null)
            {
                minitab = InitializeMinitab(chat);
                minitabs.Add(minitab);
            }

            chat.Initialize(this, minitab, chatDatas[i]);

            if (chat != lastOpenedChat)
                chat.Close();
        }

        RefreshChatButtons();
    }
    private ChatMinitab InitializeMinitab(Chat chat)
    {
        ChatMinitab minitab = Instantiate(minitabPrefab, minitabsParent);
        minitab.Initialize(this, chat);
        return minitab;
    }
    private void RefreshChatButtons()
    {
        bool isAnyChatOpened = false;
        foreach(var chat in chats)
        {
            if (chat.gameObject.activeSelf)
            {
                isAnyChatOpened = true;
                break;
            }
        }
        foreach(var obj in activeWithChatObjects)
        {
            obj.SetActive(isAnyChatOpened);
        }
    }
    private void RefreshLists()
    {
        chats = ListTools.ClearNulls(chats);   
        minitabs = ListTools.ClearNulls(minitabs);
    }
    private void CheckChat(Chat chat)
    {
        bool isWrongChat = chat.CurrentChatData.isWrongChat;

        DisableNotifications();
        if (isWrongChat)
        {
            succesfulDeleteNotificationBase.SetActive(true);
            AudioController.PlayRightSound();

            CurrentComputer.ConfirmTask();
            TimerController.SubPercentage(levels.succesfulAnswer_TimerBonus);
        }
        else
        {
            failureDeleteNotificationBase.SetActive(true);
            AudioController.PlayWrongSound();
            TimerController.AddPercentage(levels.wrongAnswer_TimerFine);
        }
    }
    private void DisableNotifications()
    {
        succesfulDeleteNotificationBase.SetActive(false);
        failureDeleteNotificationBase.SetActive(false);
    }

    #region Public functions
    public void SelectChat(Chat selectedChat)
    {
        foreach(var chat in chats)
        {
            if (chat == null)
                print("ya huesos");
            if(chat == selectedChat)
            {
                selectedChat.Open();
                selectedChat.CurrentChatMinitab.SetSelcted(true);
                lastOpenedChat = selectedChat;
            }
            else
            {
                chat.Close();
                chat.CurrentChatMinitab.SetSelcted(false);
            }
        }
        RefreshChatButtons();
        AudioController.PlayButtonSound();
        foreach (var obj in deactivateWhileChatChanges)
            obj.SetActive(false);
    }
    public void DeleteCurrentChat()
    {
        if (lastOpenedChat == null)
            return;

        CheckChat(lastOpenedChat);

        chats.Remove(lastOpenedChat);
        minitabs.Remove(lastOpenedChat.CurrentChatMinitab);

        lastOpenedChat.Delete();
        lastOpenedChat = null;
        RefreshLists();
        RefreshChatButtons();

        
    }
    #endregion
}
