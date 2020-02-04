using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Desktop_Manager_DragArea : MonoBehaviour, 
    IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Desktop_Manager.border_sides border_side;
    public Desktop_Manager desktop_manager;
    public string w_tag = "";
    public bool click_only = false;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (click_only) return;
        desktop_manager.Border_Pointer_Handler(this, Desktop_Manager.event_type.p_enter, border_side);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (click_only) return;
        desktop_manager.Border_Pointer_Handler(this, Desktop_Manager.event_type.p_exit, border_side);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        desktop_manager.Border_Pointer_Handler(this, Desktop_Manager.event_type.click, border_side);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (click_only) return;
        desktop_manager.Border_Pointer_Handler(this, Desktop_Manager.event_type.drag_begin, border_side);
    }

    public void OnDrag(PointerEventData data)
    {
        if (click_only) return;
        desktop_manager.Border_Pointer_Handler(this, Desktop_Manager.event_type.drag, border_side);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (click_only) return;
        desktop_manager.Border_Pointer_Handler(this, Desktop_Manager.event_type.drag_end, border_side);
    }
}
