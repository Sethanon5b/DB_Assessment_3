using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    /// <summary>
    /// When the meat_boy icon is dropped onto the item slot, it will load the game scene. 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData) 
    {
        if(eventData.pointerDrag != null) 
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            SceneManager.LoadScene("Game_Scene_1");
        }
    }
}
