using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// source : https://www.youtube.com/watch?v=BGr-7GZJNXg
public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // Calls two components - "RectTransform" and "CanvasGroup".
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// This handles the functionality of picking up the 2D icon of meat boy. When he is picked up, his Alpha will lower to .6 - to make it easier to know that 
    /// he is picked up. 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData) 
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }
    // Handles dragging the object along with the mouse pointer
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    // Handles dropping the object, which then restores the 2D object's alpha back to 1.
    public void OnEndDrag(PointerEventData eventData) 
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
