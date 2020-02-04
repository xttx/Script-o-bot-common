using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Event_RightClick : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent on_right_click = new UnityEvent();

	public void OnPointerClick(PointerEventData eventData)
    {
		if (eventData.button == PointerEventData.InputButton.Right) {
			if (on_right_click != null) on_right_click.Invoke();
		}
	}
}
