using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Event_DblClick : MonoBehaviour, IPointerClickHandler
{
    public static float dbl_click_timer = 0.3f;
    public UnityEvent on_dbl_click = new UnityEvent();

    float last_time = 0f;
    public void OnPointerClick(PointerEventData e){
        if (e.button == PointerEventData.InputButton.Left) {
            if (Time.unscaledTime <= last_time + dbl_click_timer) {
                //Debug.Log("Dblclick");
                if (on_dbl_click != null) on_dbl_click.Invoke();
            } else {
                last_time = Time.unscaledTime;
            }
        }
    }
}
