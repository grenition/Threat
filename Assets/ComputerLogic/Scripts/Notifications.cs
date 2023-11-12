using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notifications : MonoBehaviour
{
    public static Notifications instance;

    [SerializeField] private UI_ResizingAnimationOnAwake notificationGameObject;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private float lifetime = 5f;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        notificationGameObject.gameObject.SetActive(false);
    }
    public static void Notify(string message)
    {
        instance.notificationGameObject.gameObject.SetActive(false);
        instance.notificationGameObject.gameObject.SetActive(true);
        instance.notificationText.ChangeText(message);

        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.CloseNotificationAfterTime(instance.lifetime));
    }
    public void CloseNotification()
    {
        notificationGameObject.CloseWithAnimation();
    }
    private IEnumerator CloseNotificationAfterTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        CloseNotification();
    }
}
