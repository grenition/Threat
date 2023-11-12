using UnityEngine;
using UnityEngine.EventSystems;

public delegate void PointerEventDataHadler(PointerEventData eventData);

public class UI_ClickableObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public PointerEventDataHadler onDrag;
    public PointerEventDataHadler onBeginDrag;
    public PointerEventDataHadler onPointerDown;
    public PointerEventDataHadler onPointerUp;
    public bool isDraggable = true;
    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }
}
